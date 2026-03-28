using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PencilDrawController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform nibPoint;
    public LineRenderer lineRenderer;

    [Header("Line Settings")]
    public float minPointDistance = 0.002f;

    [Header("Events")]
    public UnityEvent OnDrawCompleted;   // 🔥 EVENT AFTER DRAW

    private float currentProgress = 0f;
    private bool drawingEnabled = false;
    private bool drawCompleted = false;  // 🔒 ensure event fires once

    private List<Vector3> points = new List<Vector3>();

    void Start()
    {
        animator.speed = 0f;
        lineRenderer.positionCount = 0;
    }

    public void EnableDrawing()
    {
        drawingEnabled = true;
        drawCompleted = false;
    }

    public void SetProgress(float progress)
    {
        if (!drawingEnabled || drawCompleted)
            return;

        if (progress <= currentProgress)
            return;

        currentProgress = progress;

        animator.Play(0, 0, currentProgress);
        animator.Update(0f);   // 🔥 critical for accuracy

        DrawLine();

        // ✅ FIRE EVENT WHEN DRAW IS COMPLETE
        if (currentProgress >= 1f && !drawCompleted)
        {
            drawCompleted = true;
            OnDrawCompleted?.Invoke();
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
