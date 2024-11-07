using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindButton : MonoBehaviour
{
    [SerializeField] 
    private InputActionReference actionToRebind;
    [SerializeField]
    private int controlToRebind;
    [SerializeField] 
    private TMP_Text bindingDisplayNameText;
    [SerializeField] 
    private GameObject startRebindObject;
    [SerializeField] 
    private GameObject waitingForInputObject;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Start()
    {
        int bindingIndex = actionToRebind.action.GetBindingIndexForControl(actionToRebind.action.controls[controlToRebind]);
        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            actionToRebind.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void StartRebinding()
    {
        startRebindObject.SetActive(false);
        waitingForInputObject.SetActive(true);

        rebindingOperation = actionToRebind.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        int bindingIndex = actionToRebind.action.GetBindingIndexForControl(actionToRebind.action.controls[controlToRebind]);

        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            actionToRebind.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);
    }
}
