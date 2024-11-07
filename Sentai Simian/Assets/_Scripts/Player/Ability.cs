using System;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public Action onAbilityRecharged;

    [SerializeField]
    protected float cooldown = 3f;
    protected float timer;

    public bool IsActive => timer <= 0f;
    public float PercentComplete => timer / cooldown;

    protected virtual void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (IsActive)
            {
                onAbilityRecharged?.Invoke();
            }
        }
    }

    public virtual void Use()
    {
        timer = cooldown;
    }
}
