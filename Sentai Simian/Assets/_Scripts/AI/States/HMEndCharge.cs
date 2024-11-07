using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

[System.Serializable]
public class HMEndCharge : State<NPC>
{
    [SerializeField]
    private float stopDuration;
    [SerializeField]
    private float impactForce;
    private float timer = 0f;

    private State<NPC> chaseState;
    private State<NPC> staggeredState;

    [SerializeField]
    private float deceleration = 40f;

    public void SetStates(State<NPC> _staggered, State<NPC> _chase)
    {
        staggeredState = _staggered;
        chaseState = _chase;
    }

    public override void Enter(NPC owner)
    {
        timer = 0f;
    }

    public override void Execute(NPC owner)
    {
        owner.Decelerate(deceleration);

        timer += Time.deltaTime;
        if (timer >= stopDuration)
        {
            owner.FSM.SwitchState(chaseState);
        }
    }

    public override void Exit(NPC owner)
    {
    }

    public override bool HandleMessage(NPC owner, Telegram telegram)
    {
        switch (telegram.message)
        {
            case Message.MSG_COLLISION:
                HandleCollision(owner, telegram.sender);
                return true;
            default:
                break;
        }

        return false;
    }

    private void HandleCollision(NPC owner, Transform collisionTransform)
    {
        if (collisionTransform.CompareTag("Player") || collisionTransform.CompareTag("Enemy"))
            return;

        owner.FSM.SwitchState(staggeredState);
        owner.GenerateImpulseWithVelocity(owner.transform.forward * impactForce);

        if (owner is HeavyMeleeEnemy hme) {
            hme.CrashIntoWall();
        }
    }
}
