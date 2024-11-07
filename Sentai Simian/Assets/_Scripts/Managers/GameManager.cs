using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public Action<AsyncOperation> onLevelLoadStarted;
    public Action<AsyncOperation> onLevelUnloadStarted;

    [SerializeField]
    private LoadingScreen loadingScreen;
    private string currentLevel;
    private float currentTimeScale = 1f;
    private bool isPaused = false;

    [SerializeField]
    private Transform[] managers;

    public float CurrentTimeScale => currentTimeScale;
    public bool IsPaused => isPaused;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        currentLevel = SceneManager.GetActiveScene().name;

        if (managers != null)
        {
            foreach (var manager in managers)
            {
                Instantiate(manager, transform);
            }
        }
    }

    public void StartLevelTransition(string levelToTransitionTo)
    {
        if (loadingScreen != null)
        {
            loadingScreen.TransitionIn(() => LoadLevel(levelToTransitionTo));
        }
        else
        {
            Debug.Log("[GameManager] No loading screen was assigned.");
            LoadLevel(levelToTransitionTo);
        }
    }

    public void LoadLevel(string levelToLoad)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Single);
        onLevelLoadStarted?.Invoke(ao);
        ao.completed += OnLevelLoaded;

        /*
        if (currentLevel != null && currentLevel != "_Boot")
        {
            UnloadLevel(currentLevel);
        }

        currentLevel = levelToLoad;
        */
    }

    private void OnLevelLoaded(AsyncOperation obj)
    {
        // SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentLevel));
    }

    public void UnloadLevel(string levelToUnload)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelToUnload);
    }

    public void ReloadLevel()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        StartLevelTransition(currentLevel);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        AudioManager.Instance.SwitchSnapshots(AudioManager.Instance.GetSnapshot("Paused"), 0.5f);
        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        AudioManager.Instance.SwitchSnapshots(AudioManager.Instance.GetSnapshot("Unpaused"), 0.5f);
        isPaused = false;
    }

    public void LoadMainMenu()
    {
        Resume();
        StartLevelTransition("MainMenu");
    }
}
