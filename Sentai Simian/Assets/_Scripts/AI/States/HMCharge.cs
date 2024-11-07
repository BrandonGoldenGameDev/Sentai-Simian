using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

[System.Serializable]
public class HMCharge : State<NPC>
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float impactForce;

    private State<NPC> endChargeState;
    private State<NPC> staggeredState;

    public void SetStates(State<NPC> _staggered, State<NPC> _endCharge)
    {
        staggeredState = _staggered;
        endChargeState = _endCharge;
    }

    public override void Enter(NPC owner)
    {
        if (owner is HeavyMeleeEnemy hme) {
            hme.StartCharge();
        }
    }

    public override void Execute(NPC owner)
    {
        Vector3 facingDir = owner.transform.forward;
        Vector3 dirToTarget = (owner.Target.position - owner.transform.position).Flatten();
        float dot = Vector3.Dot(facingDir, dirToTarget);

        if (dot >= 0)
        {
            facingDir = Vector3.RotateTowards(facingDir, dirToTarget, rotationSpeed * Time.deltaTime, 0.0f);
            owner.SetTargetRotation(Quaternion.LookRotation(facingDir, Vector3.up));
            owner.SetVelocity(new Vector3(facingDir.x * speed, owner.Velocity.y, facingDir.z * speed));
        }
        else
        {
            owner.FSM.SwitchState(endChargeState);
        }
    }

    public override void Exit(NPC owner)
    {
        owner.ReturnToken();
        if (owner is HeavyMeleeEnemy hme) {
            hme.EndCharge();
        }
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
        if (collisionTransform.CompareTag("Player"))
        {
            PlayerHealth health = collisionTransform.GetComponent<PlayerHealth>();
            health.TakeDamage(damage);
            owner.FSM.SwitchState(endChargeState);
        }
        else if (collisionTransform.CompareTag("Enemy"))
        {
            var damageable = collisionTransform.GetComponent<IDamageable>();
            damageable.TakeDamage(damage, null, null);
        }
        else
        {
            owner.GenerateImpulseWithVelocity(owner.transform.forward * impactForce);
            owner.FSM.SwitchState(staggeredState);

            if (owner is HeavyMeleeEnemy hme) {
                hme.CrashIntoWall();
            }
        }    
    }
}
