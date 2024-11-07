using SS.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PAttackState : State<PlayerMovement>
{
    [SerializeField]
    private AttackData[] attacks;
    [SerializeField]
    private float deceleration;
    [SerializeField]
    private float gravity = 30f;
    private float comboTime = 0f;
    private float timer = 0f;

    private int currentAttack = 0;
    private bool bufferedAttack = false;

    public override void Enter(PlayerMovement owner)
    {
        owner.dashAction += DashActionPerformed;
        owner.repulsorAction += RepulsorActionPerformed;
        owner.aimAction += AimActionPerformed;
        owner.attackAction += AttackActionPerformed;
        owner.ultimateAction += UltimateActionPerformed;
        bufferedAttack = true;
    }

    public override void Execute(PlayerMovement owner)
    {
        timer -= Time.deltaTime;
        if (timer <= comboTime)
        {
            if (bufferedAttack && currentAttack < attacks.Length)
            {
                Attack(owner, owner.GetLastMovedDirection());
                owner.Anim.SetTrigger("OnAttack");
            }
        }

        if (timer <= 0)
        {
            owner.FSM.SwitchState(owner.states.GroundState);
        }

        owner.Decelerate(deceleration);
        owner.ApplyGravity(gravity);
    }

    public override void Exit(PlayerMovement owner)
    {
        owner.dashAction -= DashActionPerformed;
        owner.repulsorAction -= RepulsorActionPerformed;
        owner.aimAction -= AimActionPerformed;
        owner.attackAction -= AttackActionPerformed;
        owner.ultimateAction -= UltimateActionPerformed;
        owner.Anim.SetTrigger("ExitAttack");
        currentAttack = 0;
    }

    private void Attack(PlayerMovement owner, Vector3 attackDirection)
    {
        timer = attacks[currentAttack].attackDuration;
        comboTime = attacks[currentAttack].attackComboTime;

        owner.Velocity = owner.AlignVectorWithGround(attackDirection) * attacks[currentAttack].attackLungeForce;
        owner.SetRotationTarget(attackDirection);
        owner.a_Attack.Attack(attackDirection, attacks[currentAttack]);

        currentAttack++;
        bufferedAttack = false;
    }

    private void AttackActionPerformed(PlayerMovement owner)
    {
        if (currentAttack < attacks.Length)
            bufferedAttack = true;
    }

    private void DashActionPerformed(PlayerMovement owner)
    {
        if (owner.a_Dash.IsActive)
            owner.FSM.SwitchState(owner.states.DashState);
    }

    private void AimActionPerformed(PlayerMovement owner)
    {
        if (owner.a_Hookshot.IsActive)
            owner.FSM.SwitchState(owner.states.AimHookshotState);
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
