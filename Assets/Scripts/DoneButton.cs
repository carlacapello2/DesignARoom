using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoneButton : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public Canvas canvas;
    public Button doneButton;
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
   
        if (designScreen.activeSelf) //se la schermata di design � attiva pu� rimanerlo solo se il numero di trackables � meggiore di zero
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
