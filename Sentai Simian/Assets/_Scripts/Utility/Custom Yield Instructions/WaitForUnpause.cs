using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForUnpause : CustomYieldInstruction
{
    public override bool keepWaiting => GameManager.Instance.IsPaused;

    public WaitForUnpause()
    {
        Debug.Log("Waiting for unpause.");
    }
}
