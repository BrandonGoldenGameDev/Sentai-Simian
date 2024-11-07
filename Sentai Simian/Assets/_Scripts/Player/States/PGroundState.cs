using SS.StateMachine;
using UnityEngine;

[System.Serializable]
public class PGroundState : State<PlayerMovement>
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float acceleration = 12f;
    [SerializeField]
    private float deceleration = 40f;
    [SerializeField]
    private float gravity = 30f;

    public override void Enter(PlayerMovement owner)
    {
        owner.dashAction        += DashActionPerformed;
        owner.repulsorAction    += RepulsorActionPerformed;
        owner.aimAction         += AimActionPerformed;
        owner.attackAction      += AttackActionPerformed;
        owner.ultimateAction    += UltimateActionPerformed;
    }

    public override void Execute(PlayerMovement owner)
    {
        Vector3 wishDir = owner.GetWishDir();
        float wishSpeed = wishDir.magnitude * speed;
        wishDir.Normalize();

        owner.Accelerate(wishDir, wishSpeed, acceleration);

        if (wishSpeed == 0 || Vector3.Dot(wishDir, owner.Velocity) < 0)
            owner.Decelerate(deceleration);

        if (!owner.IsGrounded)
        {
            Vector3 newVel = owner.Velocity;
            newVel.y -= gravity * Time.deltaTime;
            owner.Velocity = newVel;
        }

        if (wishSpeed > 0f)
        {
            owner.GetLastMovedDirection();
            owner.SetRotationTarget(wishDir);
        }
    }

    public override void Exit(PlayerMovement owner)
    {
        owner.dashAction        -= DashActionPerformed;
        owner.repulsorAction    -= RepulsorActionPerformed;
        owner.aimAction         -= AimActionPerformed;
        owner.attackAction      -= AttackActionPerformed;
        owner.ultimateAction    -= UltimateActionPerformed;
    }

    private void AimActionPerformed(PlayerMovement owner)
    {
        if (owner.a_Hookshot.IsActive)
            owner.FSM.SwitchState(owner.states.AimHookshotState);
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

    private void RepulsorActionPerformed(PlayerMovement owner)
    {
        if (owner.a_Repulsor.IsActive)
            owner.FSM.SwitchState(owner.states.RepulsorState);
    }

    private void UltimateActionPerformed(PlayerMovement owner)
    {
        if (owner.Ultimate.IsReady)
            owner.FSM.SwitchState(owner.states.UltimateState);
    }
}
