using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

[System.Serializable]
public class HRFireLaser : State<NPC>
{
    [SerializeField]
    private float duration;
    private float timer = 0f;

    private State<NPC> exitState;

    public void SetStates(State<NPC> _exit)
    {
        exitState = _exit;
    }

    public override void Enter(NPC owner)
    {
        var hre = owner as HeavyRangedEnemy;
        if (hre != null)
        {
            timer = 0f;
            hre.FireLaserAtTarget();
        }
    }

    public override void Execute(NPC owner)
    {
        owner.Decelerate();

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            owner.FSM.SwitchState(exitState);
        }
    }

    public override void Exit(NPC owner)
    {
        owner.Anim.SetBool("IsAttacking", false);
    }

    public override bool HandleMessage(NPC owner, Telegram telegram)
    {
        return base.HandleMessage(owner, telegram);
    }
}
