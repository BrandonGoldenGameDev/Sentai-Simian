using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField, Tooltip("Determines how many waves have to be completed before a new zone opens."), Min(1)]
    private int zoneActivationWaveInterval;

    private List<Zone> zones;
    private List<Zone> activeZones;

    private WaveManager waveManager;

    private void Start()
    {
        zones = new List<Zone>(FindObjectsOfType<Zone>());
        activeZones = new List<Zone>();

        foreach (var zone in zones)
        {
            zone.onZoneActivated += (z) => activeZones.Add(z);
        }

        waveManager = GetComponent<WaveManager>();
        waveManager.onWaveCompleted += OnWaveCompleted;
        SetStartingZone();
    }

    private void SetStartingZone()
    {
        if (zones == null)
        {
            Debug.LogError("ZONEMANAGER: No zones have been assigned.");
            return;
        }

        Zone startingZone = zones.Find(zone => zone.IsStart);

        if (startingZone == null)
        {
            Debug.LogError("ZONEMANAGER: No zone is marked as the starting zone.");
            return;
        }

        startingZone.Enable();
    }

    private void OnWaveCompleted()
    {
        if (waveManager.CurrentWaveIndex % zoneActivationWaveInterval != 0)
            return;

        List<Door> unopenedDoors = new List<Door>();
        foreach (var zone in activeZones)
        {
            unopenedDoors.AddRange(zone.UnopenedDoors);
        }

        if (unopenedDoors.Count > 0)
        {
            Door door = unopenedDoors[Random.Range(0, unopenedDoors.Count)];
            door.Open();
        }
    }

    public List<NPCSpawn> GetActiveSpawns()
    {
        var activeSpawns = new List<NPCSpawn>();
        foreach (var activeZone in activeZones)
        {
            activeSpawns.AddRange(activeZone.GetActiveSpawns());
        }

        return activeSpawns;
    }
}
