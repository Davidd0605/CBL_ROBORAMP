using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class DisableAfterClick: MonoBehaviour
{
    public Button myButton;
    
    void Start()
    {
        myButton.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}