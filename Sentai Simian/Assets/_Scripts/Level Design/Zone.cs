using System.Collections.Generic;
using UnityEngine;
using System;

public class Zone : MonoBehaviour
{
    public Action<Zone> onZoneActivated;

    [SerializeField]
    private bool isStart;
    private bool isActive;
    [SerializeField]
    private List<Door> connectedDoors;
    [SerializeField]
    private List<NPCSpawn> spawnPoints;

    public bool IsStart => isStart;
    public bool IsActive => isActive;

    public List<Door> UnopenedDoors => connectedDoors.FindAll(door => !door.IsOpen);
    public List<NPCSpawn> SpawnPoints => spawnPoints;

    private void Start()
    {
        foreach (var door in connectedDoors)
        {
            door.AddConnectedZone(this);
        }
    }

    public void Enable()
    {
        isActive = true;
        onZoneActivated?.Invoke(this);
        foreach (var spawnPoint in spawnPoints)
        {
            spawnPoint.IsActive = true;
        }
    }

    public List<NPCSpawn> GetActiveSpawns()
    {
        var spawns = new List<NPCSpawn>();
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.IsActive && spawnPoint.gameObject.activeSelf)
                spawns.Add(spawnPoint);
        }

        return spawns;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.gameObject.activeSelf)
                Debug.DrawLine(transform.position, spawnPoint.transform.position, Color.red);
        }

        foreach (var door in connectedDoors)
        {
            Debug.DrawLine(transform.position, door.transform.position, Color.blue);
        }
    }
}
