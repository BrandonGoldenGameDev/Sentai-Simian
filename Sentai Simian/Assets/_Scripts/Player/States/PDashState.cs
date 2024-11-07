using SS.StateMachine;
using UnityEngine;

[System.Serializable]
public class PDashState : State<PlayerMovement>
{
    [SerializeField]
    private float dashSpeed = 15f;
    [SerializeField]
    private float dashDuration = 0.2f;
    private float dashTimer = 0f;

    private Vector3 dashDirection;
    private State<PlayerMovement> stateToTransitionTo;


    public override void Enter(PlayerMovement owner)
    {
        stateToTransitionTo = owner.states.GroundState;
        dashDirection = owner.GetLastMovedDirection();
        owner.a_Dash.Use();
        owner.SetRotation(dashDirection);
        owner.Health.InvincibilityOn();
        owner.Dash();
        owner.Anim.SetBool("IsDashing", true);

        owner.attackAction += AttackActionPerformed;
    }

    public override void Execute(PlayerMovement owner)
    {
        owner.Velocity = owner.AlignVectorWithGround(dashDirection) * dashSpeed;

        dashTimer += Time.deltaTime;
        if (dashTimer >= dashDuration)
        {
            owner.FSM.SwitchState(stateToTransitionTo);
        }
    }

    public override void Exit(PlayerMovement owner)
    {
        dashTimer = 0f;
        owner.Health.InvincibilityOff();
        owner.a_Dash.EndDash();
        owner.Anim.SetBool("IsDashing", false);

        owner.attackAction -= AttackActionPerformed;
    }

    private void AttackActionPerformed(PlayerMovement owner)
    {
        stateToTransitionTo = owner.states.DashAttackState;
    }
}
