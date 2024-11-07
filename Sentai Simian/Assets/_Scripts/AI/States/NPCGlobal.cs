using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

[System.Serializable]
public class NPCGlobal : State<NPC>
{
    private State<NPC> staggerState;
    
    public void SetStates(State<NPC> _stagger)
    {
        staggerState = _stagger;
    }

    public override bool HandleMessage(NPC owner, Telegram telegram)
    {
        switch (telegram.message)
        {
            case Message.MSG_DAMAGED:
                return true;
            case Message.MSG_STAGGERED:
                owner.FSM.SwitchState(staggerState);
                return true;
        }

        return false;
    }
}
