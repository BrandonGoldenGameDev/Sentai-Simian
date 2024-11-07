using System;
using UnityEngine;

public class HookshotProjectile : MonoBehaviour
{
    public Action<Transform> onObjectHit;
    public Action<Transform> onGrapplePointHit;
    private bool registerCollisions = false;

    [SerializeField]
    private Transform grappleOrigin;

    private LineRenderer rope;
    private Collider coll;

    public bool RegisterCollisions
    {
        get => registerCollisions;
        set 
        { 
            registerCollisions = value;
            coll.enabled = value;
        } 
    }

    private void Awake()
    {
        rope = GetComponent<LineRenderer>();
        coll = GetComponent<Collider>();
    }

    private void Update()
    {
        if (!RegisterCollisions)
            return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("GrapplePoint") || collider.CompareTag("Enemy"))
            {
                onGrapplePointHit?.Invoke(collider.transform);
                return;
            }
        }
    }

    private void LateUpdate()
    {
        rope.SetPosition(0, grappleOrigin.position);
        rope.SetPosition(1, transform.position);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!RegisterCollisions)
            return;

        if (collider.isTrigger)
            return;

        onObjectHit?.Invoke(collider.transform);
    }

    public void MovePosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
