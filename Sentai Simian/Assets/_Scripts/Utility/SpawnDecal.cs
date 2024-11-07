using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDecal : MonoBehaviour
{
    [SerializeField]
    private GameObject decal;

    private void Awake()
    {
        Instantiate(decal, transform.position, Quaternion.Euler(90f, Random.Range(0f, 360f), 0f));
    }
}
