using SS.StateMachine;
using UnityEngine;

[System.Serializable]
public class PlayerStates
{
    [SerializeField]
    private PGroundState sGround;
    [SerializeField]
    private PDashState sDash;
    [SerializeField]
    private PRepulsorState sRepulsor;
    [SerializeField]
    private PAttackState sAttack;
    [SerializeField]
    private PDashAttackState sDashAttack;
    [SerializeField]
    private PHookshotPull sPullHookshot;
    [SerializeField]
    private PHookshotAim sAimHookshot;
    [SerializeField]
    private PGlobal sGlobal;
    [SerializeField]
    private PDead sDead;
    [SerializeField]
    private PUltimateState sUltimate;

    public PGroundState GroundState => sGround;
    public PDashState DashState => sDash;
    public PRepulsorState RepulsorState => sRepulsor;
    public PAttackState AttackState => sAttack;
    public PDashAttackState DashAttackState => sDashAttack;
    public PHookshotPull PullHookshotState => sPullHookshot;
    public PHookshotAim AimHookshotState => sAimHookshot;
    public PGlobal GlobalState => sGlobal;
    public PDead DeadState => sDead;
    public PUltimateState UltimateState => sUltimate;

}
