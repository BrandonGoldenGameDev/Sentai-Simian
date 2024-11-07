using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    [SerializeField]
    private AudioSource music;
    [SerializeField]
    private AudioClip victoryMusic;
    [SerializeField]
    private GameObject bg;
    [SerializeField]
    private GameObject victoryText;
    [SerializeField]
    private GameObject statsParent;
    [SerializeField]
    private TextMeshProUGUI timeTakenText;
    [SerializeField]
    private TextMeshProUGUI damageTakenText;
    [SerializeField]
    private TextMeshProUGUI parriesText;
    [SerializeField]
    private TextMeshProUGUI ultimatesText;
    [SerializeField]
    private Button quitButton;
    [SerializeField]
    private float timeScale = 0.2f;

    private WaveManager waveManager;
    private PlayerStatsTracker playerStats;

    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        quitButton.onClick.AddListener(LoadMainMenu);

        if (waveManager != null)
        {
            waveManager.onAllWavesCompleted += EnableVictoryUI;
        }

        playerStats = FindObjectOfType<PlayerStatsTracker>();
    }

    public void EnableVictoryUI()
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        victoryText.SetActive(true);
        bg.SetActive(true);

        if (playerStats == null)
            return;

        statsParent.SetActive(true);
        timeTakenText.gameObject.SetActive(true);
        timeTakenText.text = playerStats.LevelTimer.ToString("F1");
        damageTakenText.gameObject.SetActive(true);
        damageTakenText.text = playerStats.DamageTaken.ToString();
        parriesText.gameObject.SetActive(true);
        parriesText.text = playerStats.SuccessfulParries.ToString();
        ultimatesText.gameObject.SetActive(true);
        ultimatesText.text = playerStats.UltimatesCast.ToString();

        quitButton.gameObject.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(quitButton.gameObject);

        music.Stop();
        music.clip = victoryMusic;
        music.Play();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        GameManager.Instance.LoadMainMenu();
    }
}
