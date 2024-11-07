using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawn : MonoBehaviour
{
    private ParticleSystem spawnParticles;
    private AudioSource source;
    private bool isActive;
    public bool IsActive
    {
        get { return isActive && !isSpawningNPC; }
        set { isActive = value; }
    }

    private bool isSpawningNPC = false;

    private void Start()
    {
        spawnParticles = GetComponentInChildren<ParticleSystem>();
        source = GetComponent<AudioSource>();
    }

    public void StartSpawningNPC(Transform npc, Action<GameObject> onNPCSpawned)
    {
        source.Play();
        StartCoroutine(SpawnNPC(npc, onNPCSpawned));
    }

    private IEnumerator SpawnNPC(Transform npc, Action<GameObject> onNPCSpawned)
    {
        if (spawnParticles != null)
        {
            isSpawningNPC = true;
            spawnParticles.Play(true);

            yield return new WaitForSeconds(spawnParticles.main.duration);

            isSpawningNPC = false;
            spawnParticles.Stop(true);
        }

        onNPCSpawned?.Invoke(Instantiate(npc, transform.position, transform.rotation).gameObject);
    }
}
