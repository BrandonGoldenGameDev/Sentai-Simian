using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

[System.Serializable]
public class LMAttack : State<NPC>
{
    [SerializeField]
    private float attackDuration = 2f;
    [SerializeField]
    private float attackTriggerTime = 0.5f;
    private float timer = 0f;
    private bool attacked = false;
    [SerializeField]
    private float attackWindUpVolume;
    [SerializeField]
    private AudioClip attackWindUpSound;

    private State<NPC> chaseState;

    public void SetStates(State<NPC> _chase)
    {
        chaseState = _chase;
    }

    public override void Enter(NPC owner)
    {
        owner._AudioSource.PlayOneShot(attackWindUpSound, attackWindUpVolume);
        owner.Anim.SetTrigger("OnAttack");
        timer = 0f;
        attacked = false;
        owner.IsPathfinding = false;

        Vector3 dirToTarget = owner.Target.position - owner.transform.position;
        dirToTarget.Flatten();
    }

    public override void Execute(NPC owner)
    {
        if (timer >= attackDuration)
        {
            owner.FSM.SwitchState(chaseState);
        }

        if (!attacked && timer >= attackTriggerTime)
        {
            attacked = true;
            var e = (LightMeleeEnemy)owner;
            e.Attack(3f);
        }

        timer += Time.deltaTime;

        owner.Decelerate(10f);
    }

    public override void Exit(NPC owner) {
        owner.ReturnToken();
    }
}
