using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private const string EnemyProjectileLayerName = "Enemy Projectile";
    private const string PlayerProjectileLayerName = "Player Projectile";

    private Rigidbody rb;
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private int playerDamage;
    [SerializeField]
    private int enemyDamage;
    [SerializeField]
    private float lifetime = 10f;
    private float timer = 0f;

    private Vector3 movementDirection;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movementDirection = transform.forward;
    }

    void Update()
    {
        rb.velocity = movementDirection * speed;
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = collision.gameObject.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(playerDamage);
                health.Player.FSM.HandleMessage(new SS.StateMachine.Telegram(transform, SS.StateMachine.Message.MSG_DAMAGED));
            }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.TryGetComponent(out NPC npc))
            {
                HitStop.Instance.Stop(0.2f);
                npc.TakeDamage(enemyDamage, null, null);
            }
        }

        Destroy(gameObject);
    }

    public void Reflect(bool toEnemy)
    {
        int newLayer = LayerMask.NameToLayer(toEnemy ? PlayerProjectileLayerName : EnemyProjectileLayerName);
        rb.gameObject.layer = newLayer;

        rb.velocity = -rb.velocity;
        movementDirection = -movementDirection;
        transform.rotation = Quaternion.LookRotation(rb.velocity);

        speed *= 2;
    }
}
