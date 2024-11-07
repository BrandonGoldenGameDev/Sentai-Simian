using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInstruction : MonoBehaviour
{
    [SerializeField, Min(0)]
    private float duration;

    private void Awake()
    {
        Destroy(gameObject, duration);
    }
}
