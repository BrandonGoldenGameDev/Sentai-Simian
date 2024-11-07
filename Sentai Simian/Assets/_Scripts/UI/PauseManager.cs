using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private enum ActiveMenuType
    {
        Pause, Settings
    }

    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject settingsMenu;
    [SerializeField]
    private GameObject background;
    private bool canPause = true;

    private ActiveMenuType activeMenuType = ActiveMenuType.Pause;
    private PlayerInput playerInput;
    private PlayerHealth playerHealth;
    private WaveManager waveManager;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>(true);
        playerHealth = FindObjectOfType<PlayerHealth>(true);
        waveManager = FindObjectOfType<WaveManager>(true);

        playerInput.actions["Pause"].performed += _ => OnPausePressed();
        playerInput.actions["Cancel"].performed += _ => OnCancelPressed();
        playerHealth.onPlayerDeath += () => canPause = false;

        if (waveManager != null)
            waveManager.onAllWavesCompleted += () => canPause = false;
    }

    public void OnPausePressed()
    {
        if (!canPause)
            return;

        switch (activeMenuType)
        {
            case ActiveMenuType.Pause:
                TogglePause();
                break;
            case ActiveMenuType.Settings:
                OpenPauseMenu();
                break;
            default:
                break;
        }
    }

    public void OnCancelPressed()
    {
        switch (activeMenuType)
        {
            case ActiveMenuType.Pause:
                ExitPauseMenu();
                break;
            case ActiveMenuType.Settings:
                OpenPauseMenu();
                break;
            default:
                break;
        }
    }

    public void TogglePause()
    {
        if (GameManager.Instance.IsPaused)
            ExitPauseMenu();
        else
            EnterPauseMenu();
    }

    private void EnterPauseMenu()
    {
        OpenPauseMenu();
        background.SetActive(true);
        playerInput.SwitchCurrentActionMap("UI");
        GameManager.Instance.Pause();
    }

    private void ExitPauseMenu()
    {
        background.SetActive(false);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");
        GameManager.Instance.Resume();
    }

    public void OpenSettingsMenu()
    {
        activeMenuType = ActiveMenuType.Settings;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void OpenPauseMenu()
    {
        activeMenuType = ActiveMenuType.Pause;
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void QuitToMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }
}
