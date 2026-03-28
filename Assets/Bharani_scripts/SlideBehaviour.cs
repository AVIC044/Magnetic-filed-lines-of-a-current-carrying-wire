using UnityEngine;
using UnityEngine.Events;
using System;

public class SlideBehaviour : MonoBehaviour
{
    [Header("Slide Events")]
    public UnityEvent OnSnapCompletedUnity;
    public UnityEvent OnAnimationCompletedUnity;
    public UnityEvent OnSlideCompletedUnity;

    public Action OnSnapCompletedAction;
    public Action OnAnimationCompletedAction;
    public Action OnSlideCompletedAction;

    private bool isCompleted = false;

    // -------- SNAP COMPLETED (Compass or Pencil) --------
    public void SnapCompleted()
    {
        OnSnapCompletedUnity?.Invoke();
        OnSnapCompletedAction?.Invoke();
    }

    // -------- ANIMATION COMPLETED (Arc or Circle) --------
    public void AnimationCompleted()
    {
        OnAnimationCompletedUnity?.Invoke();
        OnAnimationCompletedAction?.Invoke();
    }

    // -------- FINAL SLIDE COMPLETION --------
    public void SlideCompleted()
    {
        if (isCompleted) return;
        isCompleted = true;

        OnSlideCompletedUnity?.Invoke();
        OnSlideCompletedAction?.Invoke();

        SlideManager.Instance.MarkSlideCompleted();
    }

    // -------- RESTORE WHEN USER RETURNS TO THIS SLIDE --------
    public void RestoreCompletedState()
    {
        // YOU customize per slide (enable/disable objects)
        // Example:
        // - disable highlights
        // - disable drags
        // - show completed visuals
        // - position objects as completed

        // This is intentionally empty for your customization.
    }
}
