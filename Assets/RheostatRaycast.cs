using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class DragRightRaycast : MonoBehaviour
{
    private bool isDragging;
    private Vector3 lastMouse;

    [Header("Raycast Settings")]
    public LayerMask dragLayer;

    [Header("Clamp Limits (Local Position)")]
    public float minX = -2f;
    public float maxX = 2f;

    [Header("Ammeter System")]
    public Transform point0A;
    public Transform point2A;
    public Transform point10A;

    public TextMeshProUGUI ammeterText;
    public float maxAmp = 10f;

    [Header("2A Success")]
    public UnityEvent onReached2A;      // 🔥 EVENT
    public Image successImage;          // 🔥 IMAGE TO ENABLE

    private bool snappedTo2A = false;
    private bool isLocked = false;

    void Start()
    {
        ammeterText.text = "6.0";

        if (successImage != null)
            successImage.gameObject.SetActive(false);   // hide at start
    }

    void Update()
    {
        if (isLocked)
        {
            transform.position = point2A.position;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, dragLayer))
            {
                if (hit.transform == transform)
                {
                    isDragging = true;
                    lastMouse = Input.mousePosition;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
            isDragging = false;

        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMouse;

            transform.position += transform.right * (delta.x * 0.5f * Time.deltaTime);

            Vector3 local = transform.localPosition;
            local.x = Mathf.Clamp(local.x, minX, maxX);
            transform.localPosition = local;

            lastMouse = Input.mousePosition;

            UpdateAmmeter();
        }
    }

    void UpdateAmmeter()
    {
        float ampValue = 0f;

        float d0_2 = Vector3.Distance(point0A.position, point2A.position);
        float d2_10 = Vector3.Distance(point2A.position, point10A.position);

        float d0_current = Vector3.Distance(point0A.position, transform.position);
        float d2_current = Vector3.Distance(point2A.position, transform.position);

        if (d0_current <= d0_2)
        {
            float t = d0_current / d0_2;
            ampValue = Mathf.Lerp(0f, 2f, t);
        }
        else
        {
            float t = d2_current / d2_10;
            ampValue = Mathf.Lerp(2f, 10f, t);
        }

        ampValue = Mathf.Clamp(ampValue, 0, maxAmp);

        // ❌ REMOVED " A"
        ammeterText.text = ampValue.ToString("0.0");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == point2A)
        {
            transform.position = point2A.position;

            // ❌ NO "A" TEXT
            ammeterText.text = "2.0";

            isLocked = true;
            isDragging = false;

            // 🖼 ENABLE IMAGE
            if (successImage != null)
                successImage.gameObject.SetActive(true);

            // 🔥 FIRE EVENT
            onReached2A?.Invoke();
        }
    }
}
