using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayDropdown : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private Display[] displays;

    private void Awake()
    {
        displays = Display.displays;
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();

        Display currentDisplay;
        if (Settings.TargetDisplay.Value >= 0 && Settings.TargetDisplay.Value < displays.Length)
            currentDisplay = displays[Settings.TargetDisplay.Value];
        else
            currentDisplay = Display.main;

        for (int i = 0; i < displays.Length; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData("Monitor [" + i + "]"));

            if (currentDisplay == displays[i])
            {
                Settings.Resolution.Value = i;
                dropdown.value = i;
            }
        }

        dropdown.onValueChanged.AddListener(ChangeDisplay);
    }

    private void ChangeDisplay(int value)
    {
        Debug.Log($"Changing display to {displays[value]}");
        Settings.TargetDisplay.Value = value;
    }
}
