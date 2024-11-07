using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyNavAgent : MonoBehaviour
{
    protected Rigidbody rb;
    
    [Header("Navigation Settings"), SerializeField]
    private float deceleration = 15f;
    [SerializeField]
    private float acceleration = 20f;
    [SerializeField]
    private float maxSpeed = 5f;
    [SerializeField]
    private float stoppingDistance = 2f;
    [SerializeField]
    private float nextWaypointDistance = 1f;
    private bool isPathfinding = true;

    private Quaternion targetRotation;
    private Vector3 targetPos;
    private NavMeshPath path;
    private int currentWaypoint = 0;

    public bool IsPathfinding { get => isPathfinding; set => isPathfinding = value; }
    public void SetDestination(Vector3 _target) => targetPos = _target;
    public void SetTargetRotation(Quaternion rotation) => targetRotation = rotation;
    public Vector3 Velocity => rb.velocity;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();

        InvokeRepeating("FindPath", 0f, 0.25f);
        targetRotation = transform.rotation;
    }

    protected virtual void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

        if (path == null || path.status == NavMeshPathStatus.PathInvalid)
            return;

        DrawPath();
    }

    protected virtual void FixedUpdate() { }

    public void GoToDestination()
    {
        if (path == null || path.status == NavMeshPathStatus.PathInvalid)
        {
            Decelerate();
            return;
        }

        bool reachedEndOfPath = FindNextWaypoint();
        if (!reachedEndOfPath)
            Seek();
        else
            Arrive();
    }

    private void Seek()
    {
        Vector3 vel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 desiredVelocity = (path.corners[currentWaypoint] - transform.position).Flatten();
        desiredVelocity *= maxSpeed;
        Vector3 pushDir = desiredVelocity - vel;
        float pushAmt = pushDir.magnitude;
        pushDir.Normalize();

        targetRotation = Quaternion.LookRotation(desiredVelocity, Vector3.up);

        float canPush = acceleration * Time.deltaTime;
        if (pushAmt < canPush)
        {
            canPush = pushAmt;
        }

        rb.velocity += pushDir * canPush;
    }

    private void Arrive()
    {
        Vector3 toTarget = path.corners[currentWaypoint] - transform.position;
        toTarget.y = 0f;
        float dist = toTarget.magnitude;

        if (dist > 0)
        {
            Decelerate(deceleration);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        Vector3 dirToTarget = (targetPos - transform.position).Flatten();
        targetRotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
    }

    public void Decelerate(float decel)
    {
        Vector3 vel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float speed = vel.magnitude;
        float drop = decel * Time.deltaTime;
        float newSpeed = speed - drop;

        if (newSpeed < 0)
            newSpeed = 0;
        if (speed > 0)
            newSpeed /= speed;

        vel.x *= newSpeed;
        vel.y = rb.velocity.y;
        vel.z *= newSpeed;

        rb.velocity = vel;
    }

    public void Decelerate()
    {
        Decelerate(deceleration);
    }

    private bool FindNextWaypoint()
    {
        while (true)
        {
            Vector3 vectorToWaypoint = path.corners[currentWaypoint] - transform.position;
            float sqrDist = vectorToWaypoint.sqrMagnitude;

            // Get next waypoint
            if (sqrDist <= nextWaypointDistance * nextWaypointDistance)
            {
                if (currentWaypoint + 1 < path.corners.Length)
                {
                    currentWaypoint++;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }

    private void FindPath()
    {
        if (!isPathfinding)
            return;

        if (path == null)
            path = new NavMeshPath();

        currentWaypoint = 0;
        if (!NavMesh.CalculatePath(transform.position, targetPos, NavMesh.AllAreas, path))
        {
        }
    }

    private void DrawPath()
    {
        for (int i = 0; i < path.corners.Length; i++)
        {
            if (i == 0)
                continue;

            Debug.DrawLine(path.corners[i - 1], path.corners[i]);
        }
    }

    public void SetVelocity(Vector3 vel)
    {
        rb.velocity = vel;
    }
}
