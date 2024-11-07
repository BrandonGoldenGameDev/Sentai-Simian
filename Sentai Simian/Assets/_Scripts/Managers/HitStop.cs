using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : Singleton<HitStop>
{
    private bool isStopped = false;
    private Coroutine hitStopCoroutine;

    public void Stop(float duration, bool interrupt = false, Action onStopEnded = null)
    {
        if (isStopped)
        {
            if (!interrupt)
                return;

            StopCoroutine(hitStopCoroutine);
        }

        hitStopCoroutine = StartCoroutine(StartHitStop(duration, onStopEnded));
    }

    private IEnumerator StartHitStop(float duration, Action onStopEnded)
    {
        isStopped = true;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);
        if (GameManager.Instance.IsPaused)
        {
            yield return new WaitForUnpause();
        }

        onStopEnded?.Invoke();
        isStopped = false;
        Time.timeScale = GameManager.Instance.CurrentTimeScale;
    }
}
