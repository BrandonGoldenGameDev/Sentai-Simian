using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour
{
    private AudioSource musicSource;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.ignoreListenerPause = true;
    }
}
