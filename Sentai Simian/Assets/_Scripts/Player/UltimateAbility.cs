using UnityEngine;
using System;

public class UltimateAbility : MonoBehaviour
{
    public Action onUltimateCast;

    [SerializeField]
    private Transform projectile;
    private PlayerAttack attack;
    [SerializeField, Min(0)]
    private int maxMeter = 500;
    private int currentMeter = 0;
    [SerializeField, Min(0)]
    private int meterFillRate = 1;
    [SerializeField]
    private AudioClip castChargeClip;
    [SerializeField]
    private AudioClip castClip;
    private AudioSource source;

    [SerializeField]
    private ParticleSystem castWindupParticles;
    [SerializeField]
    private ParticleSystem castParticles;

    public bool IsReady => currentMeter >= maxMeter;
    public float ChargePercentage => currentMeter / (float)maxMeter;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        attack = GetComponent<PlayerAttack>();
        attack.onEnemyDamaged += ChargeMeter;
    }

    private void ChargeMeter(float damage)
    {
        currentMeter += Mathf.RoundToInt(damage * meterFillRate);
        currentMeter = Mathf.Min(currentMeter, maxMeter);
    }

    public void StartCast()
    {
        source.PlayOneShot(castChargeClip);
        castWindupParticles.Play();
    }

    public void Use(Vector3 castDirection)
    {
        currentMeter = 0;
        onUltimateCast?.Invoke();
        Instantiate(projectile, transform.position, Quaternion.LookRotation(castDirection, Vector3.up));
        castParticles.Play();
        source.PlayOneShot(castClip);
    }
}
