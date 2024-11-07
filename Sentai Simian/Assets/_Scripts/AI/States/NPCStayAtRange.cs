using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;
using UnityEngine.AI;

[System.Serializable]
public class NPCStayAtRange : State<NPC>
{
    [SerializeField]
    private float minRange;
    [SerializeField]
    private float maxRange;
    [SerializeField]
    private float numOfFeelers;
    [SerializeField]
    private float angleBetweenFeelers;
    [SerializeField]
    private float feelerDistance = 10f;
    [SerializeField]
    private float attackCooldown = 2f;
    private float timer;

    private State<NPC> attackState;

    public override void Enter(NPC owner)
    {
        timer = 0f;
        owner.IsPathfinding = true;
    }

    public void SetStates(State<NPC> _attack)
    {
        attackState = _attack;
    }

    public override void Execute(NPC owner)
    {
        Vector3 dirAwayFromTarget = owner.transform.position - owner.Target.position;

        NavMeshPath path = new();
        float distFromTarget;
        if (NavMesh.CalculatePath(owner.transform.position, owner.Target.position, NavMesh.AllAreas, path)) {
            distFromTarget = CalculatePathDistance(path);
        } else {
            distFromTarget = dirAwayFromTarget.magnitude;
        }

        dirAwayFromTarget = dirAwayFromTarget.Flatten();

        if (distFromTarget < minRange)
        {
            FleeTarget(owner, dirAwayFromTarget, distFromTarget);
        }
        else if (distFromTarget > maxRange)
        {
            // Check if position is actually near a nav mesh.
            if (NavMesh.SamplePosition(owner.Target.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                owner.SetDestination(hit.position);
            }
        }
        
        timer += Time.deltaTime;
        if (distFromTarget < maxRange && timer >= attackCooldown)
        {
            Vector3 dirToTarget = (owner.Target.position - owner.transform.position).normalized;
            if (!Physics.Raycast(owner.transform.position, dirToTarget, out RaycastHit hit, distFromTarget, LayerMask.GetMask("Default")))
            {
                if (owner.TryClaimToken()) {
                    owner.FSM.SwitchState(attackState);
                }
            }
        }

        owner.GoToDestination();
    }

    private static float CalculatePathDistance(NavMeshPath path) {
        float distFromTarget = 0.0f;
        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1)) {
            for (int i = 1; i < path.corners.Length; ++i) {
                distFromTarget += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return distFromTarget;
    }

    private void FleeTarget(NPC owner, Vector3 dirAwayFromTarget, float distFromTarget)
    {
        Vector3 testPosition = owner.transform.position + dirAwayFromTarget * feelerDistance;

        if (!NavMesh.Raycast(owner.transform.position, testPosition, out NavMeshHit hit, NavMesh.AllAreas)) {
            Debug.DrawLine(owner.transform.position, testPosition, Color.magenta);
            owner.SetDestination(testPosition);
        } else {
            float bestDistance = 0f;
            Vector3 bestPosition = owner.transform.position;
            bool positionFound = false;
            for (int i = 1; i <= numOfFeelers; i++) {
                float raycastLength = (minRange + (maxRange - minRange) * 0.5f) - distFromTarget;
                Vector3 rightFeeler = RotateVector(dirAwayFromTarget, angleBetweenFeelers * i, Vector3.up) * raycastLength;
                Vector3 leftFeeler = RotateVector(dirAwayFromTarget, -angleBetweenFeelers * i, Vector3.up) * raycastLength;
                Vector3 rightTestPosition = owner.transform.position + rightFeeler;
                Vector3 leftTestPosition = owner.transform.position + leftFeeler;

                if (!NavMesh.Raycast(owner.transform.position, rightTestPosition, out NavMeshHit rightHit, NavMesh.AllAreas)) {
                    Debug.DrawLine(owner.transform.position, rightTestPosition, Color.red);
                    owner.SetDestination(rightTestPosition);
                    positionFound = true;
                    break;
                } else {
                    float dist = rightHit.distance;
                    if (dist > bestDistance) {
                        bestDistance = dist;
                        bestPosition = rightHit.position;
                    }
                }

                if (!NavMesh.Raycast(owner.transform.position, leftTestPosition, out NavMeshHit leftHit, NavMesh.AllAreas)) {
                    Debug.DrawLine(owner.transform.position, leftTestPosition, Color.blue);
                    owner.SetDestination(leftTestPosition);
                    positionFound = true;
                    break;
                } else {
                    float dist = leftHit.distance;
                    if (dist > bestDistance) {
                        bestDistance = dist;
                        bestPosition = leftHit.position;
                    }
                }

                Debug.DrawLine(owner.transform.position, rightHit.position, Color.red);
                Debug.DrawLine(owner.transform.position, leftHit.position, Color.blue);
            }

            if (!positionFound) {
                owner.SetDestination(bestPosition);
            }
        }
    }

    public override void Exit(NPC owner)
    {
    }
    
    public Vector3 RotateVector(Vector3 vector, float degrees, Vector3 axis)
    {
        return Quaternion.AngleAxis(degrees, axis) * vector;
    }
}
