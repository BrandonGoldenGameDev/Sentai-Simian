using UnityEngine;
using UnityEngine.InputSystem;

public class RebindSaveLoad : MonoBehaviour
{
    public InputActionAsset actions;

    public void OnEnable()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
        {
            actions.LoadBindingOverridesFromJson(rebinds);
            Debug.Log("Loading keybinds.");
        }
    }

    public void OnDisable()
    {
        Debug.Log("Saving Keybinds.");
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
