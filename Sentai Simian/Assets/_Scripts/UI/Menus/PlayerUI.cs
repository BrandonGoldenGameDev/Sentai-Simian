using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour
{
    private PauseManager pauseManager;
    private PlayerInput playerInput;

    private void Start()
    {
        pauseManager = FindObjectOfType<PauseManager>(true);
        playerInput = FindObjectOfType<PlayerInput>(true);
        playerInput.actions["Pause"].performed += OnPausePressed;
        playerInput.actions["Cancel"].performed += OnCancelPressed;
    }

    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (pauseManager == null)
            return;

        pauseManager.OnPausePressed();
    }

    private void OnCancelPressed(InputAction.CallbackContext ctx)
    {

    }
}
