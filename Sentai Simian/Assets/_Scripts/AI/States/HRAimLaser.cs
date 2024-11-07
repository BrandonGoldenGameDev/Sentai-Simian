using System.Collections;
using UnityEngine;
using SS.StateMachine;

[System.Serializable]
public class HRAimLaser : State<NPC>
{
    [SerializeField]
    private float chargeUpTime = 3f;
    [SerializeField]
    private float deceleration = 15f;
    private float timer = 0f;

    [SerializeField]
    private float losLostDuration;
    private float losTimer;

    private State<NPC> attackState;
    private State<NPC> staggeredState;

    public void SetStates(State<NPC> _attack, State<NPC> _staggered)
    {
        attackState = _attack;
        staggeredState = _staggered;
    }

    public override void Enter(NPC owner)
    {
        HeavyRangedEnemy hre = owner as HeavyRangedEnemy;
        if (hre == null)
        {
            Debug.LogError($"{ToString()}: the owner of this state is not of the right type.");
            owner.FSM.RevertToPreviousState();
            return;
        }

        hre.IsPathfinding = false;
        hre.AimLaser = true;
        hre.StartAim(chargeUpTime);

        timer = 0f;
        losTimer = 0f;
    }

    public override void Execute(NPC owner)
    {
        HeavyRangedEnemy hre = owner as HeavyRangedEnemy;

        Vector3 dirToTarget = hre.Target.position - hre.transform.position;
        Vector3 lookDirection = dirToTarget.Flatten();

        hre.SetTargetRotation(Quaternion.LookRotation(lookDirection, Vector3.up));

        timer += Time.deltaTime;
        if (timer >= chargeUpTime)
        {
            owner.FSM.SwitchState(attackState);
        }
        else
        {
            if (!hre.HasLineOfSightOfTarget())
            {
                losTimer += Time.deltaTime;
                if (losTimer >= losLostDuration)
                {
                    owner.FSM.SwitchState(attackState);
                }
            }
            else
            {
                losTimer = 0f;
            }
        }

        owner.Decelerate(deceleration);
    }

    public override void Exit(NPC owner)
    {
        HeavyRangedEnemy hre = owner as HeavyRangedEnemy;

        owner.ReturnToken();
        hre.AimLaser = false;
    }

    public override bool HandleMessage(NPC owner, Telegram telegram)
    {
        if (telegram.message == Message.MSG_DAMAGED) {
            owner.FSM.SwitchState(staggeredState);

            if (owner is HeavyRangedEnemy hre) {
                hre.EndAim();
            }
        }

        return base.HandleMessage(owner, telegram);
    }
}