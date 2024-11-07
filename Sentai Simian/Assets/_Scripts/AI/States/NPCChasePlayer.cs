using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;
using UnityEngine.AI;

[System.Serializable]
public class NPCChasePlayer : State<NPC> {
    [SerializeField]
    private float attackStartDistance = 2f;
    [SerializeField]
    private float attackCooldown = 3f;
    private float attackTimer = 0f;

    private State<NPC> attackState;

    public void SetStates(State<NPC> _attack) {
        attackState = _attack;
    }

    public override void Enter(NPC owner) {
        owner.IsPathfinding = true;
    }

    public override void Execute(NPC owner) {
        // Check if position is actually near a nav mesh.
        if (NavMesh.SamplePosition(owner.Target.position, out NavMeshHit hit, 3f, NavMesh.AllAreas)) {
            owner.SetDestination(hit.position);
            owner.GoToDestination();
        }

        if (attackTimer >= attackCooldown) {
            float sqrDistToTarget = (owner.Target.position - owner.transform.position).sqrMagnitude;
            if (sqrDistToTarget <= attackStartDistance * attackStartDistance)             {
                if (owner.TryClaimToken()) {
                    attackTimer = 0f;
                    owner.FSM.SwitchState(attackState);
                }
            }
        }
        else {
            attackTimer += Time.deltaTime;
        }
    }
}
