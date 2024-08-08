using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ChangeMaterialButton : MonoBehaviour
{
    private ARPlaneManager planeManager;
    public Material materialToApply; // Materiale da applicare

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ChangeAllPlanesMaterial); //aggiunge un listener al pulsante. Al click chiama il metodo ChangeAllPlanesMaterial
    }

    public void SetPlaneManager(ARPlaneManager manager)
    {
        planeManager = manager;
    }

    void ChangeAllPlanesMaterial()
    {
        if (planeManager == null) return;

        foreach (var plane in planeManager.trackables)
        {
            if (plane.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                
                Renderer planeRenderer = plane.GetComponent<Renderer>();
                if (planeRenderer != null)
                {
                    planeRenderer.material = materialToApply;
                }
            }
        }
    }
}
