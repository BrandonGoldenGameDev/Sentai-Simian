using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

[System.Serializable]
public class PUltimateState : State<PlayerMovement>
{
    [SerializeField]
    private float stateDuration;
    [SerializeField]
    private float castDuration;
    private float timer;

    public override void Enter(PlayerMovement owner)
    {
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        timer = 0f;
        owner.Anim.SetTrigger("Cast Ultimate");
        owner.SetRotation(owner.GetLastMovedDirection());
        owner.Ultimate.StartCast();
    }

    public override void Execute(PlayerMovement owner)
    {
        timer += Time.deltaTime;
        owner.Decelerate(40f);
        owner.ApplyGravity(30f);
        owner.SetRotation(owner.GetLastMovedDirection());

        if (timer >= castDuration && owner.Ultimate.IsReady)
        {
            owner.Ultimate.Use(owner.GetLastMovedDirection());
            Time.timeScale = 1f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

        if (timer >= stateDuration)
        {
            owner.FSM.SwitchState(owner.states.GroundState);
        }
    }
}
