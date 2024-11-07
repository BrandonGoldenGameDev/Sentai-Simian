using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.Audio;

public class PlayerHealth : MonoBehaviour
{
    public Action onPlayerDeath;
    public Action<int> onPlayerDamaged;

    private CinemachineImpulseSource impulseSource;
    private Animator animator;
    private PlayerMovement player;
    [SerializeField]
    private Collider hitbox;
    private bool isInvincible;
    [SerializeField]
    private float pickupVolume;
    [SerializeField]
    private AudioClip pickupClip;
    [SerializeField]
    private ParticleSystem healthPickupParticles;
    [SerializeField]
    private float hurtVolume;
    [SerializeField]
    private AudioClip hurtClip;
    private AudioSource audioSource;

    [SerializeField]
    private int maxHealth = 100;
    private int currentHealth;

    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private GameObject deathCamera;

    public bool RepulsorActive { get; set; }
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public PlayerMovement Player { get => player; }

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = GetComponent<PlayerMovement>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
            return;

        if (currentHealth <= 0)
            return;

        if (!RepulsorActive)
        {
            audioSource.PlayOneShot(hurtClip, hurtVolume);
            currentHealth -= damage;
            onPlayerDamaged?.Invoke(damage);
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }

        impulseSource.GenerateImpulse();
    }

    public void Heal(int amount)
    {
        audioSource.PlayOneShot(pickupClip, pickupVolume);

        int healthBefore = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        int totalHealthHealed = currentHealth - healthBefore;

        healthPickupParticles.Emit(totalHealthHealed / 10);
    }

    private void Die()
    {
        animator.SetTrigger("OnDeath");
        player.FSM.HandleMessage(new SS.StateMachine.Telegram(transform, SS.StateMachine.Message.MSG_DEAD));
        mainCamera.SetActive(false);
        deathCamera.SetActive(true);
        onPlayerDeath?.Invoke();
    }

    public void InvincibilityOn()
    {
        hitbox.enabled = false;
        isInvincible = true;
    }

    public void InvincibilityOff()
    {
        hitbox.enabled = true;
        isInvincible = false;
    }
}
