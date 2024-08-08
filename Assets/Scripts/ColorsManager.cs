using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;



public class ColorsManager : MonoBehaviour
{
    public GameObject colorIcon, plusIcon;
    public ARPlaneManager planeManager;
    private List<ChangeMaterialButton> colorButtons = new List<ChangeMaterialButton>();

    void Start()
    {
        ActivateColorIcon();
    }



    void ActivateColorIcon()
    {
        Button mainButton = colorIcon.GetComponent<Button>();
        mainButton.onClick.AddListener(ToggleColorButtons);

        // Trova i pulsanti figli
        var buttons = colorIcon.GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            if (button != mainButton) // Ignora il pulsante principale
            {
                button.gameObject.SetActive(false); // Nascondi i pulsanti figli all'inizio

                // Assegna il planeManager a ogni pulsante figlio
                ChangeMaterialButton changeMaterialButton = button.GetComponent<ChangeMaterialButton>();
                if (changeMaterialButton != null)
                {
                    changeMaterialButton.SetPlaneManager(planeManager);
                    colorButtons.Add(changeMaterialButton);//aggiunge alla lista di bottoni tutti gli oggetti figli dell'icona color
                }
            }
        }
    }


    void ToggleColorButtons()
    {
        foreach (var button in colorButtons)
        {
            button.gameObject.SetActive(!button.gameObject.activeSelf); // Attiva/disattiva i pulsanti figli al tocco sull'icona color
            plusIcon.gameObject.SetActive(button.gameObject.activeSelf);
        }
    }

    public void toggleActivation() //metodo pubblico che serve a disattivare e attivare i bottoni colors alla pressione di altri elementi UI
    {
        foreach (var button in colorButtons)
        {
            if (button.gameObject.activeInHierarchy)
            {
                button.gameObject.SetActive(!button.gameObject.activeSelf);
                plusIcon.gameObject.SetActive(button.gameObject.activeSelf);
            }
        }
    }

}
