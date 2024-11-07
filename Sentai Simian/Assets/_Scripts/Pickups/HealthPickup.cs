using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField]
    private int healAmount;
    [SerializeField]
    private float acceleration, maxSpeed;
    [SerializeField]
    private float minIdleTime = 0.5f;
    private float currentSpeed;
    private float timeAlive = 0f;
    private Collider coll;

    public Transform Target { get; set; }
    public float TimeAlive => timeAlive;

    private void Awake() 
    {
        coll = GetComponent<Collider>();
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;

        if (Target == null) {
            if (timeAlive >= 60f) {
                Destroy(gameObject);
            }

            return;
        }

        currentSpeed += acceleration * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        Vector3 newPos = Vector3.MoveTowards(transform.position, Target.position, currentSpeed * Time.deltaTime);
        transform.position = newPos;

        if (Vector3.Distance(transform.position, Target.position) <= 0.25f)
        {
            Target.GetComponentInParent<PlayerHealth>()?.Heal(healAmount);
            Destroy(gameObject);
        }
    }

    public void Attract(Transform newTarget) 
    {
        if (Target != null) {
            return;
        }

        if (timeAlive < minIdleTime) {
            return;
        }

        Target = newTarget;
        coll.attachedRigidbody.isKinematic = true;
        coll.enabled = false;
    }
}
