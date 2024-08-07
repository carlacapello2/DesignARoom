using UnityEngine;
using UnityEngine.UI;

public class ToggleButtons : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;

    void Start()
    {
        button1.onClick.AddListener(() => OnButtonPressed(button1));
        button2.onClick.AddListener(() => OnButtonPressed(button2));
        button3.onClick.AddListener(() => OnButtonPressed(button3));
    }

    void OnButtonPressed(Button selectedButton)
    {
        // Deseleziona tutti i bottoni
        button1.interactable = true;
        button2.interactable = true;
        button3.interactable = true;

        
    }
}
