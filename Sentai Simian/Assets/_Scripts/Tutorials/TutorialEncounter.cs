using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialEncounter : MonoBehaviour
{
    [SerializeField]
    private NPCSpawn spawn;
    [SerializeField]
    private Transform enemyToSpawn;
    [SerializeField]
    private Door door;

    [SerializeField]
    private UnityEvent onTutorialComplete;

    private int totalAlive = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (totalAlive > 0)
            return;
        
        spawn.StartSpawningNPC(enemyToSpawn, OnNPCSpawned);
    }

    private void OnNPCSpawned(GameObject npc)
    {
        totalAlive++;
        npc.GetComponent<NPC>().onNPCDeath += OnNPCKilled;
    }

    private void OnNPCKilled()
    {
        if (door != null)
        {
            door.Open();
        }

        onTutorialComplete?.Invoke();
    }
}
