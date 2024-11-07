using UnityEngine;

public class PlayerStatsTracker : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private float levelTimer = 0f;
    private int damageTaken = 0;
    private int successfulParries = 0;
    private int ultimatesCast = 0;

    private bool updateTimer = true;

    public float LevelTimer { get => levelTimer; set => levelTimer = value; }
    public int DamageTaken { get => damageTaken; set => damageTaken = value; }
    public int SuccessfulParries { get => successfulParries; set => successfulParries = value; }
    public int UltimatesCast { get => ultimatesCast; set => ultimatesCast = value; }

    private void Awake()
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        health.onPlayerDamaged += OnPlayerDamaged;
        player.GetComponent<Repulsor>().onSuccesfulParry += OnParry;
        player.GetComponent<UltimateAbility>().onUltimateCast += OnUltimateCast;
        var waveManager = FindObjectOfType<WaveManager>();
        if (waveManager)
            waveManager.onAllWavesCompleted += () => updateTimer = false;
    }

    private void Update()
    {
        if (updateTimer)
            levelTimer += Time.deltaTime;
    }

    private void OnPlayerDamaged(int damage)
    {

    }

    private void OnParry()
    {

    }

    private void OnUltimateCast() 
    {
        
    }
}
