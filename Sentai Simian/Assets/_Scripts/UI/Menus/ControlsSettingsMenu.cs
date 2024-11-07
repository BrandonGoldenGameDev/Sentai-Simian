using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsSettingsMenu : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputs;

    private const string RebindsKey = "rebinds";

    private void Awake()
    {
        if (string.IsNullOrEmpty(Settings.Rebinds.Value))
            return;

        inputs.LoadBindingOverridesFromJson(Settings.Rebinds.Value);
    }

    public void Save()
    {

    }

    public void ClearBindings()
    {
        inputs.RemoveAllBindingOverrides();
    }
}
