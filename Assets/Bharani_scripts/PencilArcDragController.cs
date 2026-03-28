using UnityEngine;

public class PencilArcDragController : MonoBehaviour
{
    [Header("Snap Settings")]
    public Transform snapPoint;
    public float snapDistance = 0.05f;

    [Header("Arrow Controller")]
    public ArcDrawController arrowDrawController;

    [Header("Reset Settings")]
    public bool smoothReset = true;
    public float resetSpeed = 6f;

    private bool snapped = false;
    private bool resetting = false;

    private float fixedY;
    private Plane dragPlane;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        fixedY = transform.position.y;

        // Infinite math plane at Y = fixedY
        dragPlane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));
    }

    void Update()
    {
        if (snapped)
            return;

        if (resetting)
        {
            SmoothReturn();
            return;
        }

        if (Input.GetMouseButton(0))
            DragOnXZ();

        if (Input.GetMouseButtonUp(0))
            CheckSnapOrReset();
    }

    // ---------------------------------------
    //  DRAG ON XZ PLANE
    // ---------------------------------------
    void DragOnXZ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = new Vector3(hitPoint.x, fixedY, hitPoint.z);
        }
    }

    // ---------------------------------------
    //  SNAP OR RESET LOGIC
    // ---------------------------------------
    void CheckSnapOrReset()
    {
        float distance = Vector3.Distance(
            arrowDrawController.nibPoint.position,
            snapPoint.position
        );

        if (distance <= snapDistance)
        {
            // SNAP SUCCESS
            snapped = true;

            Vector3 offset = transform.position - arrowDrawController.nibPoint.position;
            transform.position = snapPoint.position + offset;

            arrowDrawController.StartArrowDraw();
        }
        else
        {
            // RESET TO START
            resetting = true;
        }
    }

    // ---------------------------------------
    //  SMOOTH RETURN TO START POSITION
    // ---------------------------------------
    void SmoothReturn()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            startPosition,
            Time.deltaTime * resetSpeed
        );

        // Stop when close enough
        if (Vector3.Distance(transform.position, startPosition) < 0.01f)
        {
            transform.position = startPosition;
            resetting = false;
        }
    }
}
