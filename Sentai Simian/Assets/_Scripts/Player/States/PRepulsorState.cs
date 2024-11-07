using SS.StateMachine;
using UnityEngine;

[System.Serializable]
public class PRepulsorState : State<PlayerMovement>
{
    [SerializeField]
    private float useDuration;
    [SerializeField]
    private float castDuration;
    [SerializeField]
    private float gravity;
    private float useTimer;

    public override void Enter(PlayerMovement owner)
    {
        owner.attackAction += AttackActionPerformed;
        owner.aimAction += AimActionPerformed;
        owner.dashAction += DashActionPerformed;
        owner.ultimateAction += UltimateActionPerformed;
        owner.Anim.SetBool("UsingRepulsor", true);
        owner.SetRotation(owner.GetLastMovedDirection());
        owner.Health.RepulsorActive = true;
    }

    public override void Execute(PlayerMovement owner)
    {
        owner.Velocity = new Vector3(0f, owner.Velocity.y, 0f);
        owner.ApplyGravity(gravity);

        useTimer += Time.deltaTime;
        if (owner.a_Repulsor.IsActive && useTimer >= castDuration)
        {
            owner.a_Repulsor.TriggerWeakRepulsor();
        }

        if (useTimer >= useDuration)
        {
            owner.FSM.SwitchState(owner.states.GroundState);
        }
    }

    public override void Exit(PlayerMovement owner)
    {
        useTimer = 0f;
        owner.attackAction -= AttackActionPerformed;
        owner.aimAction -= AimActionPerformed;
        owner.dashAction -= DashActionPerformed;
        owner.ultimateAction -= UltimateActionPerformed;
        owner.Anim.SetBool("UsingRepulsor", false);
    }

    public override bool HandleMessage(PlayerMovement owner, Telegram telegram)
    {
        switch (telegram.message)
        {
            case Message.MSG_DAMAGED:
                if (useTimer <= castDuration)
                {
                    owner.Anim.SetTrigger("OnParry");
                    owner.a_Repulsor.TriggerStaggerRepulsor();
                }

                return true;

            case Message.MSG_DAMAGED_LASER:
                if (useTimer <= castDuration)
                {
                    owner.Anim.SetTrigger("OnParry");
                    owner.a_Repulsor.ReflectLaser(telegram.sender);
                }

                return true;
        }

        return false;
    }

    private void AttackActionPerformed(PlayerMovement owner)
    {
        owner.FSM.SwitchState(owner.states.AttackState);
    }

    private void AimActionPerformed(PlayerMovement owner)
    {
        if (owner.a_Hookshot.IsActive)
            owner.FSM.SwitchState(owner.states.AimHookshotState);
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
