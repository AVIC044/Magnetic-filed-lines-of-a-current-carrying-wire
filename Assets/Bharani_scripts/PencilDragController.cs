//using UnityEngine;
//using UnityEngine.Events;

//public class PencilDragController : MonoBehaviour
//{
//    [Header("References")]
//    public PencilDrawController circleDrawController;
//    public Transform snapPoint;
//    public LayerMask groundMask;

//    [Header("Visual")]
//    public Transform pencilMesh;
//    public Transform animatedCircleRoot;

//    [Header("Gesture Settings")]
//    public float requiredRotation = 360f;
//    public bool counterClockwiseOnly = true;

//    [Header("Drag Events")]
//    public UnityEvent OnDragStart;
//    public UnityEvent OnDragging;
//    public UnityEvent OnDragEnd;

//    private Vector3 defaultPosition;
//    private float fixedY;
//    private bool snapped = false;
//    private bool dragging = false;

//    private Vector2 screenCenter;
//    private float accumulatedAngle = 0f;
//    private float lastAngle;
//    private bool drawingStarted = false;

//    void Start()
//    {
//        defaultPosition = transform.position;
//        fixedY = transform.position.y;

//        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
//    }

//    void Update()
//    {
//        if (!snapped)
//        {
//            HandlePlacementDrag();
//        }
//        else
//        {
//            HandleGestureDrawing();
//        }
//    }

//    // ---------------- STAGE 1 : DRAG ON XZ ----------------
//    void HandlePlacementDrag()
//    {
//        // Detect drag start
//        if (Input.GetMouseButtonDown(0))
//        {
//            dragging = true;
//            OnDragStart?.Invoke();
//        }

//        if (dragging && Input.GetMouseButton(0))
//        {
//            // While dragging
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
//            {
//                Vector3 pos = transform.position;
//                pos.x = hit.point.x;
//                pos.z = hit.point.z;
//                pos.y = fixedY;
//                transform.position = pos;

//                // Call dragging event
//                OnDragging?.Invoke();
//            }
//        }

//        // Detect drag end
//        if (dragging && Input.GetMouseButtonUp(0))
//        {
//            dragging = false;
//            OnDragEnd?.Invoke();

//            if (Vector3.Distance(transform.position, snapPoint.position) <= 0.1f)
//            {
//                SnapToPoint();
//            }
//            else
//            {
//                ResetToStart();
//            }
//        }
//    }

//    void SnapToPoint()
//    {
//        snapped = true;

//        transform.position = snapPoint.position;

//        animatedCircleRoot.localRotation = Quaternion.identity;
//        pencilMesh.localRotation = Quaternion.identity;

//        circleDrawController.EnableDrawing();
//    }

//    void ResetToStart()
//    {
//        transform.position = defaultPosition;
//    }

//    // ---------------- STAGE 2 : GESTURE DRAW ----------------
//    void HandleGestureDrawing()
//    {
//        if (!Input.GetMouseButton(0))
//        {
//            drawingStarted = false;
//            accumulatedAngle = 0f;
//            return;
//        }

//        Vector2 mousePos = Input.mousePosition;
//        float currentAngle = AngleFromCenter(mousePos);

//        if (!drawingStarted)
//        {
//            lastAngle = currentAngle;
//            drawingStarted = true;
//            return;
//        }

//        float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle);

//        if (counterClockwiseOnly && deltaAngle < 0f)
//        {
//            lastAngle = currentAngle;
//            return;
//        }

//        accumulatedAngle += Mathf.Abs(deltaAngle);
//        lastAngle = currentAngle;

//        pencilMesh.Rotate(Vector3.up, deltaAngle, Space.World);

//        float progress = Mathf.Clamp01(accumulatedAngle / requiredRotation);
//        circleDrawController.SetProgress(progress);
//    }

//    float AngleFromCenter(Vector2 pos)
//    {
//        Vector2 dir = pos - screenCenter;
//        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
//    }
//}

using UnityEngine;
using UnityEngine.Events;

public class PencilDragController : MonoBehaviour
{
    [Header("References")]
    public PencilDrawController circleDrawController;
    public Transform snapPoint;
    public LayerMask groundMask;

    [Header("Visual")]
    public Transform pencilMesh;
    public Transform animatedCircleRoot;

    [Header("Gesture Settings")]
    public float requiredRotation = 360f;
    public bool counterClockwiseOnly = true;

    [Header("Drag Events")]
    public UnityEvent OnDragStart;
    public UnityEvent OnDragging;
    public UnityEvent OnDragEnd;

    [Header("Snap Events")]   // ⭐ NEW EVENT GROUP
    public UnityEvent OnSnapCompleted;

    private Vector3 defaultPosition;
    private float fixedY;
    private bool snapped = false;
    private bool dragging = false;

    private Vector2 screenCenter;
    private float accumulatedAngle = 0f;
    private float lastAngle;
    private bool drawingStarted = false;

    // ⭐ NEW — MATHEMATICAL DRAG PLANE
    private Plane dragPlane;

    void Start()
    {
        defaultPosition = transform.position;
        fixedY = transform.position.y;

        screenCenter = new Vector2(Screen.width / 2f, Screen.width / 2f);

        dragPlane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));
    }

    void Update()
    {
        if (!snapped)
        {
            HandlePlacementDrag();
        }
        else
        {
            HandleGestureDrawing();
        }
    }

    // ---------------- STAGE 1 : DRAG ON XZ ----------------
    void HandlePlacementDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
            OnDragStart?.Invoke();
        }

        if (dragging && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                transform.position = new Vector3(hitPoint.x, fixedY, hitPoint.z);
                OnDragging?.Invoke();
            }
        }

        if (dragging && Input.GetMouseButtonUp(0))
        {
            dragging = false;
            OnDragEnd?.Invoke();

            if (Vector3.Distance(transform.position, snapPoint.position) <= 0.1f)
            {
                SnapToPoint();
            }
            else
            {
                ResetToStart();
            }
        }
    }

    void SnapToPoint()
    {
        snapped = true;

        transform.position = snapPoint.position;

        animatedCircleRoot.localRotation = Quaternion.identity;
        pencilMesh.localRotation = Quaternion.identity;

        circleDrawController.EnableDrawing();

        // ⭐ NEW — TRIGGER SNAP COMPLETE EVENTS
        OnSnapCompleted?.Invoke();
    }

    void ResetToStart()
    {
        transform.position = defaultPosition;
    }

    // ---------------- STAGE 2 : GESTURE DRAW ----------------
    void HandleGestureDrawing()
    {
        if (!Input.GetMouseButton(0))
        {
            drawingStarted = false;
            accumulatedAngle = 0f;
            return;
        }

        Vector2 mousePos = Input.mousePosition;
        float currentAngle = AngleFromCenter(mousePos);

        if (!drawingStarted)
        {
            lastAngle = currentAngle;
            drawingStarted = true;
            return;
        }

        float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle);

        if (counterClockwiseOnly && deltaAngle < 0f)
        {
            lastAngle = currentAngle;
            return;
        }

        accumulatedAngle += Mathf.Abs(deltaAngle);
        lastAngle = currentAngle;

        pencilMesh.Rotate(Vector3.up, deltaAngle, Space.World);

        float progress = Mathf.Clamp01(accumulatedAngle / requiredRotation);
        circleDrawController.SetProgress(progress);
    }

    float AngleFromCenter(Vector2 pos)
    {
        Vector2 dir = pos - screenCenter;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}


