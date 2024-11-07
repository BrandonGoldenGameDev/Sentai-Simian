using SS.StateMachine;
using UnityEngine;

[System.Serializable]
public class PHookshotPull : State<PlayerMovement>
{
    [SerializeField]
    private float hookshotSpeed = 15f;
    [SerializeField]
    private float endGrappleDistance = 3f;

    public override void Enter(PlayerMovement owner)
    {
        owner.attackAction  += AttackActionPerformed;
        owner.dashAction    += DashActionPerformed;
        owner.a_Hookshot.Fire();
        owner.Anim.SetTrigger("OnGrappleFire");
    }

    public override void Execute(PlayerMovement owner)
    {
        if (owner.a_Hookshot.State == Hookshot.HookshotState.Grabbed)
        {
            Vector3 dirToTarget = owner.a_Hookshot.GrabbedObject.position - owner.transform.position;
            float distanceToTarget = dirToTarget.magnitude;

            if (distanceToTarget <= endGrappleDistance)
            {
                owner.FSM.SwitchState(owner.states.GroundState);
                owner.Anim.SetTrigger("OnExitGrapple");
            }

            dirToTarget.Normalize();
            owner.Velocity = dirToTarget * hookshotSpeed;
        }
        else
        {
            owner.Velocity = Vector3.zero;
        }

        if (owner.a_Hookshot.State == Hookshot.HookshotState.Waiting)
        {
            owner.FSM.SwitchState(owner.states.GroundState);
            owner.Anim.SetTrigger("OnExitGrapple");
        }
    }

    public override void Exit(PlayerMovement owner)
    {
        if (owner.a_Hookshot.State != Hookshot.HookshotState.Waiting)
        {
            owner.a_Hookshot.Return();
        }

        owner.attackAction  -= AttackActionPerformed;
        owner.dashAction    -= DashActionPerformed;
    }

    private void DashActionPerformed(PlayerMovement owner)
    {
        if (owner.a_Dash.IsActive)
            owner.FSM.SwitchState(owner.states.DashState);

        owner.Anim.SetTrigger("OnExitGrapple");
    }

    private void AttackActionPerformed(PlayerMovement owner)
    {
        owner.FSM.SwitchState(owner.states.DashAttackState);
        
        if (owner.a_Hookshot.State != Hookshot.HookshotState.Grabbed) {
            owner.Anim.SetTrigger("OnExitGrapple");
        }
    }
}
