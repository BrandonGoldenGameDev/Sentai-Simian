using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRangedEnemy : NPC
{
    [SerializeField]
    private AudioClip fireClip;
    [SerializeField]
    private NPCStayAtRange sStayAtRange;
    [SerializeField]
    private NPCShootProjectile sShoot;
    [SerializeField]
    private NPCStaggered sStaggered;
    private NPCGlobal sGlobal = new NPCGlobal();

    private AudioSource source;

    protected override void Awake()
    {
        base.Awake();
        sShoot.SetStates(sStayAtRange);
        sStayAtRange.SetStates(sShoot);
        sStaggered.SetStates(sStayAtRange);
        sGlobal.SetStates(sStaggered);
        FSM.SwitchState(sStayAtRange);
        FSM.SetGlobalState(sGlobal);

        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
    }

    protected override void Update()
    {
        base.Update();
    }

    public void FireProjectile(Transform projectile, Vector3 direction)
    {
        Instantiate(projectile, transform.position, Quaternion.LookRotation(direction));
        source.PlayOneShot(fireClip);
    }

    public override bool TryClaimToken() {
        if (actionToken != AITokenPool.INVALID_TOKEN_ID) {
            return true;
        }

        if (AIManager.Instance == null) {
            return true;
        }

        if (AIManager.Instance.lightRangedTokens.TryClaimToken(out var token)) {
            actionToken = token;
            return true;
        }

        return false;
    }

    public override void ReturnToken() {
        AIManager.Instance?.lightRangedTokens.ReturnToken(ref actionToken);
    }
}
