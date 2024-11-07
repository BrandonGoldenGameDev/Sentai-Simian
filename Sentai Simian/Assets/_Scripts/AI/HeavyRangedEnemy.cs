using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.StateMachine;
using UnityEditor;

public class HeavyRangedEnemy : NPC
{
    [SerializeField]
    private LineRenderer laserLine;
    [SerializeField]
    private Transform laserOrigin;
    [SerializeField]
    private ParticleSystem laserParticles;
    [SerializeField]
    private Transform laserImpactParticles;
    [SerializeField]
    private int laserDamage;
    [SerializeField]
    private float laserWidth = 0.2f;
    private int laserTweenId;

    [SerializeField]
    private NPCStayAtRange sStayAtRange;
    [SerializeField]
    private HRAimLaser sHitscan;
    [SerializeField]
    private HRFireLaser sLaser;
    [SerializeField]
    private NPCStaggered sStaggered;
    private bool aimLaser = false;

    public LineRenderer Laser => laserLine;
    public bool AimLaser 
    {
        get => aimLaser;
        set
        {
            aimLaser = value;
            laserLine.gameObject.SetActive(value);
            laserParticles.gameObject.SetActive(value);
            if (value)
                laserParticles.Play();
        }
    }

    protected override void Awake()
    {
        base.Awake();

        sStayAtRange.SetStates(sHitscan);
        sHitscan.SetStates(sLaser, sStaggered);
        sLaser.SetStates(sStayAtRange);
        sStaggered.SetStates(sStayAtRange);
        FSM.SwitchState(sStayAtRange);
    }

    private void Start()
    {
        SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
    }

    protected override void Update()
    {
        base.Update();

        if (aimLaser)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Vector3 startPos = laserOrigin.position;
            Vector3 endPos;

            if (HasLineOfSightOfTarget(out RaycastHit hit))
            {
                endPos = target.position;
            }
            else
            {

                if (hit.collider != null)
                    endPos = hit.point;
                else
                    endPos = transform.position;
            }

            laserLine.SetPosition(0, startPos);
            laserLine.SetPosition(1, endPos);
            laserParticles.transform.position = hit.point;
            laserParticles.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
    }

    public void FireLaserAtTarget()
    {
        animator.SetTrigger("OnFireLaser");
        Vector3 dirToTarget = Target.position - transform.position;
        dirToTarget.Normalize();

        if (HasLineOfSightOfTarget(out RaycastHit hit))
        {
            var player = hit.transform.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(laserDamage);
                player.Player.FSM.HandleMessage(new Telegram(transform, Message.MSG_DAMAGED));
                player.Player.FSM.HandleMessage(new Telegram(transform, Message.MSG_DAMAGED_LASER));
            }
        }

        if (hit.collider != null)
        {
            ImpulseSource.GenerateImpulseAt(hit.point, dirToTarget);
            GameObject impactParticles = Instantiate(laserImpactParticles, hit.point, Quaternion.LookRotation(hit.normal)).gameObject;
            Destroy(impactParticles, 3f);
        }

        audioSource.Stop();
    }

    public bool HasLineOfSightOfTarget() => HasLineOfSightOfTarget(out RaycastHit hit);
    public bool HasLineOfSightOfTarget(out RaycastHit hit)
    {
        Vector3 dirToTarget = Target.position - transform.position;

        if (Physics.Raycast(transform.position, dirToTarget.normalized, out hit, 100f, LayerMask.GetMask("Player Hitbox", "Default")))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    public void StartAim(float aimDuration)
    {
        animator.SetTrigger("OnAimLaser");
        animator.SetBool("IsAttacking", true);
        audioSource.Play();
        LeanTween.cancel(laserTweenId);
        laserTweenId = LeanTween.value(0f, laserWidth, aimDuration)
             .setEaseOutSine()
             .setOnUpdate(SetLaserWidth)
             .id;
    }

    public void EndAim()
    {
        animator.SetBool("IsAttacking", false);
        audioSource.Stop();
    }

    public void SetLaserWidth(float width)
    {
        laserLine.startWidth = width;
        laserLine.endWidth = width;
    }

    private void OnDestroy()
    {
        LeanTween.cancel(laserTweenId);
    }

    public override bool TryClaimToken() {
        if (actionToken != AITokenPool.INVALID_TOKEN_ID) {
            return true;
        }

        if (AIManager.Instance == null) {
            return true;
        }

        if (AIManager.Instance.heavyRangedTokens.TryClaimToken(out var token)) {
            actionToken = token;
            return true;
        }

        return false;
    }

    public override void ReturnToken() {
        AIManager.Instance?.heavyRangedTokens.ReturnToken(ref actionToken);
    }
}
