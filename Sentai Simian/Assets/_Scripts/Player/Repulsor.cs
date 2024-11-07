using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

public class Repulsor : Ability
{
    public Action onSuccesfulParry;

    private AudioSource audioSource;
    private PlayerHealth health;
    [SerializeField]
    private Transform repulsorStaggerParticleEffect;
    [SerializeField]
    private Transform repulsorPushParticleEffect;
    [SerializeField]
    private LayerMask repulsorLayer;
    [SerializeField]
    private float parryVolume;
    [SerializeField]
    private AudioClip parryClip;
    [SerializeField]
    private float force = 20f;
    [SerializeField]
    private float range = 5f;
    [SerializeField]
    private Transform reflectedLaserPrefab;
    [SerializeField, Range(0, 1f)]
    private float cooldownReductionPerKill = 0.5f;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        audioSource = GetComponent<AudioSource>();
        GetComponent<PlayerAttack>().onEnemyKilled += OnEnemyKilled;
    }

    public void TriggerStaggerRepulsor(Action onHitStopEnded = null)
    {
        onHitStopEnded += () => RepulsorBlast(repulsorStaggerParticleEffect, coll =>
        {
            if (coll.TryGetComponent(out NPC npc))
                npc.FSM.HandleMessage(new Telegram(transform, Message.MSG_STAGGERED));
        });
        
        onSuccesfulParry?.Invoke();
        audioSource.PlayOneShot(parryClip, parryVolume);
        HitStop.Instance.Stop(0.3f, true, onHitStopEnded);
    }

    public void TriggerWeakRepulsor() => RepulsorBlast(repulsorPushParticleEffect);

    private void RepulsorBlast(Transform particleEffect, Action<Collider> onHitWithRepulsor = null)
    {
        Use();
        health.RepulsorActive = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, repulsorLayer);
        foreach (var coll in colliders)
        {
            PushRigidbody(coll, (coll.transform.position - transform.position).normalized);

            if (coll.TryGetComponent(out Projectile projectile))
            {
                projectile.Reflect(true);
            }

            onHitWithRepulsor?.Invoke(coll);
        }

        Instantiate(particleEffect, transform.position, Quaternion.identity);
    }

    private void PushRigidbody(Collider coll, Vector3 dir)
    {
        coll.attachedRigidbody?.AddForce(dir * force, ForceMode.Impulse);
    }

    public void ReflectLaser(Transform target)
    {
        var reflectedLaserGO = Instantiate(reflectedLaserPrefab, transform.position, Quaternion.identity);
        var reflectedLaser = reflectedLaserGO.GetComponent<ReflectedLaser>();
        TriggerStaggerRepulsor(() => reflectedLaser?.ReflectLaser(target));
    }

    private void OnEnemyKilled()
    {
        timer -= cooldown * cooldownReductionPerKill;
        timer = Mathf.Max(timer, 0f);
    }
}
