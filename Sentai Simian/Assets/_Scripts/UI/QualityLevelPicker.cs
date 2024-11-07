using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QualityLevelPicker : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();

        dropdown.AddOptions(new List<string>(QualitySettings.names));
        dropdown.value = QualitySettings.GetQualityLevel();

        dropdown.onValueChanged.AddListener(ChangeQuality);
    }

    private void ChangeQuality(int value)
    {
        Debug.Log($"Changing quality to {value}");
        QualitySettings.SetQualityLevel(value, true);
    }
}
