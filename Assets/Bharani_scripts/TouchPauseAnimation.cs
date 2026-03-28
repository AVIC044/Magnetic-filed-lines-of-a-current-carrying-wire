using UnityEngine;

public class TouchPauseAnimation : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Animation Clips To Control")]
    public string[] animationNames;

    [Header("Objects To Hide While Touching or Dragging")]
    public GameObject[] objectsToHide;

    private bool playingFromEvent = false;
    private string currentAnimation = "";
    private bool isTouching = false;

    void Update()
    {
        if (!playingFromEvent)
            return;

        // -----------------------------------------------------
        // ⭐ DETECT TOUCH OR DRAG (Mouse + Mobile)
        // -----------------------------------------------------
        bool touching = Input.GetMouseButton(0) || Input.touchCount > 0;

        // TOUCH START
        if (touching && !isTouching)
        {
            isTouching = true;

            // ⭐ STOP animation instantly
            animator.speed = 0f;

            // ⭐ Hide selected objects
            HideObjects();
        }

        // TOUCH END
        if (!touching && isTouching)
        {
            isTouching = false;

            // ⭐ Resume animation
            animator.speed = 1f;

            // ⭐ Show selected objects
            ShowObjects();
        }
    }

    // -----------------------------------------------------
    // ⭐ CALL THIS FROM EVENTS TO START ANIMATION
    // -----------------------------------------------------
    public void PlayAnimation(int animationIndex)
    {
        if (animationIndex < 0 || animationIndex >= animationNames.Length)
        {
            Debug.LogWarning("Invalid animation index");
            return;
        }

        currentAnimation = animationNames[animationIndex];
        playingFromEvent = true;

        animator.speed = 1f;
        animator.Play(currentAnimation, 0, 0f);

        // Always show objects when animation starts
        ShowObjects();
    }

    // -----------------------------------------------------
    // ⭐ CALL THIS TO STOP ANIMATION COMPLETELY
    // -----------------------------------------------------
    public void StopAnimation()
    {
        playingFromEvent = false;
        animator.speed = 0f;

        // Make sure objects are visible when stopped
        ShowObjects();
    }

    // -----------------------------------------------------
    // Helpers
    // -----------------------------------------------------
    void HideObjects()
    {
        foreach (var obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    void ShowObjects()
    {
        foreach (var obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }
}
