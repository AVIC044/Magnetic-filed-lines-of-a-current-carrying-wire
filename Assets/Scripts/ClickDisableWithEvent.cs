using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ClickDisableWithEvent : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject glowobjwct;

    [Header("Raycast Settings")]
    public LayerMask clickableLayer;

    public GameObject texttogeton;

    [Header("Event Fired On Click")]
    public UnityEvent onObjectClicked;

    // 🔥 TARGET TRANSFORM FOR CAMERA
    public Transform cameraTarget;

    [Header("Camera Move Settings")]
    public float moveSpeed = 2f;
    public bool smoothMove = true;

    void Start()
    {  
        glowobjwct.SetActive(true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, clickableLayer))
            {
                GameObject clickedObj = hit.collider.gameObject;

                if (texttogeton != null)
                {
                    texttogeton.SetActive(true);         
                }

                // Disable the object
                clickedObj.SetActive(false);

                // Fire event
                onObjectClicked?.Invoke();
            }
        }
    }

    // 🔥 CALL THIS FROM EVENT OR BUTTON
    public void MoveCameraToTarget()
    {
        if (cameraTarget == null || mainCamera == null) return;

        if (smoothMove)
            StartCoroutine(SmoothMove());
        else
        {
            mainCamera.transform.position = cameraTarget.position;
            mainCamera.transform.rotation = cameraTarget.rotation;
        }
    }

    IEnumerator SmoothMove()
    {
        Transform cam = mainCamera.transform;

        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;

        Vector3 endPos = cameraTarget.position;
        Quaternion endRot = cameraTarget.rotation;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;

            cam.position = Vector3.Lerp(startPos, endPos, t);
            cam.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }
    }
}
