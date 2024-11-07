using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UltimateProjectile : MonoBehaviour
{
    private Rigidbody rb;
    private Collider coll;

    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float damage;
    [SerializeField]
    private Transform impactParticles;

    private void Awake()
    {
        coll = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = transform.forward;
        rb.MovePosition(transform.position + (moveDirection * speed * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Physics.IgnoreCollision(collision.collider, coll);
        if (collision.collider.CompareTag("Enemy"))
        {
            if (collision.collider.TryGetComponent(out NPC npc))
            {
                npc.TakeDamage(damage, null, null);
                HitStop.Instance.Stop(0.15f);
            }
        }
        else
        {
            ContactPoint contact = collision.GetContact(0);
            Instantiate(impactParticles, contact.point, Quaternion.LookRotation(contact.normal));

            Destroy(gameObject);
        }
    }
}
