using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomizeAudio : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private float minPitch, maxPitch;
    [SerializeField, Min(0)]
    private float minVolume, maxVolume;


    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource)
            Randomize();
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (!audioSource)
            return;

        Randomize();
        audioSource.PlayOneShot(clip);
    }

    private void Randomize()
    {
        audioSource.volume = Random.Range(minVolume, maxVolume);
        audioSource.pitch = Random.Range(minPitch, maxPitch);
    }
}
