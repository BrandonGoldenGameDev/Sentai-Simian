using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplaySettingsTab : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown fullscreenDropdown;
    [SerializeField]
    private TMP_Dropdown vsyncDropdown;
    [SerializeField]
    private Slider targetFramerateSlider;

    private void Awake()
    {
        vsyncDropdown.value = Settings.Vsync.Value;
        targetFramerateSlider.value = Settings.TargetFramerate.Value;
        fullscreenDropdown.value = Settings.FullscreenMode.Value;
        targetFramerateSlider.interactable = Settings.Vsync.Value <= 0;

        fullscreenDropdown.onValueChanged.AddListener(ChangeFullscreenMode);
        vsyncDropdown.onValueChanged.AddListener(ChangeVsyncLevel);
        targetFramerateSlider.onValueChanged.AddListener(ChangeTargetFramerate);
    }

    private void ChangeVsyncLevel(int vsyncLevel)
    {
        Settings.Vsync.Value = vsyncLevel;
        QualitySettings.vSyncCount = vsyncLevel;
        targetFramerateSlider.interactable = vsyncLevel <= 0;
    }

    private void ChangeTargetFramerate(float target)
    {
        if (target == targetFramerateSlider.maxValue)
            target = -1;

        Application.targetFrameRate = (int)target;
    }

    private void ChangeFullscreenMode(int mode)
    {
        var newMode = Screen.fullScreenMode;
        switch (mode)
        {
            case 0:
                newMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                newMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                newMode = FullScreenMode.Windowed;
                break;
            default:
                break;
        }

        Debug.Log($"New Screen Mode: {newMode}");
        Screen.fullScreenMode = newMode;
    }
}
