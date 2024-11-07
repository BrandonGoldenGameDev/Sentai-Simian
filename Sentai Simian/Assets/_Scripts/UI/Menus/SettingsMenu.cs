using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SettingsMenu : Menu
{
    [SerializeField]
    private GameObject menuToSwitchTo;
    [SerializeField]
    private GameObject[] tabs;
    [SerializeField]
    private Button controlsTabButton;
    private int currentTab = 0;

    private void Awake()
    {
        SwitchTab(currentTab);
    }

    private void Update()
    {
        if (Gamepad.current != null)
        {
            controlsTabButton.interactable = false;
        }
        else
        {
            controlsTabButton.interactable = true;
        }
    }

    public void Back()
    {
        Settings.Apply();
        gameObject.SetActive(false);
        menuToSwitchTo.SetActive(true);
    }

    public void SwitchTab(int tab)
    {
        if (tab >= tabs.Length)
            return;

        currentTab = tab;
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(false);
        }

        tabs[currentTab].SetActive(true);
    }
}
