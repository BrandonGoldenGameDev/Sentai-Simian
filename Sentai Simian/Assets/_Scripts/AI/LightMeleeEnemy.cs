using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;

public class LightMeleeEnemy : NPC
{
    [SerializeField]
    private float attackVolume = 1f;
    [SerializeField]
    private AudioClip attackClip;
    [SerializeField]
    private NPCChasePlayer sChase;
    [SerializeField]
    private LMAttack sAttack;
    [SerializeField]
    private NPCStaggered sStaggered;
    private NPCGlobal sGlobal;

    protected override void Awake()
    {
        base.Awake();

        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        sGlobal = new NPCGlobal();
        sChase.SetStates(sAttack);
        sAttack.SetStates(sChase);
        sStaggered.SetStates(sChase);
        sGlobal.SetStates(sStaggered);
        FSM.SwitchState(sChase);
        FSM.SetGlobalState(sGlobal);
    }

    private void Start()
    {
        SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
    }

    public void Attack(float range)
    {
        audioSource.PlayOneShot(attackClip, attackVolume);
        Vector3 dirToTarget = target.position - transform.position;
        float sqrDistToTarget = dirToTarget.sqrMagnitude;

        SetTargetRotation(Quaternion.LookRotation(dirToTarget.Flatten(), Vector3.up));

        if (sqrDistToTarget < range * range)
        {
            PlayerHealth pHealth = Target.GetComponent<PlayerHealth>();
            pHealth?.TakeDamage(20);
            pHealth?.Player.FSM.HandleMessage(new Telegram(transform, Message.MSG_DAMAGED));
        }
    }

    public override bool TryClaimToken() {
        if (actionToken != AITokenPool.INVALID_TOKEN_ID) {
            return true;
        }

        if (AIManager.Instance == null) {
            return true;
        }

        if (AIManager.Instance.lightMeleeTokens.TryClaimToken(out var token)) {
            actionToken = token;
            return true;
        }

        return false;
    }

    public override void ReturnToken() {
        AIManager.Instance?.lightMeleeTokens.ReturnToken(ref actionToken);
    }
}
