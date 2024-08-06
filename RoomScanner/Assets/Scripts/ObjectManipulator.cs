using UnityEngine;
using UnityEngine.XR.ARFoundation;

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

    void Start()
    {
        arCamera = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.CompareTag("PlacedObject"))
                    {
                        SelectObject(hit.transform.gameObject);
                    }
                    else if (selectedObject != null)
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
        }
        else if (Input.touchCount == 2 && selectedObject != null)
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
        if (renderer != null)
        {
            originalColor = renderer.material.color;
            renderer.material.color = Color.yellow; // Cambia colore per indicare la selezione
        }
    }

    private void DeselectObject()
    {
        Renderer renderer = selectedObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = originalColor; // Ripristina il colore originale
        }
        selectedObject = null;
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
}
