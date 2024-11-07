using SS.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PHookshotAim : State<PlayerMovement>
{
    [SerializeField]
    private float gravity;

    public override void Enter(PlayerMovement owner)
    {
        owner.attackAction  += AttackActionPerformed;
        owner.fireAction    += FireActionPerformed;
        owner.dashAction    += DashActionPerformed;
        owner.ultimateAction += UltimateActionPerformed;
        owner.Anim.SetBool("AimingGrapple", true);
    }

    public override void Execute(PlayerMovement owner)
    {
        owner.Velocity = new Vector3(0f, owner.Velocity.y, 0f);
        owner.ApplyGravity(gravity);
        owner.a_Hookshot.Aim();
    }

    
    public override void Exit(PlayerMovement owner)
    {
        owner.attackAction  -= AttackActionPerformed;
        owner.fireAction    -= FireActionPerformed;
        owner.dashAction    -= DashActionPerformed;
        owner.ultimateAction -= UltimateActionPerformed;

        owner.Anim.SetBool("AimingGrapple", false);
    }
    private void FireActionPerformed(PlayerMovement owner)
    {
        owner.FSM.SwitchState(owner.states.PullHookshotState);
    }

    private void AttackActionPerformed(PlayerMovement owner)
    {
        owner.FSM.SwitchState(owner.states.AttackState);
    }

    private void DashActionPerformed(PlayerMovement owner)
    {
        if (owner.a_Dash.IsActive)
            owner.FSM.SwitchState(owner.states.DashState);
    }

    private void UltimateActionPerformed(PlayerMovement owner)
    {
        if (owner.Ultimate.IsReady)
            owner.FSM.SwitchState(owner.states.UltimateState);
    }

}
