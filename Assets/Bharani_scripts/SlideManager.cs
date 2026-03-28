using UnityEngine;
using System;

public class SlideManager : MonoBehaviour
{
    public static SlideManager Instance;

    [Header("Slides")]
    public SlideBehaviour[] slides;

    private int currentIndex = 0;
    private bool[] slideCompleted;

    public event Action<int> OnSlideChanged;       // C# event
    public event Action<int> OnSlideCompleted;     // C# event

    void Awake()
    {
        Instance = this;
        slideCompleted = new bool[slides.Length];
    }

    void Start()
    {
        ShowSlide(0);
    }

    public void ShowSlide(int index)
    {
        for (int i = 0; i < slides.Length; i++)
            slides[i].gameObject.SetActive(false);

        slides[index].gameObject.SetActive(true);
        currentIndex = index;

        OnSlideChanged?.Invoke(currentIndex);

        if (slideCompleted[index])
            slides[index].RestoreCompletedState();
    }

    public void NextSlide()
    {
        int next = currentIndex + 1;
        if (next < slides.Length)
            ShowSlide(next);
    }

    public void PreviousSlide()
    {
        int prev = currentIndex - 1;
        if (prev >= 0)
            ShowSlide(prev);
    }

    public void MarkSlideCompleted()
    {
        if (!slideCompleted[currentIndex])
        {
            slideCompleted[currentIndex] = true;
            OnSlideCompleted?.Invoke(currentIndex);
        }
    }

    public int GetCurrentIndex() => currentIndex;
}
