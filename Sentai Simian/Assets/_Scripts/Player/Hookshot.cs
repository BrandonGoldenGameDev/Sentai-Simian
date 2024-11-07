using System;
using System.Collections.Generic;
using UnityEngine;

public class Hookshot : Ability
{
    public enum HookshotState
    {
        Waiting,
        Fired,
        Grabbed,
        Returning
    }

    [SerializeField]
    private Transform hookshot;
    private HookshotProjectile projectile;
    private Transform grabbedObject;
    private PlayerMovement movement;
    private Animator animator;
    private AudioSource audioSource;

    [SerializeField]
    private float hookshotRange = 30f;
    [SerializeField]
    private float hookshotSpeed = 30f;
    [SerializeField]
    private float hookshotReturnSpeed = 60f;
    [SerializeField, Range(-1f, 1f)]
    private float snappingDotProduct = 0.7f;
    [SerializeField]
    private Transform hookshotGrabParticles;
    [SerializeField]
    private AudioClip grappleFireClip;
    [SerializeField]
    private AudioClip grappleHitClip;
    private HookshotState state = HookshotState.Waiting;
    private Vector3 targetPosition;

    [SerializeField, Range(0, 1f)]
    private float cooldownReductionPerKill = 0.5f;

    public HookshotState State => state;
    public Transform GrabbedObject => grabbedObject;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        GetComponent<PlayerAttack>().onEnemyKilled += OnEnemyKilled;
    }

    private void Start()
    {
        projectile = hookshot.GetComponent<HookshotProjectile>();
        projectile.onGrapplePointHit += OnGrapplePointHit;
        projectile.onObjectHit += OnObjectHit;

        hookshot.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        float distFromPlayer = Vector3.SqrMagnitude(movement.transform.position - hookshot.position);

        switch (state)
        {
            case HookshotState.Waiting:
                hookshot.gameObject.SetActive(false);
                break;
            case HookshotState.Fired:
                MoveToPosition(targetPosition, hookshotSpeed);
                if (distFromPlayer >= hookshotRange * hookshotRange - 0.1f)
                {
                    state = HookshotState.Returning;
                }
                break;
            case HookshotState.Grabbed:
                projectile.MovePosition(grabbedObject.position);
                break;
            case HookshotState.Returning:
                MoveToPosition(movement.transform.position, hookshotReturnSpeed);
                if (distFromPlayer <= 0f)
                {
                    state = HookshotState.Waiting;
                }
                break;
            default:
                break;
        }

        if (state == HookshotState.Waiting)
        {
            base.Update();
        }
    }

    private void MoveToPosition(Vector3 pos, float speed)
    {
        projectile.MovePosition(Vector3.MoveTowards(hookshot.position, pos, speed * Time.deltaTime));
    }

    public void Aim()
    {
        Vector3 aimDir = GetAutoAimDirection(movement.GetLastMovedDirection());
        movement.SetRotationTarget(aimDir.Flatten());
    }

    public void Fire()
    {
        hookshot.gameObject.SetActive(true);
        projectile.RegisterCollisions = true;

        Vector3 wishDir = GetAutoAimDirection(movement.GetLastMovedDirection());
        targetPosition = transform.position + wishDir * hookshotRange;
        hookshot.position = transform.position + wishDir;
        hookshot.rotation = Quaternion.LookRotation(wishDir, Vector3.up);
        state = HookshotState.Fired;
        animator.SetTrigger("OnGrappleFire");
        audioSource.PlayOneShot(grappleFireClip, 0.7f);
    }

    private Vector3 GetAutoAimDirection(Vector3 dir)
    {
        Vector3 fireDir = dir;
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, hookshotRange, LayerMask.GetMask("Enemy"));
        float lowestScore = float.MaxValue;
        Debug.DrawRay(transform.position, dir * hookshotRange, Color.magenta);

        foreach (var nearbyEnemy in nearbyEnemies)
        {
            Vector3 directionToEnemy = (nearbyEnemy.attachedRigidbody.position -  transform.position).normalized;
            float dot = Vector3.Dot(dir, directionToEnemy);

            // check if enemy is within snapping angle, saves a distance calculation
            if (dot >= snappingDotProduct)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, nearbyEnemy.attachedRigidbody.position);
                Debug.DrawLine(transform.position, transform.position + directionToEnemy * distanceToEnemy, Color.green);

                // lower score = better target
                float score = (1f - dot) * distanceToEnemy;
                if (score < lowestScore)
                {
                    // check for walls between the player and the target
                    if (!Physics.Raycast(transform.position, directionToEnemy, distanceToEnemy, LayerMask.GetMask("Default")))
                    {
                        fireDir = directionToEnemy;
                    }
                }
            }
            else
            {
                Debug.DrawLine(transform.position, nearbyEnemy.attachedRigidbody.position, Color.red);
            }
        }

        Debug.DrawRay(transform.position, fireDir * hookshotRange, Color.blue);
        return fireDir;
    }

    public void Return()
    {
        projectile.RegisterCollisions = false;
        state = HookshotState.Returning;
    }

    public void OnGrapplePointHit(Transform _grabbed)
    {
        animator.SetTrigger("OnGrapplePull");
        projectile.RegisterCollisions = false;
        state = HookshotState.Grabbed;
        grabbedObject = _grabbed;
        hookshot.position = _grabbed.position;

        Instantiate(hookshotGrabParticles, _grabbed.position, Quaternion.identity, _grabbed);
        audioSource.PlayOneShot(grappleHitClip, 0.7f);

        base.Use();
    }

    public void OnObjectHit(Transform obj)
    {
        if (state != HookshotState.Fired)
            return;

        if (obj.CompareTag("GrapplePoint") || obj.CompareTag("Enemy"))
        {
            OnGrapplePointHit(obj);
        }
        else
        {
            Return();
        }
    }

    private void OnEnemyKilled()
    {
        timer -= cooldown * cooldownReductionPerKill;
        timer = Mathf.Max(timer, 0f);
    }
}
