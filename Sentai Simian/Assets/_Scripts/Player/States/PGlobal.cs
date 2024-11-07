using System.Collections;
using UnityEngine;
using SS.StateMachine;

[System.Serializable]
public class PGlobal : State<PlayerMovement>
{
    public override bool HandleMessage(PlayerMovement owner, Telegram telegram)
    {
        switch (telegram.message)
        {
            case Message.MSG_DEAD:
                owner.FSM.SwitchState(owner.states.DeadState);
                return true;
        }

        return false;
    }
}