using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform afterimagePrefab;
    private Animator animator;
    [SerializeField]
    private float afterimageSpawnRate = 0.1f;
    private float timer = 0f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = afterimageSpawnRate;
            SpawnAfterimage();
        }
    }

    private void SpawnAfterimage()
    {
        AfterImage afterimage = Instantiate(afterimagePrefab, animator.transform.position, animator.transform.rotation).GetComponent<AfterImage>();
        afterimage.Activate(animator);
    }
}
