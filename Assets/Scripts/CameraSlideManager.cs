//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class CameraSlideManager : MonoBehaviour
//{
//    /* ================= CAMERA ================= */

//    [Header("Camera")]
//    public Transform cameraTransform;
//    public float moveSpeed = 1.5f;

//    [Header("Camera Movement Curve")]
//    public AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1);

//    /* ================= SLIDES ================= */

//    [Header("Slides (Order Matters)")]
//    public List<Transform> slidePoints;

//    /* ================= PER SLIDE OBJECT CONTROL ================= */

//    [System.Serializable]
//    public class SlideObjectControl
//    {
//        public List<GameObject> enableObjects;
//        public List<GameObject> disableObjects;
//    }

//    [Header("Per Slide Object Control (Optional)")]
//    public List<SlideObjectControl> slideObjectControls;

//    /* ================= UI ================= */

//    [Header("UI Buttons")]
//    public Button nextButton;
//    public Button previousButton;

//    [Header("Slide Indicator Text")]
//    public TextMeshProUGUI slideText;

//    /* ================= CLICKABLES ================= */

//    [Header("Clickable Objects (Current Slide)")]
//    public List<ClickableTracker> clickableObjects;

//    /* ================= INTERNAL ================= */

//    int currentIndex = 0;
//    bool isMoving = false;
//    bool[] slideCompleted;

//    /* ================= UNITY ================= */

//    void Start()
//    {
//        slideCompleted = new bool[slidePoints.Count];

//        if (nextButton) nextButton.interactable = false;
//        if (previousButton) previousButton.interactable = false;

//        if (nextButton) nextButton.onClick.AddListener(NextSlide);
//        if (previousButton) previousButton.onClick.AddListener(PreviousSlide);

//        MoveToSlide(0);
//    }

//    /* ================= PUBLIC API (IMPORTANT) ================= */

//    // 🔔 Called by external scripts (drag / enable logic)
//    public void MarkCurrentSlideCompleted()
//    {
//        slideCompleted[currentIndex] = true;

//        if (nextButton)
//            nextButton.interactable = true;
//    }

//    public int GetCurrentSlideIndex()
//    {
//        return currentIndex;
//    }

//    /* ================= SLIDE NAVIGATION ================= */

//    void NextSlide()
//    {
//        if (isMoving)
//            return;

//        if (currentIndex >= slidePoints.Count - 1)
//            return;

//        MoveToSlide(currentIndex + 1);
//    }

//    void PreviousSlide()
//    {
//        if (isMoving)
//            return;

//        if (currentIndex <= 0)
//            return;

//        MoveToSlide(currentIndex - 1);
//    }

//    void MoveToSlide(int targetIndex)
//    {
//        if (slidePoints == null || slidePoints.Count == 0)
//            return;

//        if (targetIndex < 0 || targetIndex >= slidePoints.Count)
//            return;

//        StartCoroutine(MoveCameraRoutine(slidePoints[targetIndex], targetIndex));
//    }

//    IEnumerator MoveCameraRoutine(Transform target, int targetIndex)
//    {
//        if (!target)
//            yield break;

//        isMoving = true;

//        // 🔒 Disable BOTH buttons during movement
//        if (nextButton) nextButton.interactable = false;
//        if (previousButton) previousButton.interactable = false;

//        Vector3 startPos = cameraTransform.position;
//        Quaternion startRot = cameraTransform.rotation;

//        float t = 0f;
//        while (t < 1f)
//        {
//            t += Time.deltaTime * moveSpeed;
//            float curveT = moveCurve.Evaluate(t);

//            cameraTransform.position = Vector3.Lerp(startPos, target.position, curveT);
//            cameraTransform.rotation = Quaternion.Slerp(startRot, target.rotation, curveT);

//            yield return null;
//        }

//        cameraTransform.position = target.position;
//        cameraTransform.rotation = target.rotation;

//        currentIndex = targetIndex;

//        ApplySlideObjectControl();
//        HandleSlideState();
//        UpdateSlideText();

//        // Previous button rule
//        if (previousButton)
//            previousButton.interactable = currentIndex > 0;

//        isMoving = false;
//    }

//    /* ================= SLIDE STATE HANDLING ================= */

//    void HandleSlideState()
//    {
//        // ✅ If slide already completed (click OR drag)
//        if (slideCompleted[currentIndex])
//        {
//            if (nextButton)
//                nextButton.interactable = true;
//            return;
//        }

//        // 🔹 Click-based slide
//        if (clickableObjects != null && clickableObjects.Count > 0)
//        {
//            foreach (var c in clickableObjects)
//            {
//                if (c != null)
//                    c.ResetClick();
//            }

//            if (nextButton)
//                nextButton.interactable = false;
//        }

//        // 🔹 Drag-based slide → CameraSlideManager does NOTHING
//        // Drag script will call MarkCurrentSlideCompleted()
//    }

//    /* ================= CLICKABLE CALLBACK ================= */

//    // 🔔 Called by ClickableTracker
//    public void NotifyObjectClicked()
//    {
//        if (clickableObjects == null || clickableObjects.Count == 0)
//            return;

//        foreach (var c in clickableObjects)
//        {
//            if (c == null || !c.IsClicked)
//                return;
//        }

//        // ✅ Mark slide completed
//        slideCompleted[currentIndex] = true;

//        if (nextButton)
//            nextButton.interactable = true;
//    }

//    /* ================= OBJECT ENABLE / DISABLE ================= */

//    void ApplySlideObjectControl()
//    {
//        if (slideObjectControls == null)
//            return;

//        if (currentIndex >= slideObjectControls.Count)
//            return;

//        SlideObjectControl control = slideObjectControls[currentIndex];

//        if (control.enableObjects != null)
//        {
//            foreach (var obj in control.enableObjects)
//                if (obj) obj.SetActive(true);
//        }

//        if (control.disableObjects != null)
//        {
//            foreach (var obj in control.disableObjects)
//                if (obj) obj.SetActive(false);
//        }
//    }

//    /* ================= UI ================= */

//    void UpdateSlideText()
//    {
//        if (!slideText)
//            return;

//        slideText.text = $"{currentIndex + 1} / {slidePoints.Count}";
//    }
//}

//OG Safe script
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class CameraSlideManager : MonoBehaviour
//{
//    /* ================= CAMERA ================= */

//    [Header("Camera")]
//    public Transform cameraTransform;
//    public float moveSpeed = 1.5f;

//    [Header("Camera Movement Curve")]
//    public AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1);

//    /* ================= SLIDES ================= */

//    [Header("Slides (Order Matters)")]
//    public List<Transform> slidePoints;

//    /* ================= PER SLIDE OBJECT CONTROL ================= */

//    [System.Serializable]
//    public class SlideObjectControl
//    {
//        public List<GameObject> enableObjects;
//        public List<GameObject> disableObjects;
//    }

//    [Header("Per Slide Object Control (Optional)")]
//    public List<SlideObjectControl> slideObjectControls;

//    /* ================= UI ================= */

//    [Header("UI Buttons")]
//    public Button nextButton;
//    public Button previousButton;

//    [Header("Slide Indicator Text")]
//    public TextMeshProUGUI slideText;

//    /* ================= CLICKABLES ================= */

//    [Header("Clickable Objects (Current Slide)")]
//    public List<ClickableTracker> clickableObjects;

//    /* ================= INTERNAL ================= */

//    int currentIndex = 0;
//    bool isMoving = false;
//    bool[] slideCompleted;

//    /* ================= UNITY ================= */

//    void Start()
//    {
//        slideCompleted = new bool[slidePoints.Count];

//        if (nextButton) nextButton.interactable = false;
//        if (previousButton) previousButton.interactable = false;

//        if (nextButton) nextButton.onClick.AddListener(NextSlide);
//        if (previousButton) previousButton.onClick.AddListener(PreviousSlide);

//        MoveToSlide(0);
//    }

//    /* ================= PUBLIC API (IMPORTANT) ================= */

//    // 🔔 Called by external scripts (drag / enable logic)
//    public void MarkCurrentSlideCompleted()
//    {
//        slideCompleted[currentIndex] = true;

//        if (nextButton)
//            nextButton.interactable = true;
//    }

//    public int GetCurrentSlideIndex()
//    {
//        return currentIndex;
//    }

//    /* ================= SLIDE NAVIGATION ================= */

//    void NextSlide()
//    {
//        if (isMoving)
//            return;

//        if (currentIndex >= slidePoints.Count - 1)
//            return;

//        MoveToSlide(currentIndex + 1);
//    }

//    void PreviousSlide()
//    {
//        if (isMoving)
//            return;

//        if (currentIndex <= 0)
//            return;

//        MoveToSlide(currentIndex - 1);
//    }

//    void MoveToSlide(int targetIndex)
//    {
//        if (slidePoints == null || slidePoints.Count == 0)
//            return;

//        if (targetIndex < 0 || targetIndex >= slidePoints.Count)
//            return;

//        StartCoroutine(MoveCameraRoutine(slidePoints[targetIndex], targetIndex));
//    }

//    IEnumerator MoveCameraRoutine(Transform target, int targetIndex)
//    {
//        if (!target)
//            yield break;

//        isMoving = true;

//        // 🔒 Disable BOTH buttons during movement
//        if (nextButton) nextButton.interactable = false;
//        if (previousButton) previousButton.interactable = false;

//        Vector3 startPos = cameraTransform.position;
//        Quaternion startRot = cameraTransform.rotation;

//        float t = 0f;
//        while (t < 1f)
//        {
//            t += Time.deltaTime * moveSpeed;
//            float curveT = moveCurve.Evaluate(t);

//            cameraTransform.position = Vector3.Lerp(startPos, target.position, curveT);
//            cameraTransform.rotation = Quaternion.Slerp(startRot, target.rotation, curveT);

//            yield return null;
//        }

//        cameraTransform.position = target.position;
//        cameraTransform.rotation = target.rotation;

//        currentIndex = targetIndex;

//        ApplySlideObjectControl();
//        HandleSlideState();
//        UpdateSlideText();

//        // Previous button rule
//        if (previousButton)
//            previousButton.interactable = currentIndex > 0;

//        isMoving = false;
//    }

//    /* ================= SLIDE STATE HANDLING ================= */

//    void HandleSlideState()
//    {
//        // ✅ If slide already completed (click OR drag)
//        if (slideCompleted[currentIndex])
//        {
//            if (nextButton)
//                nextButton.interactable = true;
//            return;
//        }

//        // 🔹 Click-based slide
//        if (clickableObjects != null && clickableObjects.Count > 0)
//        {
//            foreach (var c in clickableObjects)
//            {
//                if (c != null)
//                    c.ResetClick();
//            }

//            if (nextButton)
//                nextButton.interactable = false;
//        }

//        // 🔹 Drag-based slide → CameraSlideManager does NOTHING
//        // Drag script will call MarkCurrentSlideCompleted()
//    }

//    /* ================= CLICKABLE CALLBACK ================= */

//    // 🔔 Called by ClickableTracker
//    public void NotifyObjectClicked()
//    {
//        if (clickableObjects == null || clickableObjects.Count == 0)
//            return;

//        foreach (var c in clickableObjects)
//        {
//            if (c == null || !c.IsClicked)
//                return;
//        }

//        // ✅ Mark slide completed
//        slideCompleted[currentIndex] = true;

//        if (nextButton)
//            nextButton.interactable = true;
//    }

//    /* ================= OBJECT ENABLE / DISABLE ================= */

//    void ApplySlideObjectControl()
//    {
//        if (slideObjectControls == null)
//            return;

//        if (currentIndex >= slideObjectControls.Count)
//            return;

//        SlideObjectControl control = slideObjectControls[currentIndex];

//        if (control.enableObjects != null)
//        {
//            foreach (var obj in control.enableObjects)
//                if (obj) obj.SetActive(true);
//        }

//        if (control.disableObjects != null)
//        {
//            foreach (var obj in control.disableObjects)
//                if (obj) obj.SetActive(false);
//        }
//    }

//    /* ================= UI ================= */

//    void UpdateSlideText()
//    {
//        if (!slideText)
//            return;

//        slideText.text = $"{currentIndex + 1} / {slidePoints.Count}";
//    }

//    public void RestoreButtonStateAfterExternalCameraMove()
//    {
//        // Previous button rule
//        if (previousButton)
//            previousButton.interactable = currentIndex > 0;

//        // Next button rule
//        if (slideCompleted[currentIndex] && nextButton)
//            nextButton.interactable = true;
//    }

//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class CameraSlideManager : MonoBehaviour
//{
//    /* ================= CAMERA ================= */

//    [Header("Camera")]
//    public Transform cameraTransform;
//    public float moveSpeed = 1.5f;
//    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

//    /* ================= SLIDES ================= */

//    [Header("Camera Slide Points (Order Matters)")]
//    public List<Transform> slidePoints;

//    /* ================= UI ================= */

//    [Header("UI Buttons")]
//    public Button nextButton;
//    public Button previousButton;

//    [Header("Slide Indicator (Optional)")]
//    public TextMeshProUGUI slideText;

//    /* ================= INTERNAL ================= */

//    int currentIndex = 0;
//    bool isMoving = false;

//    // 🔒 Stores which slides are completed
//    HashSet<int> completedSlides = new HashSet<int>();

//    Coroutine moveRoutine;

//    /* ================= UNITY ================= */

//    void Start()
//    {
//        // Initial button states
//        if (nextButton) nextButton.interactable = false;
//        if (previousButton) previousButton.interactable = false;

//        if (nextButton) nextButton.onClick.AddListener(NextSlide);
//        if (previousButton) previousButton.onClick.AddListener(PreviousSlide);

//        // Move to first slide automatically
//        MoveToSlide(0);
//    }

//    /* ================= PUBLIC API (THIS IS THE KEY) ================= */

//    /// <summary>
//    /// 🔥 CALL THIS FROM ANY SCRIPT / BUTTON
//    /// Marks the current slide as completed
//    /// </summary>
//    public void MarkCurrentSlideCompleted()
//    {
//        if (!completedSlides.Contains(currentIndex))
//            completedSlides.Add(currentIndex);

//        EnableNextIfAllowed();
//    }

//    /* ================= SLIDE NAVIGATION ================= */

//    void NextSlide()
//    {
//        if (isMoving) return;
//        if (currentIndex >= slidePoints.Count - 1) return;

//        MoveToSlide(currentIndex + 1);
//    }

//    void PreviousSlide()
//    {
//        if (isMoving) return;
//        if (currentIndex <= 0) return;

//        MoveToSlide(currentIndex - 1);
//    }

//    void MoveToSlide(int targetIndex)
//    {
//        if (slidePoints == null || slidePoints.Count == 0) return;
//        if (targetIndex < 0 || targetIndex >= slidePoints.Count) return;

//        if (moveRoutine != null)
//            StopCoroutine(moveRoutine);

//        moveRoutine = StartCoroutine(MoveCameraRoutine(slidePoints[targetIndex], targetIndex));
//    }

//    IEnumerator MoveCameraRoutine(Transform target, int targetIndex)
//    {
//        isMoving = true;

//        // 🔒 Disable buttons during movement (PROTECTION ONLY)
//        if (nextButton) nextButton.interactable = false;
//        if (previousButton) previousButton.interactable = false;

//        Vector3 startPos = cameraTransform.position;
//        Quaternion startRot = cameraTransform.rotation;

//        float t = 0f;
//        while (t < 1f)
//        {
//            t += Time.deltaTime * moveSpeed;
//            float curveT = moveCurve.Evaluate(t);

//            cameraTransform.position =
//                Vector3.Lerp(startPos, target.position, curveT);

//            cameraTransform.rotation =
//                Quaternion.Slerp(startRot, target.rotation, curveT);

//            yield return null;
//        }

//        cameraTransform.position = target.position;
//        cameraTransform.rotation = target.rotation;

//        currentIndex = targetIndex;

//        UpdateButtonsAfterMove();
//        UpdateSlideText();

//        isMoving = false;
//    }

//    /* ================= BUTTON LOGIC ================= */

//    void UpdateButtonsAfterMove()
//    {
//        // Previous button logic
//        if (previousButton)
//            previousButton.interactable = currentIndex > 0;

//        // Next button logic (ONLY manager decides)
//        EnableNextIfAllowed();
//    }

//    void EnableNextIfAllowed()
//    {
//        if (nextButton == null)
//            return;

//        // Enable ONLY if this slide is completed
//        nextButton.interactable = completedSlides.Contains(currentIndex);
//    }

//    /* ================= UI ================= */

//    void UpdateSlideText()
//    {
//        if (!slideText) return;
//        slideText.text = $"{currentIndex + 1} / {slidePoints.Count}";
//    }

//    /* ================= DEBUG / INFO ================= */

//    public int GetCurrentSlideIndex()
//    {
//        return currentIndex;
//    }

//    public bool IsSlideCompleted(int index)
//    {
//        return completedSlides.Contains(index);
//    }
//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraSlideManager : MonoBehaviour
{
    /* ================= CAMERA ================= */

    [Header("Camera")]
    public Transform cameraTransform;
    public float moveSpeed = 1.5f;

    [Header("Camera Movement Curve")]
    public AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1);

    /* ================= SLIDES ================= */

    [Header("Slides (Order Matters)")]
    public List<Transform> slidePoints;

    /* ================= PER SLIDE OBJECT CONTROL ================= */

    [System.Serializable]
    public class SlideObjectControl
    {
        public List<GameObject> enableObjects;
        public List<GameObject> disableObjects;
    }

    [Header("Per Slide Object Control (Optional)")]
    public List<SlideObjectControl> slideObjectControls;

    /* ================= UI ================= */

    [Header("UI Buttons")]
    public Button nextButton;
    public Button previousButton;

    [Header("Slide Indicator Text")]
    public TextMeshProUGUI slideText;

    /* ================= CLICKABLES ================= */

    [Header("Clickable Objects (Current Slide)")]
    public List<ClickableTracker> clickableObjects;

    /* ================= INTERNAL ================= */

    int currentIndex = 0;
    bool isMoving = false;

    // 🔒 Slide completion memory (CLICK / DRAG / DIAL PAD)
    bool[] slideCompleted;

    Coroutine moveRoutine;

    /* ================= UNITY ================= */

    void Start()
    {
        slideCompleted = new bool[slidePoints.Count];

        if (nextButton) nextButton.interactable = false;
        if (previousButton) previousButton.interactable = false;

        if (nextButton) nextButton.onClick.AddListener(NextSlide);
        if (previousButton) previousButton.onClick.AddListener(PreviousSlide);

        MoveToSlide(0);
    }

    /* ================= PUBLIC API ================= */

    // 🔔 CALLED BY DRAG / DIAL PAD / EXTERNAL LOGIC
    public void MarkCurrentSlideCompleted()
    {
        slideCompleted[currentIndex] = true;

        // Enable next immediately if not moving
        if (!isMoving && nextButton)
            nextButton.interactable = true;
    }

    public int GetCurrentSlideIndex()
    {
        return currentIndex;
    }

    /* ================= SLIDE NAVIGATION ================= */

    void NextSlide()
    {
        if (isMoving) return;
        if (currentIndex >= slidePoints.Count - 1) return;

        MoveToSlide(currentIndex + 1);
    }

    void PreviousSlide()
    {
        if (isMoving) return;
        if (currentIndex <= 0) return;

        MoveToSlide(currentIndex - 1);
    }

    void MoveToSlide(int targetIndex)
    {
        if (slidePoints == null || slidePoints.Count == 0) return;
        if (targetIndex < 0 || targetIndex >= slidePoints.Count) return;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveCameraRoutine(slidePoints[targetIndex], targetIndex));
    }

    IEnumerator MoveCameraRoutine(Transform target, int targetIndex)
    {
        if (!target)
            yield break;

        isMoving = true;

        // 🔒 Disable buttons ONLY during movement
        if (nextButton) nextButton.interactable = false;
        if (previousButton) previousButton.interactable = false;

        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            float curveT = moveCurve.Evaluate(t);

            cameraTransform.position =
                Vector3.Lerp(startPos, target.position, curveT);

            cameraTransform.rotation =
                Quaternion.Slerp(startRot, target.rotation, curveT);

            yield return null;
        }

        cameraTransform.position = target.position;
        cameraTransform.rotation = target.rotation;

        currentIndex = targetIndex;

        ApplySlideObjectControl();
        HandleSlideState();
        UpdateSlideText();

        // Previous button rule
        if (previousButton)
            previousButton.interactable = currentIndex > 0;

        isMoving = false;
    }

    /* ================= SLIDE STATE HANDLING ================= */

    void HandleSlideState()
    {
        // ✅ If slide already completed → Next stays enabled
        if (slideCompleted[currentIndex])
        {
            if (nextButton)
                nextButton.interactable = true;
            return;
        }

        // 🔹 Click-based slide logic
        if (clickableObjects != null && clickableObjects.Count > 0)
        {
            foreach (var c in clickableObjects)
            {
                if (c != null)
                    c.ResetClick();
            }

            if (nextButton)
                nextButton.interactable = false;
        }

        // 🔹 Drag / DialPad slides:
        // They will explicitly call MarkCurrentSlideCompleted()
    }

    /* ================= CLICKABLE CALLBACK ================= */

    // 🔔 Called by ClickableTracker (OG behavior preserved)
    public void NotifyObjectClicked()
    {
        if (clickableObjects == null || clickableObjects.Count == 0)
            return;

        foreach (var c in clickableObjects)
        {
            if (c == null || !c.IsClicked)
                return;
        }

        slideCompleted[currentIndex] = true;

        if (nextButton)
            nextButton.interactable = true;
    }

    /* ================= OBJECT ENABLE / DISABLE ================= */

    void ApplySlideObjectControl()
    {
        if (slideObjectControls == null) return;
        if (currentIndex >= slideObjectControls.Count) return;

        SlideObjectControl control = slideObjectControls[currentIndex];

        if (control.enableObjects != null)
        {
            foreach (var obj in control.enableObjects)
                if (obj) obj.SetActive(true);
        }

        if (control.disableObjects != null)
        {
            foreach (var obj in control.disableObjects)
                if (obj) obj.SetActive(false);
        }
    }

    /* ================= UI ================= */

    void UpdateSlideText()
    {
        if (!slideText) return;
        slideText.text = $"{currentIndex + 1} / {slidePoints.Count}";
    }

    /* ================= EXTERNAL CAMERA SUPPORT ================= */

    // 🔔 CALLED BY CAMERA ZOOM / DRAG CAMERA SCRIPTS
    public void RestoreButtonStateAfterExternalCameraMove()
    {
        if (previousButton)
            previousButton.interactable = currentIndex > 0;

        if (slideCompleted[currentIndex] && nextButton)
            nextButton.interactable = true;
    }

    public void EnableNextButton()
    {
        nextButton.gameObject.SetActive(true);
    }
}



