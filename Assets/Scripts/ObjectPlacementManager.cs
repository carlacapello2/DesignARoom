using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectPlacementManager : MonoBehaviour
{
    public ARPlaneManager arPlaneManager;
    public List<GameObject> objectPrefabs;
    private GameObject selectedObject;
    private ARRaycastManager arRaycastManager;
    private Vector2 touchPosition;
    public GameObject objectIcon, XROrigin;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool buttonsVisible = false;

    void Start()
    {
        arRaycastManager = XROrigin.GetComponent<ARRaycastManager>();
        ActivateObjectIcon();
    }

    void Update()
    {
        Debug.Log("Update method called");

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Touch phase began");

                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    Debug.Log("Touch over UI element");
                    return;
                }

                if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    Debug.Log("Raycast hit a plane");
                    var hitPose = hits[0].pose;
                    var hitObject = hits[0].trackable as ARPlane;

                    if (hitObject != null && selectedObject != null)
                    {
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
                            selectedObject = null;
                        }
                    }
                }
                else if (touch.phase == TouchPhase.Moved && selectedObject != null)
                {
                    Debug.Log("Touch phase moved");
                    if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                    {
                        var hitPose = hits[0].pose;
                        selectedObject.transform.position = hitPose.position;
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

    public void ButtonVisibleChanger()
    {
        if (buttonsVisible)
        {
            ToggleColorButtons();
        }
    }

    void ToggleColorButtons() //metodo creato per far chiudere il menù colori appena apro il menù oggetti
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
