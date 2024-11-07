using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject firstSelectedGameObject;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);
    }
}
