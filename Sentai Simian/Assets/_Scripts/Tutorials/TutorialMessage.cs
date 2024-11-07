using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TutorialMessage : MonoBehaviour
{
    private TMP_Text text;
    private string startingText;

    [SerializeField]
    private InputActionReference inputAction;
    [SerializeField]
    private Color highlight;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        startingText = text.text;
    }

    private void Update()
    {
        ParseTutorialMessage(startingText);
    }

    private void ParseTutorialMessage(string _text)
    {
        string[] tokens = _text.Split('[', ']');
        for (int i = 0; i < tokens.Length; i++)
        {
            // Only odd numbered tokens will be between brackets
            if (i % 2 == 0)
                continue;

            tokens[i] = GetBindingString();
        }

        string message = "";
        foreach (string token in tokens)
        {
            message += token;
        }

        text.text = message;
    }

    private string GetBindingString()
    {
        int bindingIndex = inputAction.action.GetBindingIndexForControl(inputAction.action.controls[0]);
        var binding = inputAction.action.bindings[bindingIndex];
        string bindingString;
        if (binding.isPartOfComposite)
        {
            string bindingName1 = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            int bindingIndex2 = inputAction.action.GetBindingIndexForControl(inputAction.action.controls[1]);
            string bindingName2 = InputControlPath.ToHumanReadableString(inputAction.action.bindings[bindingIndex2].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            int bindingIndex3 = inputAction.action.GetBindingIndexForControl(inputAction.action.controls[2]);
            string bindingName3 = InputControlPath.ToHumanReadableString(inputAction.action.bindings[bindingIndex3].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            int bindingIndex4 = inputAction.action.GetBindingIndexForControl(inputAction.action.controls[3]);
            string bindingName4 = InputControlPath.ToHumanReadableString(inputAction.action.bindings[bindingIndex4].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            bindingString = bindingName1 + bindingName2 + bindingName3 + bindingName4;
        }
        else
        {
            bindingString = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }

        return "<color=orange>" + bindingString + "</color>";
    }
}
