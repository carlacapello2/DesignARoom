using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class ScanScreenManager : MonoBehaviour
{
    public GameObject scanScreen;
    public ARPlaneManager planeManager;
    public float areaChangeThreshold = 0.01f; // Tolleranza per il cambiamento dell'area
    private Dictionary<ARPlane, float> previousPlaneAreas;
    private float stationaryTime = 0f;

    void Start()
    {
        scanScreen.SetActive(true);
        previousPlaneAreas = new Dictionary<ARPlane, float>();
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDestroy()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        bool areaChanged = false;

        // Check for changes in updated planes
        foreach (var plane in args.updated)
        {
            if (plane.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                float currentArea = plane.size.x * plane.size.y;

                if (previousPlaneAreas.TryGetValue(plane, out float previousArea))
                {
                    if (Mathf.Abs(currentArea - previousArea) > areaChangeThreshold)
                    {
                        areaChanged = true;
                    }
                }

                previousPlaneAreas[plane] = currentArea;
            }
        }

        // Handle newly added planes
        foreach (var plane in args.added)
        {
            if (plane.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                float currentArea = plane.size.x * plane.size.y;
                previousPlaneAreas[plane] = currentArea;
                areaChanged = true; // Aggiungiamo un nuovo piano, quindi c'è sicuramente un cambiamento
            }
        }

        // Handle removed planes
        foreach (var plane in args.removed)
        {
            previousPlaneAreas.Remove(plane);
            areaChanged = true; // Rimuoviamo un piano, quindi c'è sicuramente un cambiamento
        }

        if (areaChanged)
        {
            scanScreen.SetActive(false);
            stationaryTime = 0f;
        }
    }

    void Update()
    {
        stationaryTime += Time.deltaTime;

        if (stationaryTime >= 5.0f)
        {
            scanScreen.SetActive(true);
        }
    }
}
