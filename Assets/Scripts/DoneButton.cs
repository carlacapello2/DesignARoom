using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoneButton : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject iconPrefab; // Prefab dell'icona
    public Canvas canvas; // Riferimento alla Canvas
    public Button doneButton; // Riferimento al Button Done
    public GameObject designScreen;

    void Start()
    {
        planeManager.planesChanged += OnPlanesChanged;
        doneButton.onClick.AddListener(OnDoneButtonClicked);
    }

    void OnDestroy()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Aggiorna la visibilità dell'icona in base alla presenza dei trackable solo se il pulsante Done è stato premuto
        if (designScreen.activeSelf)
        {
            designScreen.SetActive(planeManager.trackables.count > 0);
        }
    }

    void OnDoneButtonClicked()
    {
        // Passa alla schermata design quando il pulsante Done viene premuto
        if (planeManager.trackables.count > 0)
        {
            designScreen.SetActive(true);
        }
    }
}
