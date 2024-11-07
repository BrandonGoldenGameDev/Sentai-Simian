using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

[System.Serializable]
public class NPCStaggered : State<NPC>
{
    [SerializeField]
    private float deceleration;
    [SerializeField]
    private float staggerDuration = 3f;
    private float timer;

    private State<NPC> onExitState;

    public void SetStates(State<NPC> _exit)
    {
        onExitState = _exit;
    }

    public override void Enter(NPC owner)
    {
        owner.StartStagger();
        timer = staggerDuration;
    }

    public override void Execute(NPC owner)
    {
        timer -= Time.deltaTime;
        
        if (timer < 0)
        {
            owner.FSM.SwitchState(onExitState);
        }

        owner.Decelerate(10f);
    }

    public override void Exit(NPC owner)
    {
        owner.EndStagger();
    }

    public override bool HandleMessage(NPC owner, Telegram telegram)
    {
        switch (telegram.message)
        {
            case Message.MSG_DAMAGED:
                return true;
        }

        return false;
    }
}
