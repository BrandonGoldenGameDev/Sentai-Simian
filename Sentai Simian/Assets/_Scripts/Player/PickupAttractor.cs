using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAttractor : MonoBehaviour
{
    [SerializeField]
    private float radius;

    private LayerMask pickupLayer;
    private Collider[] colliders;

    private void Start()
    {
        pickupLayer = LayerMask.GetMask("Pickup");
        colliders = new Collider[100];
    }

    private void Update()
    {
        int amt = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders, pickupLayer);
        for (int i = 0; i < amt; i++)
        {
            colliders[i].GetComponent<HealthPickup>().Attract(transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
