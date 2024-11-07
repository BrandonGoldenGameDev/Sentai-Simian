using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSettings : MonoBehaviour
{
    private CinemachineImpulseListener listener;

    private void Awake()
    {
        listener = GetComponent<CinemachineImpulseListener>();
        listener.m_Gain = Settings.CameraShakeAmount.Value;
    }

    private void Update()
    {
        if (listener == null)
            return;

        listener.m_Gain = Settings.CameraShakeAmount.Value;
    }
}
