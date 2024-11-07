using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplaySettingsTab : MonoBehaviour
{
    [SerializeField]
    private Slider cameraShakeSlider;
    [SerializeField]
    private Toggle hudToggle;

    private void Awake()
    {
        cameraShakeSlider.value = Settings.CameraShakeAmount.Value;
        cameraShakeSlider.onValueChanged.AddListener(value => Settings.CameraShakeAmount.Value = value);

        hudToggle.isOn = Settings.EnableHUD.Value;
        hudToggle.onValueChanged.AddListener(value => Settings.EnableHUD.Value = value);
    }
}
