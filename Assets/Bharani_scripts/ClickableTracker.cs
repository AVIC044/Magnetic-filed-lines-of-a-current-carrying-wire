using UnityEngine;

public class ClickableTracker : MonoBehaviour
{
    [SerializeField] CameraSlideManager manager;

    public bool IsClicked { get; private set; }

    void OnMouseDown()
    {
        if (IsClicked) return;

        IsClicked = true;
        manager.NotifyObjectClicked();
    }

    public void ResetClick()
    {
        IsClicked = false;
    }

    public void ForceClicked()
    {
        IsClicked = true;
    }
}
