using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    [SerializeField]
    private GameObject settingsMenu;

    public void LoadLevel(string levelToLoad)
    {
        GameManager.Instance.StartLevelTransition(levelToLoad);
    }

    public void SwitchToSettingsMenu()
    {
        gameObject.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
