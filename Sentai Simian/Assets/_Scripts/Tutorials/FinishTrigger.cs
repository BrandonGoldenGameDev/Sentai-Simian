using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private VictoryUI victoryUI;

    private void Awake()
    {
        victoryUI = FindObjectOfType<VictoryUI>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            victoryUI.EnableVictoryUI();
        }
    }
}
