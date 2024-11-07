using System;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;
using Cinemachine;

[System.Serializable]
public struct AttackData
{
    public float pushForce;
    public float attackLungeForce;
    public float attackDuration;
    public float attackComboTime;
    public float damage;
    public float offsetDistance;
    public Vector3 hitboxHalfSize;
    public float swingVolume;
    public AudioClip swingClip;
    public float hitVolume;
    public AudioClip hitClip;
}

public class PlayerAttack : MonoBehaviour
{
    public Action onEnemyKilled;
    public Action<float> onEnemyDamaged;

    [SerializeField]
    private Transform swingParticlePrefab;
    [SerializeField]
    private Transform swingImpactParticlePrefab;
    [SerializeField]
    private LayerMask whatIsHittable;
    [SerializeField]
    private float hitStopDuration;
    private CinemachineImpulseSource impulseSource;
    private AudioSource audioSource;

    [SerializeField, Header("Debug Settings")]
    private bool debug = false;
    [SerializeField]
    private Transform debugCube;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Attack(Vector3 direction, AttackData attack)
    {
        Vector3 hitboxOrigin = transform.position + direction * attack.offsetDistance;
        Quaternion hitboxOrientation = Quaternion.LookRotation(direction, Vector3.up);

        audioSource.PlayOneShot(attack.swingClip, attack.swingVolume);

        Collider[] hitColliders = Physics.OverlapBox(hitboxOrigin, attack.hitboxHalfSize, hitboxOrientation, whatIsHittable);
        bool hitFlag = false;
        foreach (var coll in hitColliders)
        {
            if (coll.TryGetComponent(out IDamageable damageable))
            {
                hitFlag = true;
                HitStop.Instance.Stop(hitStopDuration);

                damageable.TakeDamage(attack.damage, onEnemyDamaged, onEnemyKilled);

                Transform swingImpactParticles = Instantiate(swingImpactParticlePrefab, coll.transform.position, Quaternion.LookRotation(direction));
                Destroy(swingImpactParticles.gameObject, 1f);
            }

            if (coll.attachedRigidbody != null) 
            {
                // coll.attachedRigidbody?.AddForce(direction * attack.pushForce, ForceMode.Impulse);
                coll.attachedRigidbody.velocity = (direction * attack.pushForce) / coll.attachedRigidbody.mass;
            }

            if (coll.TryGetComponent(out NPC npc)) 
            {
                npc.FSM.HandleMessage(new Telegram(transform, Message.MSG_DAMAGED));
            }
        }

        Quaternion particleRotation = Quaternion.LookRotation(direction);
        Transform swingParticles = Instantiate(swingParticlePrefab, transform.position + direction * 0.5f, particleRotation);
        Destroy(swingParticles.gameObject, 1f);

        if (hitFlag)
            audioSource.PlayOneShot(attack.hitClip, attack.hitVolume);

        if (!debug)
            return;

        Transform hitbox = Instantiate(debugCube);
        hitbox.localScale = attack.hitboxHalfSize * 2f;
        hitbox.rotation = hitboxOrientation;
        hitbox.position = hitboxOrigin;
        Destroy(hitbox.gameObject, attack.attackDuration);
    }
}
