using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;
using UnityEngine.AI;

[System.Serializable]
public class HMChase : State<NPC>
{
    [SerializeField]
    private float chargeCooldown;
    [SerializeField]
    private float maxChargeRange = 15f;
    private float chargeTimer = 0f;

    private State<NPC> chargeState;

    public void SetStates(State<NPC> _charge)
    {
        chargeState = _charge;
    }

    public override void Enter(NPC owner)
    {
        owner.IsPathfinding = true;
    }

    public override void Execute(NPC owner)
    {
        // Check if position is actually near a nav mesh.
        if (NavMesh.SamplePosition(owner.Target.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            owner.SetDestination(hit.position);
            owner.GoToDestination();
        }

        chargeTimer += Time.deltaTime;
        if (chargeTimer >= chargeCooldown) {
            if (Vector3.Distance(owner.Target.position, owner.transform.position) <= maxChargeRange) {
                NavMesh.Raycast(owner.transform.position, owner.Target.position, out hit, NavMesh.AllAreas);
                if (!hit.hit) {
                    if (owner.TryClaimToken()) {
                        chargeTimer = 0f;
                        owner.FSM.SwitchState(chargeState);
                    }
                }
            }
        }
    }

    public override void Exit(NPC owner)
    {
    }
}
