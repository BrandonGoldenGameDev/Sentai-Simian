using SS.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PDead : State<PlayerMovement>
{
    public override void Enter(PlayerMovement owner)
    {
        owner.Anim.SetTrigger("OnDeath");
    }

    public override void Execute(PlayerMovement owner)
    {
        owner.Velocity = Vector3.zero;
    }

    public override void Exit(PlayerMovement owner)
    {
    }
}
