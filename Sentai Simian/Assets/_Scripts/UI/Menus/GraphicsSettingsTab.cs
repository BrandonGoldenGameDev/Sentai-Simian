using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicsSettingsTab : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown qualityDropdown;

    private void Awake()
    {
        QualitySettings.GetQualityLevel();
    }
}
