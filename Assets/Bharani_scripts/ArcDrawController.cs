using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ArcDrawController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform nibPoint;
    public LineRenderer lineRenderer;

    [Header("Line Settings")]
    public float minPointDistance = 0.002f;

    [Header("Events")]
    public UnityEvent OnArcDrawCompleted;   // 🔥 EVENT AFTER ARC DRAW

    private float progress = 0f;
    private bool drawing = false;
    private bool drawCompleted = false;

    private List<Vector3> points = new List<Vector3>();

    void Start()
    {
        animator.speed = 0f;
        lineRenderer.positionCount = 0;
    }

    // -----------------------------------------
    //  AUTO PLAY AFTER SNAP
    // -----------------------------------------
    public void StartArrowDraw()
    {
        drawing = true;
        drawCompleted = false;
        progress = 0f;
        points.Clear();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (!drawing || drawCompleted)
            return;

        // Time-based animation progress
        progress += Time.deltaTime;
        progress = Mathf.Clamp01(progress);

        animator.Play(0, 0, progress);
        animator.Update(0f); // force exact nib position

        DrawLine();

        // ✅ FIRE EVENT ONCE WHEN COMPLETE
        if (progress >= 1f && !drawCompleted)
        {
            drawCompleted = true;
            drawing = false;

            OnArcDrawCompleted?.Invoke();
        }
    }

    void DrawLine()
    {
        Vector3 pos = nibPoint.position;

        if (points.Count == 0 ||
            Vector3.Distance(points[points.Count - 1], pos) >= minPointDistance)
        {
            points.Add(pos);
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
    }
}
