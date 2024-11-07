using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

public class HeavyMeleeEnemy : NPC
{
    [SerializeField]
    private AudioClip crashClip;
    [SerializeField]
    private ParticleSystem chargeParticles;
    [SerializeField]
    private HMChase sChase;
    [SerializeField]
    private HMStartCharge sStartCharge;
    [SerializeField]
    private HMCharge sCharge;
    [SerializeField]
    private HMEndCharge sEndCharge;
    [SerializeField]
    private NPCStaggered sStaggered;

    protected override void Awake()
    {
        base.Awake();
        sChase.SetStates(sStartCharge);
        sStartCharge.SetStates(sCharge);
        sCharge.SetStates(sStaggered, sEndCharge);
        sEndCharge.SetStates(sStaggered, sChase);
        sStaggered.SetStates(sChase);
        FSM.SwitchState(sChase);
    }

    private void Start()
    {
        SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // dispatch message that we collided with something
        float collisionAngle = Vector3.Angle(collision.GetContact(0).normal, Vector3.up);
        if (collisionAngle >= 80f)
        {
            FSM.HandleMessage(new SS.StateMachine.Telegram(collision.transform, SS.StateMachine.Message.MSG_COLLISION));
        }
    }

    public void CrashIntoWall() {
        audioSource.PlayOneShot(crashClip);
    }

    public void StartCharge() {
        chargeParticles.Play();
    }

    public void EndCharge() {
        chargeParticles.Stop();
    }

    public override bool TryClaimToken() {
        if (actionToken != AITokenPool.INVALID_TOKEN_ID) {
            return true;
        }

        if (AIManager.Instance == null) {
            return true;
        }

        if (AIManager.Instance.heavyMeleeTokens.TryClaimToken(out var token)) {
            actionToken = token;
            return true;
        }

        return false;
    }

    public override void ReturnToken() {
        AIManager.Instance?.heavyMeleeTokens.ReturnToken(ref actionToken);
    }
}
