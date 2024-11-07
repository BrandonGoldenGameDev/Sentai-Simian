using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtTrigger : MonoBehaviour
{
    [SerializeField]
    private int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage, null, null);
        }
        else if (other.TryGetComponent(out PlayerHealth player))
        {
            player.TakeDamage(damage);
        }
    }
}
