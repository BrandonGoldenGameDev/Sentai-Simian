using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

[System.Serializable]
public class NPCShootProjectile : State<NPC> {
    private State<NPC> fleeState;

    [SerializeField]
    private Transform projectilePrefab;
    [SerializeField]
    private float stateDuration;
    [SerializeField]
    private float fireProjectileTime;
    private float timer;
    private bool fired;

    public void SetStates(State<NPC> _flee) {
        fleeState = _flee;
    }

    public override void Enter(NPC owner) {
        timer = 0f;
        fired = false;
        owner.IsPathfinding = false;
        owner.Anim.SetTrigger("OnAttack");
    }

    public override void Execute(NPC owner) {
        Vector3 dirToTarget = (owner.Target.position - owner.transform.position).normalized;
        timer += Time.deltaTime;

        if (timer >= stateDuration) {
            owner.FSM.SwitchState(fleeState);
        }
        
        if (!fired && timer >= fireProjectileTime) {
            fired = true;
            if (owner is LightRangedEnemy lre) {
                lre.FireProjectile(projectilePrefab, dirToTarget);
            }
        }

        owner.Decelerate(10f);
        owner.SetTargetRotation(Quaternion.LookRotation(dirToTarget));
    }

    public override void Exit(NPC owner) {
        owner.ReturnToken();
    }

    public override bool HandleMessage(NPC owner, Telegram telegram) {
        return base.HandleMessage(owner, telegram);
    }
}
