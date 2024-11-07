using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spawner
{
    private ZoneManager zoneManager;
    [SerializeField]
    private Transform enemyPrefab;
    [SerializeField]
    private bool isInfinite = false;
    [SerializeField]
    private int maxToSpawn;
    [SerializeField]
    private int maxAliveAtOnce;
    [SerializeField]
    private float spawnRate = 1f;
    private float spawnTimer = 0f;
    private int totalSpawned = 0;
    private int totalAlive = 0;
    private int totalKilled = 0;

    [Header("Completion Settings"), SerializeField]
    private bool necessaryForCompletion = true;
    [SerializeField, Range(0f, 1f)]
    private float killsToComplete = 0;

    public int TotalKilled => totalKilled;
    public int KillsToComplete => Mathf.FloorToInt(killsToComplete * maxToSpawn);
    public bool NecessaryForCompletion => necessaryForCompletion;

    private bool HasMaxAliveEnemies => totalAlive >= maxAliveAtOnce;
    private bool HasSpawnedAllEnemies => totalSpawned >= maxToSpawn;
    private bool CanSpawnEnemy => !HasMaxAliveEnemies && (!HasSpawnedAllEnemies || isInfinite);

    public bool IsCompleted()
    {
        if (!necessaryForCompletion)
            return true;

        float necessaryKills = KillsToComplete;

        return !necessaryForCompletion || totalKilled >= necessaryKills;
    }

    public void Init(ZoneManager _zoneManager) => zoneManager = _zoneManager;

    public void Update()
    {
        if (spawnTimer >= spawnRate)
        {
            if (CanSpawnEnemy)
            {
                SpawnEnemy();
            }
        }
        else
        {
            spawnTimer += Time.deltaTime;
        }
    }

    private void SpawnEnemy()
    {
        spawnTimer = 0f;
        var spawnPoints = zoneManager.GetActiveSpawns();
        if (spawnPoints.Count == 0)
            return;

        NPCSpawn spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
        spawnPoint.StartSpawningNPC(enemyPrefab, OnEnemySpawned);

        totalAlive++;
        totalSpawned++;
    }

    private void OnEnemySpawned(GameObject enemy)
    {
        NPC npc = enemy.GetComponent<NPC>();
        npc.onNPCDeath += OnEnemyDeath;
    }

    private void OnEnemyDeath()
    {
        totalAlive--;
        totalKilled++;
    }
}

[System.Serializable]
public class Wave
{
    public Action onStageCompelted;
    public Action onWaveCompleted;

    [SerializeField]
    private List<Spawner> spawners;
    [SerializeField]
    private List<Wave> stages;
    private int currentStage = 0;

    public bool IsCompleted => StagesAreCompleted() && SpawnersAreCompleted();
    public int MaxStages => stages.Count;
    public int CurrentStage => currentStage;

    public void Init(ZoneManager zoneManager)
    {
        foreach (var stage in stages)
        {
            stage.Init(zoneManager);
        }

        foreach (var spawner in spawners)
        {
            spawner.Init(zoneManager);
        }
    }

    public void Update()
    {
        foreach (var spawner in spawners)
        {
            spawner.Update();
        }

        if (stages == null || currentStage >= stages.Count)
            return;

        stages[currentStage].Update();
        if (stages[currentStage].IsCompleted)
        {
            currentStage++;
            onStageCompelted?.Invoke();
            Debug.Log("Stage completed!");

            if (currentStage >= stages.Count)
            {
                Debug.Log("All stages completed!");
                onWaveCompleted?.Invoke();
            }
        }
    }

    private bool SpawnersAreCompleted()
    {
        if (spawners == null)
            return true;

        for (int i = 0; i < spawners.Count; i++)
        {
            if (!spawners[i].IsCompleted())
                return false;
        }

        return true;
    }

    private bool StagesAreCompleted() => stages == null || currentStage >= stages.Count;

    public float GetPercentCompleted()
    {
        int totalKills = 0;
        int totalRequiredKills = 0;
        for (int i = 0; i < spawners.Count; i++)
        {
            if (!spawners[i].NecessaryForCompletion)
                continue;

            totalKills += spawners[i].TotalKilled;
            totalRequiredKills += spawners[i].KillsToComplete;
        }

        if (totalRequiredKills <= 0)
            return 1f;

        return Mathf.Clamp01(totalKills / (float)totalRequiredKills);
    }
}

public class WaveManager : MonoBehaviour
{
    public Action onWaveCompleted;
    public Action onAllWavesCompleted;

    [SerializeField]
    private float timeBetweenWaves = 0.5f;
    [SerializeField]
    private List<Wave> waves;
    private int currentWave = 0;
    private ZoneManager zoneManager;
    private AudioSource waveCompletedSource;
    private bool transitioningWaves = false;

    public Wave CurrentWave => waves[currentWave];
    public int CurrentWaveIndex => currentWave;

    private void Start()
    {
        zoneManager = GetComponent<ZoneManager>();
        waveCompletedSource = GetComponent<AudioSource>();

        foreach (var wave in waves)
        {
            wave.Init(zoneManager);
        }
    }

    private void Update()
    {
        if (waves == null || currentWave >= waves.Count || transitioningWaves)
            return;

        waves[currentWave].Update();
        if (waves[currentWave].IsCompleted)
        {
            StartCoroutine(TransitionWave());
        }
    }

    private IEnumerator TransitionWave() {
        transitioningWaves = true;
        currentWave++;

        bool lastWave = currentWave >= waves.Count;


        if (lastWave) {
            Debug.Log("All waves completed!");
            onAllWavesCompleted?.Invoke();
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        if (!lastWave) {
            waveCompletedSource.Play();
            onWaveCompleted?.Invoke();
        }

        transitioningWaves = false;
    }
}
