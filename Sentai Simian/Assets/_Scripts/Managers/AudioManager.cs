using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    private AudioMixer mixer;
    private AudioMixerSnapshot currentSnapshot;
    private AudioMixerSnapshot previousSnapshot;

    public AudioMixerSnapshot CurrentSnapshot => currentSnapshot;
    public AudioMixerSnapshot PreviousSnapshot => previousSnapshot;

    protected void Start()
    {
        if (mixer == null)
        {
            Debug.LogError("AudioManager: No mixer assigned to this manager.");
            return;
        }

        SetVolumeSetting("MasterVolume", Settings.MasterVolume.Value);
        SetVolumeSetting("SFXVolume", Settings.SFXVolume.Value);
        SetVolumeSetting("MusicVolume", Settings.MusicVolume.Value);
    }

    public void SetVolumeSetting(string name, float value)
    {
        mixer.SetFloat(name, Mathf.Log10(value) * 20f);
    }

    public void SwitchSnapshots(AudioMixerSnapshot snapshot, float duration)
    {
        if (snapshot != currentSnapshot)
        {
            previousSnapshot = currentSnapshot;
            snapshot.TransitionTo(duration);
            currentSnapshot = snapshot;
        }
    }

    public AudioMixerSnapshot GetSnapshot(string name) => mixer.FindSnapshot(name);
}
