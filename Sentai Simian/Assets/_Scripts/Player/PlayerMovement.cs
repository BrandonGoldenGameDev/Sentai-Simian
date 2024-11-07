using System;
using UnityEngine;
using UnityEngine.InputSystem;
using SS.StateMachine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Collider coll;
    private PlayerInput input;
    private Repulsor repulsor;
    private Hookshot hookshot;
    private PlayerAttack attack;
    private PlayerHealth health;
    private Dash dash;
    private UltimateAbility ultimate;
    private AudioSource audioSource;
    private Animator animator;
    [Header("References"), SerializeField]
    private Transform cam;
    [SerializeField]
    private Transform model;
    [SerializeField]
    private AudioClip dashClip;

    // Inputs
    private InputAction movementInputAction;
    private InputAction dashInputAction;
    private InputAction repulsorInputAction;
    private InputAction hookshotInputAction;
    private InputAction attackInputAction;
    private InputAction ultimateInputAction;

    public Action<PlayerMovement> attackAction;
    public Action<PlayerMovement> repulsorAction;
    public Action<PlayerMovement> dashAction;
    public Action<PlayerMovement> aimAction;
    public Action<PlayerMovement> fireAction;
    public Action<PlayerMovement> ultimateAction;

    private StateMachine<PlayerMovement> fsm;
    public PlayerStates states;

    private Vector3 velocity;
    private Vector3 lastMoveDirection = Vector3.forward;
    private Quaternion targetRotation;

    private int contactCount = 0;
    private Vector3 groundNormal = Vector3.up;

    public Repulsor a_Repulsor => repulsor;
    public Hookshot a_Hookshot => hookshot;
    public Dash a_Dash => dash;
    public PlayerAttack a_Attack => attack;
    public PlayerHealth Health => health;
    public Animator Anim => animator;
    public UltimateAbility Ultimate => ultimate;
    public StateMachine<PlayerMovement> FSM => fsm;
    public Vector2 MovementInput => Vector2.ClampMagnitude(movementInputAction.ReadValue<Vector2>(), 1);
    public Vector3 Velocity { get => velocity; set => velocity = value; }
    public bool IsGrounded => contactCount > 0;

    private void Awake()
    {
        GetReferencesToComponents();
        InitializeInputs();
        CreateStateMachine();

        targetRotation = model.rotation;
    }

    private void GetReferencesToComponents()
    {
        rb =        GetComponent<Rigidbody>();
        coll =      GetComponent<Collider>();
        input =     GetComponent<PlayerInput>();
        repulsor =  GetComponent<Repulsor>();
        attack =    GetComponent<PlayerAttack>();
        hookshot =  GetComponent<Hookshot>();
        dash =      GetComponent<Dash>();
        health =    GetComponent<PlayerHealth>();
        ultimate =  GetComponent<UltimateAbility>();
        audioSource = GetComponent<AudioSource>();
        animator =  GetComponentInChildren<Animator>();
    }

    private void InitializeInputs()
    {
        movementInputAction = input.actions["Movement"];
        dashInputAction = input.actions["Dash"];
        repulsorInputAction = input.actions["Use Repulsor"];
        hookshotInputAction = input.actions["Grapple"];
        attackInputAction = input.actions["Attack"];
        ultimateInputAction = input.actions["Cast Ultimate"];

        dashInputAction.performed += ctx => dashAction?.Invoke(this);
        repulsorInputAction.performed += ctx => repulsorAction?.Invoke(this);
        hookshotInputAction.performed += ctx => aimAction?.Invoke(this);
        hookshotInputAction.canceled += ctx => fireAction?.Invoke(this);
        attackInputAction.performed += ctx => attackAction?.Invoke(this);
        ultimateInputAction.performed += ctx => ultimateAction?.Invoke(this);
    }

    private void CreateStateMachine()
    {
        fsm = new StateMachine<PlayerMovement>(this);
        fsm.SetGlobalState(states.GlobalState);
        fsm.SwitchState(states.GroundState);
    }

    private void Update()
    {   
        model.rotation = Quaternion.Slerp(model.rotation, targetRotation, 15f * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        velocity = rb.velocity;

        fsm.Update();

        animator.SetFloat("Speed", velocity.magnitude);
        rb.velocity = velocity;

        ResetGroundState();
    }

    private void ResetGroundState()
    {
        groundNormal = Vector3.up;
        contactCount = 0;
    }

    #region Movement Functions
    public void Accelerate(Vector3 wishDir, float wishSpeed, float acceleration)
    {
        Vector3 vec = GetMovementVelocity();

        if (IsGrounded)
            wishDir = AlignVectorWithGround(wishDir);

        Vector3 wishVel = wishDir * wishSpeed;
        Vector3 pushDir = wishVel - vec;
        float pushSpeed = pushDir.magnitude;
        pushDir.Normalize();

        float canPush = acceleration * Time.deltaTime * wishSpeed;
        if (pushSpeed < canPush)
            canPush = pushSpeed;

        velocity += canPush * pushDir;
    }

    public void Decelerate(float deceleration)
    {
        Vector3 vec = GetMovementVelocity();

        float speed = velocity.magnitude;
        float drop = deceleration * Time.deltaTime * speed;
        float newSpeed = speed - drop;

        if (newSpeed > 0)
            newSpeed /= speed;
        else
            newSpeed = 0;

        velocity.x *= newSpeed;
        if (IsGrounded) velocity.y *= newSpeed;
        velocity.z *= newSpeed;
    }

    public void ApplyGravity(float gravity)
    {
        if (!IsGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }

    public void SetRotation(Vector3 dir)
    {
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        targetRotation = rot;
        model.rotation = rot;
    }

    public void SetRotationTarget(Vector3 dir)
    {
        targetRotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    public void Dash()
    {
        audioSource.PlayOneShot(dashClip);
    }
    #endregion

    #region Vector Manipulation
    public Vector3 AlignVectorWithGround(Vector3 vec)
    {
        if (!IsGrounded)
            return vec;

        Vector3 alignedVec = Vector3.ProjectOnPlane(vec, groundNormal).normalized;
        if (vec.sqrMagnitude != 1)
        {
            float length = vec.magnitude;
            alignedVec = alignedVec * length;
        }

        return alignedVec;
    }
    #endregion

    /// <summary>
    /// Gets the current direction the player is facing. This direction is based on the last movement direciton
    /// the player input. 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetLastMovedDirection()
    {
        Vector3 wishDir = GetWishDir();
        if (wishDir.sqrMagnitude > 0)
        {
            lastMoveDirection = wishDir.normalized;
            SetRotationTarget(wishDir);
        }

        return lastMoveDirection;
    }

    /// <summary>
    /// Gets the player's desired movement vector relative to the game camera.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWishDir()
    {
        Vector3 cameraForward = cam.forward.Flatten();
        Vector3 cameraRight = cam.right.Flatten();
        Vector3 movementInput = MovementInput;
        return movementInput.x * cameraRight + movementInput.y * cameraForward;
    }

    /// <summary>
    /// Returns the player's velocity, but removes any velocity affected by gravity if the player is in the air.
    /// This is important for fucntions that wish to know the player's speed, but want to ignore the player falling.
    /// </summary>
    /// <returns>Vector3: velocity</returns>
    public Vector3 GetMovementVelocity()
    {
        if (IsGrounded)
        {
            return velocity;
        }

        Vector3 vec = velocity;
        vec.y = 0f;

        return vec;
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            float angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle <= 45)
            {
                groundNormal = contact.normal;
                contactCount++;
            }
        }
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.Label(new Rect(100f, 80f, 300f, 20f), "State: " + fsm.CurrentState.ToString());
        GUI.Label(new Rect(100f, 100f, 300f, 20f), $"Speed: {velocity.magnitude}");
        GUI.Label(new Rect(100f, 120f, 300f, 20f), $"Ground Contacts: {contactCount}");
    }
#endif
}
