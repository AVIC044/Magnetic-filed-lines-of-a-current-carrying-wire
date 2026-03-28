using UnityEngine;
using UnityEngine.Events;

public class DragToTargetWithEvents : MonoBehaviour
{
    [Header("Target")]
    public Collider targetCollider;     // Drop target
    public bool snapToTarget = true;
    public bool resetIfMissed = true;

    [Header("Drag Plane")]
    public bool constrainY = true;       // Lock Y
    private float fixedY;
    private Plane dragPlane;

    [Header("Events")]
    public UnityEvent OnDragStart;
    public UnityEvent OnDragging;
    public UnityEvent OnDragEnd;

    private bool isDragging = false;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        fixedY = transform.position.y;

        // Infinite plane for stable dragging (prevents screen snapping)
        dragPlane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartDrag();

        if (isDragging && Input.GetMouseButton(0))
            Drag();

        if (isDragging && Input.GetMouseButtonUp(0))
            EndDrag();
    }

    // -------------------------
    // DRAG START
    // -------------------------
    void TryStartDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                isDragging = true;
                OnDragStart?.Invoke();
            }
        }
    }

    // -------------------------
    // DRAGGING
    // -------------------------
    void Drag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 point = ray.GetPoint(enter);

            if (constrainY)
                point.y = fixedY;

            transform.position = point;

            OnDragging?.Invoke();
        }
    }

    // -------------------------
    // DRAG END
    // -------------------------
    void EndDrag()
    {
        isDragging = false;
        OnDragEnd?.Invoke();

        if (targetCollider != null &&
            targetCollider.bounds.Contains(transform.position))
        {
            if (snapToTarget)
            {
                transform.position = targetCollider.bounds.center;
            }
        }
        else
        {
            if (resetIfMissed)
            {
                transform.position = startPosition;
            }
        }
    }
}
