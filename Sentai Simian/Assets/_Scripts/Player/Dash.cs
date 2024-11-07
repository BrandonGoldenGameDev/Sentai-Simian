using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability
{
    [SerializeField]
    private Transform model;
    private AfterImageSpawner afterImageSpawner;
    private int scaleTweenID;

    private void Awake()
    {
        afterImageSpawner = GetComponent<AfterImageSpawner>();
        afterImageSpawner.enabled = false;
    }

    public override void Use()
    {
        base.Use();

        LeanTween.cancel(scaleTweenID);
        model.localScale = new Vector3(0.6f, 1f, 1.3f);
        scaleTweenID = model.LeanScale(Vector3.one, 0.3f).setEase(LeanTweenType.easeOutSine).id;

        afterImageSpawner.enabled = true;
    }

    public void EndDash()
    {
        afterImageSpawner.enabled = false;
    }
}
