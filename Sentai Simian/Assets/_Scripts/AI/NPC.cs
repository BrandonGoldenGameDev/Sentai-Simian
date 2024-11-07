using System;
using System.Collections;
using UnityEngine;
using SS.StateMachine;
using UnityEditor;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public abstract class NPC : RigidbodyNavAgent, IDamageable
{
    public Action onNPCDeath;

    private StateMachine<NPC> fsm;
    protected Transform target;
    [SerializeField]
    protected SkinnedMeshRenderer model;
    protected Animator animator;
    protected CinemachineImpulseSource impulseSource;
    protected AudioSource audioSource;
    protected int actionToken = AITokenPool.INVALID_TOKEN_ID;

    [Header("NPC Settings"), SerializeField]
    private Transform healthPickup;
    [SerializeField]
    private Transform deathParticles;
    [SerializeField]
    private ParticleSystem staggeredParticles;
    [SerializeField]
    private float deathImpulseStrength = 1f;
    [SerializeField]
    private float maxHealth;
    [SerializeField, Min(0)]
    private int healthPickupsToSpawnOnDeath = 1;
    [SerializeField, Min(0)]
    private float maxHealthPickupLaunchAngle = 15f;

    private float currentHealth;
    private Coroutine hitFlashCoroutine;
    private Color baseColor;
    private bool staggered = false;

    public CinemachineImpulseSource ImpulseSource => impulseSource;
    public Animator Anim => animator;
    public StateMachine<NPC> FSM => fsm;
    public Transform Target => target;
    public AudioSource _AudioSource => audioSource;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    protected override void Awake()
    {
        base.Awake();
        fsm = new StateMachine<NPC>(this);
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentHealth = maxHealth;
        baseColor = model.sharedMaterial.color;
    }

    protected override void FixedUpdate()
    {
        fsm.Update();
        animator.SetFloat("Speed", Velocity.magnitude);
    }

    public virtual void TakeDamage(float damage, Action<float> onDamaged, Action onDeath)
    {
        float damageDealt = Mathf.Clamp(damage, 0, currentHealth);
        if (damageDealt > 0)
        {
            onDamaged?.Invoke(damageDealt);
        }

        if (staggered)
        {
            SpawnHealth(Mathf.RoundToInt(damageDealt / 10f));
        }

        currentHealth -= damageDealt;
        if (currentHealth <= 0)
        {
            onDeath?.Invoke();
            Die();
            return;
        }

        if (hitFlashCoroutine != null)
            StopCoroutine(hitFlashCoroutine);

        hitFlashCoroutine = StartCoroutine(HitFlash());
    }

    private void SpawnHealth(int healthToSpawn) 
    {
        for (int i = 0; i < healthToSpawn; i++) 
        {
            GameObject go = Instantiate(healthPickup, transform.position, Quaternion.identity).gameObject;
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();

            Vector3 direction = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, maxHealthPickupLaunchAngle), Vector3.right) * Vector3.up;
            direction = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up) * direction;
            rigidbody.velocity = direction * UnityEngine.Random.Range(5f, 10f);
        }
    }

    protected virtual void Die()
    {
        onNPCDeath?.Invoke();
        impulseSource.GenerateImpulseWithForce(deathImpulseStrength);
        GameObject go = Instantiate(deathParticles, transform.position, Quaternion.identity).gameObject;
        SpawnHealth(healthPickupsToSpawnOnDeath);
        Destroy(go, 5f);
        Destroy(gameObject);

        ReturnToken();
    }

    private IEnumerator HitFlash()
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        model.GetPropertyBlock(block);

        block.SetColor("_BaseColor", Color.white * 2f);
        model.SetPropertyBlock(block);

        yield return new WaitForSecondsRealtime(0.1f);
        
        block.SetColor("_BaseColor", baseColor);
        model.SetPropertyBlock(block);
    }

    public void GenerateImpulseWithVelocity(Vector3 _velocity) => impulseSource.GenerateImpulseWithVelocity(_velocity);
    public void GenerateImpulse() => impulseSource.GenerateImpulse();

    public void StartStagger() {
        staggered = true;
        staggeredParticles.Play(true);
    }

    public void EndStagger() {
        staggered = false;
        staggeredParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void OnDestroy() {
        ReturnToken();
    }

    public abstract bool TryClaimToken();
    public abstract void ReturnToken();

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.Label(transform.position, FSM?.CurrentState?.ToString());
    }
#endif

}
