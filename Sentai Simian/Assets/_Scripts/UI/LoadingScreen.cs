using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private RectTransform background;
    [SerializeField]
    private float transitionDuration = 1f;

    private void Start()
    {
        GameManager.Instance.onLevelLoadStarted += StartLoadingLevel;
    }

    private void StartLoadingLevel(AsyncOperation ao)
    {
        gameObject.SetActive(true);
        StartCoroutine(UpdateLoadingScreen(ao));
    }

    // Updates the loading screen based on the loading progress
    private IEnumerator UpdateLoadingScreen(AsyncOperation ao)
    {
        while (ao.progress < 1f)
        {
            yield return null;
        }

        TransitionOut();
    }

    // Transitions into the loading screen
    public void TransitionIn(Action onTransitionComplete)
    {
        gameObject.SetActive(true);
        background.anchoredPosition = new Vector2(1920f, 0f);
        background.LeanMoveX(0f, transitionDuration)
            .setEaseOutQuad()
            .setIgnoreTimeScale(true)
            .setOnComplete(onTransitionComplete);
    }

    // Transitions out of the loading screen
    private void TransitionOut()
    {
        background.anchoredPosition = new Vector2(0f, 0f);
        background.LeanMoveX(-1920f, transitionDuration)
            .setEaseOutQuad()
            .setIgnoreTimeScale(true)
            .setOnComplete(() => gameObject.SetActive(false));
    }
}
