using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootMenu : MonoBehaviour
{
    [SerializeField]
    private string levelToLoad;

    private void Start()
    {
        GameManager.Instance.LoadLevel(levelToLoad);
        Destroy(gameObject);
    }
}
