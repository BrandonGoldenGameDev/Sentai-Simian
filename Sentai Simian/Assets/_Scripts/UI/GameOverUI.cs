using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup flashCanvas;
    [SerializeField]
    private RectTransform playerHUD;
    [SerializeField]
    private RectTransform failedText;
    private CanvasGroup failedTextGroup;

    [SerializeField]
    private GameObject quitButton;
    private CanvasGroup quitButtonGroup;

    [SerializeField]
    private float timeScale = 0.2f;
    [SerializeField]
    private float flashFadeDuration = 0.5f;
    [SerializeField]
    private float gameOverDuration = 7f;

    [SerializeField]
    private AudioClip gameOverMusicClip;
    [SerializeField]
    private double gameOverMusicDelay = 2f;
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource deathSource;
    [SerializeField]
    private AudioClip deathClip;

    private void Start()
    {
        var player = FindObjectOfType<PlayerHealth>();
        if (player != null)
            player.onPlayerDeath += OnPlayerDeath;

        failedTextGroup = failedText.GetComponent<CanvasGroup>();
        quitButtonGroup = quitButton.GetComponent<CanvasGroup>();

        deathSource = GetComponent<AudioSource>();
        deathSource.ignoreListenerPause = true;
    }

    private void OnPlayerDeath()
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        musicSource.clip = gameOverMusicClip;
        musicSource.PlayScheduled(AudioSettings.dspTime + gameOverMusicDelay);
        deathSource.Play();

        playerHUD.gameObject.SetActive(false);
        flashCanvas.gameObject.SetActive(true);
        flashCanvas.alpha = 1f;
        flashCanvas.LeanAlpha(0f, flashFadeDuration)
            .setIgnoreTimeScale(true)
            .setEaseInQuart();

        LeanTween.delayedCall(2f, EnableFailedText).setIgnoreTimeScale(true);
        LeanTween.delayedCall(4f, EnableQuitButton).setIgnoreTimeScale(true);
    }

    private void EnableFailedText()
    {
        failedText.gameObject.SetActive(true);
        failedTextGroup.alpha = 0f;
        failedTextGroup.LeanAlpha(1f, 1f)
            .setIgnoreTimeScale(true)
            .setEaseInOutCubic();

        failedText.LeanMoveX(0f, 1f)
            .setIgnoreTimeScale(true)
            .setEaseOutCubic();
    }

    private void EnableQuitButton()
    {
        EventSystem.current.SetSelectedGameObject(quitButton);
        quitButton.gameObject.SetActive(true);
        quitButtonGroup.alpha = 0f;
        quitButtonGroup.LeanAlpha(1f, 1f)
            .setIgnoreTimeScale(true)
            .setEaseInOutCubic();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        GameManager.Instance.LoadMainMenu();
    }
}
