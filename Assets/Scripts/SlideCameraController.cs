using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class SlideCameraController : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public Transform cameraPoint;
        public GameObject slide;

        [TextArea(2, 5)]
        public string slideText;

        public UnityEvent onNextClicked;
        public UnityEvent onBackClicked;
        public UnityEvent onRepeatedNextClicked;
    }

    [Header("Camera")]
    public Transform cameraTransform;
    public float moveSpeed = 2f;

    [Header("Steps")]
    public List<Step> steps;

    [Header("UI")]
    public TextMeshProUGUI slideTextUI;
    public Button nextButton;
    public Button previousButton;

    [Header("Slide Counter UI")]
    public TextMeshProUGUI slideCounterText;

    [Header("Slide Counter Values")]
    public int currentSlide = 0;
    public int totalSlides = 0;

    [Header("Debug / Cheat")]
    public bool enableCheatKey = true;

    int currentIndex = 0;
    bool isMoving = false;
    bool[] slideCompleted;
    bool[] eventUsed;

    void Start()
    {
        slideCompleted = new bool[steps.Count];
        eventUsed = new bool[steps.Count];

        currentSlide = 0;
        UpdateSlideCounterUI();

        foreach (var s in steps)
            s.slide.SetActive(false);

        steps[0].slide.SetActive(true);

        cameraTransform.position = steps[0].cameraPoint.position;
        cameraTransform.rotation = steps[0].cameraPoint.rotation;

        nextButton.interactable = false;
        UpdateBackButton();
    }

    void Update()
    {
        // Old Input System cheat key
        if (enableCheatKey && Input.GetKeyDown(KeyCode.N))
        {
            ForceNextSlide_Cheat();
        }

#if ENABLE_INPUT_SYSTEM
        // New Input System cheat key
        if (enableCheatKey &&
            UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.nKey.wasPressedThisFrame)
        {
            ForceNextSlide_Cheat();
        }
#endif
    }

    // =======================================================
    // ⭐ ALWAYS WORKING CHEAT FUNCTION (Debug Next Slide)
    // =======================================================
    void ForceNextSlide_Cheat()
    {
        if (currentIndex >= steps.Count - 1)
            return;

        StopAllCoroutines();
        isMoving = false;

        slideCompleted[currentIndex] = true;
        nextButton.interactable = true;

        StartCoroutine(MoveTo(currentIndex + 1));
    }

    // Called from events to enable next
    public void EnableNextButton()
    {
        slideCompleted[currentIndex] = true;
        nextButton.interactable = true;
    }

    public void Next()
    {
        if (isMoving) return;
        if (!slideCompleted[currentIndex]) return;
        if (currentIndex >= steps.Count - 1) return;

        if (!eventUsed[currentIndex])
        {
            steps[currentIndex].onNextClicked?.Invoke();
            eventUsed[currentIndex] = true;
        }

        steps[currentIndex].onRepeatedNextClicked?.Invoke();

        StartCoroutine(MoveTo(currentIndex + 1));
    }

    public void Previous()
    {
        if (currentIndex <= 0) return;

        StopAllCoroutines();
        isMoving = false;

        steps[currentIndex].onBackClicked?.Invoke();

        StartCoroutine(MoveTo(currentIndex - 1));
    }

    IEnumerator MoveTo(int targetIndex)
    {
        isMoving = true;

        foreach (var s in steps)
            s.slide.SetActive(false);

        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        Vector3 endPos = steps[targetIndex].cameraPoint.position;
        Quaternion endRot = steps[targetIndex].cameraPoint.rotation;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            cameraTransform.position = Vector3.Lerp(startPos, endPos, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        currentIndex = targetIndex;
        currentSlide = currentIndex;

        UpdateSlideCounterUI();

        steps[currentIndex].slide.SetActive(true);

        nextButton.interactable = slideCompleted[currentIndex];
        UpdateBackButton();

        isMoving = false;
    }

    public void EnableNextSlideNextButton()
    {
        int nextIndex = currentIndex + 1;
        if (nextIndex < steps.Count)
        {
            slideCompleted[nextIndex] = true;
        }
    }

    void UpdateBackButton()
    {
        previousButton.interactable = currentIndex > 0;
    }

    void UpdateSlideCounterUI()
    {
        if (slideCounterText != null)
        {
            slideCounterText.text = (currentSlide + 1) + " / " + totalSlides;
        }
    }
}
