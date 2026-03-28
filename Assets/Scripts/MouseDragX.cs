using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class MouseDragX : MonoBehaviour
{
    private Vector3 startMousePos;
    private Vector3 startLocalPos;

    [Header("Drag")]
    public float sensitivity = 0.01f;

    [Header("Snap (Value Based)")]
    public float targetX;
    public float snapTolerance = 0.02f;

    [Header("Enable On Success")]
    public List<GameObject> objectsToEnable;

    [Header("Events")]
    public UnityEvent OnTargetReached;   // 🔥 MAIN EVENT

    private bool isLocked = false;
    private bool completed = false;

    void OnMouseDown()
    {
        if (isLocked || completed) return;

        startMousePos = Input.mousePosition;
        startLocalPos = transform.localPosition;
    }

    void OnMouseDrag()
    {
        if (isLocked || completed) return;

        float mouseDeltaX = Input.mousePosition.x - startMousePos.x;
        float newX = startLocalPos.x + mouseDeltaX * sensitivity;

        transform.localPosition = new Vector3(
            newX,
            startLocalPos.y,
            startLocalPos.z
        );

        CheckSnap(newX);
    }

    void CheckSnap(float currentX)
    {
        if (Mathf.Abs(currentX - targetX) <= snapTolerance)
        {
            LockToTarget();
        }
    }

    void LockToTarget()
    {
        if (completed) return;

        completed = true;
        isLocked = true;

        // Snap exactly to target
        transform.localPosition = new Vector3(
            targetX,
            transform.localPosition.y,
            transform.localPosition.z
        );

        // Enable GameObjects
        foreach (GameObject go in objectsToEnable)
        {
            if (go) go.SetActive(true);
        }

        // 🔔 FIRE EVENT
        OnTargetReached?.Invoke();
    }

    // Optional reset
    public void ResetSnap()
    {
        isLocked = false;
        completed = false;
    }
}
