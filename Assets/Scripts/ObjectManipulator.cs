using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ObjectManipulator : MonoBehaviour
{
    private Camera arCamera;
    private GameObject selectedObject;
    private Color originalColor;
    private float initialDistance;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private float initialAngle;

    public GameObject deleteIcon; // Riferimento all'icona di eliminazione
    public GameObject screenIcon, containerObj; // Riferimenti agli altri oggetti
    public GraphicRaycaster uiRaycaster; // Riferimento al GraphicRaycaster
    public EventSystem eventSystem; // Riferimento all'EventSystem

    void Start()
    {
        arCamera = Camera.main;
        deleteIcon.SetActive(false); // Assicurati che l'icona di eliminazione sia inizialmente disattivata

        // Aggiungi Event Trigger all'icona di eliminazione
        EventTrigger trigger = deleteIcon.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = deleteIcon.AddComponent<EventTrigger>();
        }

        AddEventTriggerListener(trigger, EventTriggerType.PointerDown, OnDeleteIconClicked);
    }

    void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            if (IsPointerOverUIObject(touch))
            {
                Debug.Log("Touch is over a UI element, returning.");
                return;
            }

            Ray ray = arCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("PlacedObject"))
                {
                    SelectObject(hit.transform.gameObject);
                }
                else if (selectedObject != null && !hit.transform.CompareTag("DeleteIcon"))
                {
                    DeselectObject();
                }
            }
            else if (selectedObject != null)
            {
                DeselectObject();
            }
        }
        else if (touch.phase == TouchPhase.Moved && selectedObject != null)
        {
            TranslateObject(touch);
        }

        if (Input.touchCount == 2 && selectedObject != null)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = selectedObject.transform.localScale;
                initialRotation = selectedObject.transform.rotation;
                initialPosition = selectedObject.transform.position;
                initialAngle = CalculateAngleBetweenTouches(touchZero, touchOne);
            }
            else if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved)
            {
                ScaleObject(touchZero, touchOne);
                RotateObject(touchZero, touchOne);
            }
        }
    }

    private void SelectObject(GameObject obj)
    {
        if (selectedObject != null)
        {
            DeselectObject();
        }

        selectedObject = obj;
        Renderer renderer = selectedObject.GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = selectedObject.GetComponentInChildren<Renderer>();
        }

        if (renderer != null)
        {
            originalColor = renderer.material.color;
            renderer.material.color = new Color(0.015f, 0.678f, 0.619f); // Cambia colore a #04ad9e
        }

        deleteIcon.SetActive(true); // Mostra l'icona di eliminazione
        screenIcon.SetActive(false); // Nascondi screenIcon
        containerObj.SetActive(false); // Nascondi containerObj
    }

    private void DeselectObject()
    {
        if (selectedObject == null) return;

        Renderer renderer = selectedObject.GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = selectedObject.GetComponentInChildren<Renderer>();
        }

        if (renderer != null)
        {
            renderer.material.color = originalColor; // Ripristina il colore originale
        }
        selectedObject = null;

        deleteIcon.SetActive(false); // Nascondi l'icona di eliminazione
        screenIcon.SetActive(true); // Mostra screenIcon
        containerObj.SetActive(true); // Mostra containerObj
    }

    private void DeleteSelectedObject()
    {
        if (selectedObject != null)
        {
            Debug.Log("Deleting selected object.");
            Destroy(selectedObject);
            selectedObject = null;
            deleteIcon.SetActive(false); // Nascondi l'icona di eliminazione
            screenIcon.SetActive(true); // Mostra screenIcon
            containerObj.SetActive(true); // Mostra containerObj
        }
    }

    private void TranslateObject(Touch touch)
    {
        Vector3 touchPosition = arCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, arCamera.WorldToScreenPoint(selectedObject.transform.position).z));
        selectedObject.transform.position = new Vector3(touchPosition.x, touchPosition.y, selectedObject.transform.position.z);
    }

    private void ScaleObject(Touch touchZero, Touch touchOne)
    {
        float currentDistance = Vector2.Distance(touchZero.position, touchOne.position);
        if (Mathf.Approximately(initialDistance, 0))
        {
            return;
        }
        float scaleFactor = currentDistance / initialDistance;
        selectedObject.transform.localScale = initialScale * scaleFactor;
    }

    private void RotateObject(Touch touchZero, Touch touchOne)
    {
        float currentAngle = CalculateAngleBetweenTouches(touchZero, touchOne);
        float angleDelta = currentAngle - initialAngle;
        selectedObject.transform.rotation = initialRotation * Quaternion.Euler(0, angleDelta, 0);
    }

    private float CalculateAngleBetweenTouches(Touch touchZero, Touch touchOne)
    {
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
        return Vector2.SignedAngle(touchZeroPrevPos, touchOnePrevPos);
    }

    private bool IsPointerOverUIObject(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = new Vector2(touch.position.x, touch.position.y);

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(eventData, results);

        return results.Count > 0; // Rileva se il touch è su un qualsiasi elemento UI
    }

    public void OnDeleteIconClicked(BaseEventData data)
    {
        Debug.Log("DeleteIcon clicked, attempting to delete.");
        DeleteSelectedObject();
    }

    private void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(callback));
        trigger.triggers.Add(entry);
    }
}
