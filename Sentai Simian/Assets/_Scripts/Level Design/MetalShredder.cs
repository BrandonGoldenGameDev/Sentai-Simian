using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalShredder : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;

    private void Update()
    {
        transform.rotation = Quaternion.AngleAxis(Time.deltaTime * rotationSpeed, transform.up) * transform.rotation;
    }
}
