using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ObjectPlacementManager : MonoBehaviour
{
    public ARPlaneManager arPlaneManager;
    public List<GameObject> objectPrefabs;
    private GameObject selectedObject;
    private ARRaycastManager arRaycastManager;
    private Vector2 touchPosition;
    public GameObject objectIcon, XROrigin;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private List<GameObject> objectButtons = new List<GameObject>();
    private bool buttonsVisible = false;

    void Start()
    {
        arRaycastManager = XROrigin.GetComponent<ARRaycastManager>();
        ActivateObjectIcon();
    }

    void Update()
    {
        if (selectedObject != null && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    var hitPose = hits[0].pose;


                    // Recupera il piano su cui è avvenuto il raycast
                    var plane = arPlaneManager.GetPlane(hits[0].trackableId);
                    if (plane != null)
                    {
                        var planeNormal = plane.normal;

                        // Calcola la direzione opposta alla normale del piano
                        var forward = planeNormal;
                        forward.y = 0; // Mantieni l'oggetto allineato orizzontalmente

                        var rotation = Quaternion.LookRotation(forward, Vector3.up);

                        GameObject placedObject = Instantiate(selectedObject, hitPose.position, rotation);
                        placedObject.tag = "PlacedObject";

                        selectedObject = null; // Deseleziona l'oggetto dopo averlo posizionato
                    }
                }
            }
        }
    }

    void ActivateObjectIcon()
    {
        Button mainButton = objectIcon.GetComponent<Button>();
        mainButton.onClick.AddListener(ToggleColorButtons);
    }

    public void buttonVisibleChanger()
    {
        if (buttonsVisible)
        {
            ToggleColorButtons();
        }
    }

    void ToggleColorButtons()
    {
        buttonsVisible = !buttonsVisible;
        foreach (Transform button in objectIcon.transform)
        {
            button.gameObject.SetActive(buttonsVisible);
        }
    }

    public void SelectObject(int index)
    {
        if (index >= 0 && index < objectPrefabs.Count)
        {
            selectedObject = objectPrefabs[index];
            Debug.Log($"Selected object: {selectedObject.name}");
        }
        else
        {
            Debug.LogError("Invalid object index");
        }
    }
}
