using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (GameManager.Instance.IsPaused)
            playerInput.SwitchCurrentActionMap("UI");
        else
            playerInput.SwitchCurrentActionMap("Player");
    }
}
