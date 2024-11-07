using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsTab : MonoBehaviour
{
    [SerializeField]
    private Slider masterVolumeSlider;
    [SerializeField]
    private Slider sfxVolumeSlider;
    [SerializeField]
    private Slider musicVolumeSlider;

    private void Awake()
    {
        masterVolumeSlider.value = Settings.MasterVolume.Value;
        sfxVolumeSlider.value = Settings.SFXVolume.Value;
        musicVolumeSlider.value = Settings.MusicVolume.Value;

        masterVolumeSlider.onValueChanged.AddListener(ChangeMasterVolume);
        sfxVolumeSlider.onValueChanged.AddListener(ChangeSFXVolume);
        musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume);
    }

    private void ChangeMasterVolume(float value)
    {
        Settings.MasterVolume.Value = value;
        AudioManager.Instance.SetVolumeSetting("MasterVolume", value);
    }

    private void ChangeSFXVolume(float value)
    {
        Settings.SFXVolume.Value = value;
        AudioManager.Instance.SetVolumeSetting("SFXVolume", value);
    }

    private void ChangeMusicVolume(float value)
    {
        Settings.MusicVolume.Value = value;
        AudioManager.Instance.SetVolumeSetting("MusicVolume", value);
    }
}
