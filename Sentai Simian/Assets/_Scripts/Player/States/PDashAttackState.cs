using SS.StateMachine;
using UnityEngine;

[System.Serializable]
public class PDashAttackState : State<PlayerMovement>
{
    [SerializeField]
    private AttackData dashAttack;
    private float timer = 0f;
    [SerializeField]
    private float deceleration;

    private State<PlayerMovement> stateToTransitionTo;

    public override void Enter(PlayerMovement owner)
    {
        stateToTransitionTo = owner.states.GroundState;
        owner.attackAction      += AttackActionPerformed;
        owner.dashAction        += DashActionPerformed;
        owner.repulsorAction    += RepulsorActionPerformed;
        owner.ultimateAction    += UltimateActionPerformed;

        Attack(owner, owner.GetLastMovedDirection());
    }

    public override void Execute(PlayerMovement owner)
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            owner.FSM.SwitchState(stateToTransitionTo);
        }
        else
        {
            if (stateToTransitionTo != owner.states.GroundState && timer <= dashAttack.attackComboTime)
            {
                owner.FSM.SwitchState(stateToTransitionTo);
            }
        }

        owner.Decelerate(deceleration);
    }

    public override void Exit(PlayerMovement owner)
    {
        owner.attackAction      -= AttackActionPerformed;
        owner.dashAction        -= DashActionPerformed;
        owner.repulsorAction    -= RepulsorActionPerformed;
        owner.ultimateAction    -= UltimateActionPerformed;

        owner.Anim.SetTrigger("ExitAttack");
    }

    private void Attack(PlayerMovement owner, Vector3 dir)
    {
        timer = dashAttack.attackDuration;
        owner.Anim.SetTrigger("OnAttack");
        owner.Velocity = owner.AlignVectorWithGround(dir) * dashAttack.attackLungeForce;
        owner.SetRotation(dir);
        owner.a_Attack.Attack(dir, dashAttack);
    }

    private void DashActionPerformed(PlayerMovement owner)
    {
        if (owner.a_Dash.IsActive)
            stateToTransitionTo = owner.states.DashState;
    }

    private void AttackActionPerformed(PlayerMovement owner)
    {
        stateToTransitionTo = owner.states.AttackState;
    }

    private void RepulsorActionPerformed(PlayerMovement owner)
    {
        if (owner.a_Repulsor.IsActive)
            stateToTransitionTo = owner.states.RepulsorState;
    }

    private void UltimateActionPerformed(PlayerMovement owner)
    {
        if (owner.Ultimate.IsReady)
            stateToTransitionTo = owner.states.UltimateState;
    }
}
