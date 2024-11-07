using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialMessage;

    private void OnTriggerEnter(Collider other)
    {
        if (tutorialMessage == null)
            return;

        if (other.CompareTag("Player"))
        {
            tutorialMessage.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (tutorialMessage == null)
            return;

        if (other.CompareTag("Player"))
        {
            tutorialMessage.SetActive(false);
        }
    }
}
