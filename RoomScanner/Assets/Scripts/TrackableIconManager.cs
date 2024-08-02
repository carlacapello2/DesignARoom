using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.Collections.Generic;

public class TrackableIconManager : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject iconPrefab; // Prefab dell'icona
    public Canvas canvas; // Riferimento alla Canvas
    public Button doneButton; // Riferimento al Button Done
    public Material newMaterial; // Il nuovo materiale da applicare ai piani
    private GameObject colorIcon;

    void Start()
    {
        planeManager.planesChanged += OnPlanesChanged;
        doneButton.onClick.AddListener(OnDoneButtonClicked);
        CreateColorIcon();
        colorIcon.SetActive(false); // Nascondi l'icona all'inizio
    }

    void OnDestroy()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    void CreateColorIcon()
    {
        colorIcon = Instantiate(iconPrefab, canvas.transform);
        colorIcon.transform.SetAsLastSibling(); // Porta l'icona in primo piano sulla Canvas
        colorIcon.GetComponent<Button>().onClick.AddListener(ChangeAllPlanesMaterial);
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Aggiorna la visibilità dell'icona in base alla presenza dei trackable solo se il pulsante Done è stato premuto
        if (colorIcon.activeSelf)
        {
            colorIcon.SetActive(planeManager.trackables.count > 0);
        }
    }

    void OnDoneButtonClicked()
    {
        // Visualizza l'icona quando il pulsante Done viene premuto
        if (planeManager.trackables.count > 0)
        {
            colorIcon.SetActive(true);
        }
    }

    void ChangeAllPlanesMaterial()
    {
        foreach (var plane in planeManager.trackables)
        {
            if (plane.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                // Assicurati che il piano abbia un componente Renderer
                Renderer planeRenderer = plane.GetComponent<Renderer>();
                if (planeRenderer != null)
                {
                    planeRenderer.material = newMaterial;
                }
            }
        }
    }
}
