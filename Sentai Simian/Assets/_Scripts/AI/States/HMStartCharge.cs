using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

[System.Serializable]
public class HMStartCharge : State<NPC>
{
    [SerializeField]
    private float duration = 1f;
    [SerializeField]
    private AudioClip chargeWindUpAudio;
    private float timer = 0f;

    private State<NPC> sCharge;

    public void SetStates(State<NPC> _charge)
    {
        sCharge = _charge;
    }

    public override void Enter(NPC owner)
    {
        owner.IsPathfinding = false;
        timer = 0f;
        owner._AudioSource.PlayOneShot(chargeWindUpAudio);
    }

    public override void Execute(NPC owner)
    {
        owner.Decelerate(10f);

        Vector3 dirToTarget = (owner.Target.position - owner.transform.position).Flatten();
        owner.SetTargetRotation(Quaternion.LookRotation(dirToTarget, Vector3.up));

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            owner.FSM.SwitchState(sCharge);
        }
    }

    public override void Exit(NPC owner)
    {
    }
}
