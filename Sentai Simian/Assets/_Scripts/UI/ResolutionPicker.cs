using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionPicker : MonoBehaviour
{
    Resolution[] resolutions;
    private TMP_Dropdown dropdown;

    private void Awake()
    {
        resolutions = Screen.resolutions;
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();

        Resolution currentResolution = Screen.currentResolution;
        for (int i = 0; i < resolutions.Length; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(resolutions[i].ToString()));

            if (currentResolution.width == resolutions[i].width && currentResolution.height == resolutions[i].height)
            {
                Settings.Resolution.Value = i;
                dropdown.value = i;
            }
        }

        dropdown.onValueChanged.AddListener(ChangeResolution);
    }

    private void ChangeResolution(int value)
    {
        Debug.Log($"Changing resolution to {resolutions[value]}");
        Settings.Resolution.Value = value;
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, Screen.fullScreenMode);
    }
}
