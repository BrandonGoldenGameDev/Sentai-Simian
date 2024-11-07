using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectedLaser : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField]
    private float hitstopDuration;
    [SerializeField]
    private float damage;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void ReflectLaser(Transform target)
    {
        Vector3 origin = transform.position;
        Vector3 direction = (target.position - origin).normalized;
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, Mathf.Infinity, LayerMask.GetMask("Default", "Enemy"));

        // Ensure that the array is sorted from closest to furthest
        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        lineRenderer.SetPosition(0, origin);
        StartCoroutine(IterateThroughLaserHits(hits, direction));
    }

    private IEnumerator IterateThroughLaserHits(RaycastHit[] hits, Vector3 dir)
    {
        bool hitWall = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                lineRenderer.SetPosition(1, hit.point);
                if (hit.transform.TryGetComponent(out NPC npc))
                {
                    npc.TakeDamage(damage, null, null);
                    HitStop.Instance.Stop(hitstopDuration, true);
                    yield return new WaitForSecondsRealtime(hitstopDuration + 0.05f);
                }
            }
            else
            {
                // We hit a wall
                hitWall = true;
                lineRenderer.SetPosition(1, hit.point);
                break;
            }
        }

        if (!hitWall)
            lineRenderer.SetPosition(1, transform.position + dir * 200f);

        Destroy(gameObject, 1f);
    }
}
