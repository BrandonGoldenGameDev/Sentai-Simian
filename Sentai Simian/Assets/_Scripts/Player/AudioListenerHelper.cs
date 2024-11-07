using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerHelper : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    private void LateUpdate()
    {
        transform.rotation = cameraTransform.rotation;
    }
}
