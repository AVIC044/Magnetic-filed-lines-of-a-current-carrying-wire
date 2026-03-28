using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SingleDragDropManager : MonoBehaviour
{
    [Header("Draggable Object")]
    public Transform draggableObject;

    [Header("Drop Zones (4)")]
    public Collider[] dropZones = new Collider[4];

    [Header("Correct Drop Zone Index (0-3)")]
    public int correctZoneIndex = 0;

    [Header("One Image Per Zone")]
    public GameObject[] zoneImages = new GameObject[4];

    [Header("Result Objects (NEW)")]
    public GameObject correctObject;
    public GameObject wrongObject;

    [Header("Buttons")]
    public Button checkButton;
    public Button retryButton;

    [Header("Events (NEW)")]
    public UnityEvent onCorrect;
    public UnityEvent onWrong;

    // 🔥 NEW EVENTS YOU ASKED FOR (ONLY ADDED)
    [Header("Check Button Events (NEW)")]
    public UnityEvent onCheckCorrect;
    public UnityEvent onCheckWrong;

    private Vector3 startPos;
    private Camera cam;
    private bool isDragging;

    private int currentZoneIndex = -1;
    private int lastShownImageIndex = -1;
    private int lastEventZoneIndex = -1;

    void Start()
    {
        cam = Camera.main;
        startPos = draggableObject.position;

        checkButton.onClick.AddListener(CheckAnswer);
        retryButton.onClick.AddListener(Retry);

        checkButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);

        if (correctObject) correctObject.SetActive(false);
        if (wrongObject) wrongObject.SetActive(false);

        HideAllImages();
    }

    void Update()
    {
        HandleDragging();
        DetectDropZone();
    }

    // ---------------- DRAGGING ----------------
    void HandleDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == draggableObject)
                {
                    isDragging = true;
                    currentZoneIndex = -1;
                    lastEventZoneIndex = -1;
                    checkButton.gameObject.SetActive(false);
                    HideCurrentImage();
                    HideResults();
                }
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, startPos);

            if (plane.Raycast(ray, out float dist))
            {
                Vector3 point = ray.GetPoint(dist);
                draggableObject.position = new Vector3(point.x, startPos.y, point.z);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            if (currentZoneIndex == -1)
            {
                Retry();
            }
        }
    }

    // ---------------- DETECT ZONE ----------------
    void DetectDropZone()
    {
        int detectedZone = -1;

        for (int i = 0; i < dropZones.Length; i++)
        {
            if (dropZones[i].bounds.Contains(draggableObject.position))
            {
                detectedZone = i;
                break;
            }
        }

        if (detectedZone != -1 && detectedZone != lastEventZoneIndex)
        {
            if (detectedZone == correctZoneIndex)
                onCorrect?.Invoke();
            else
                onWrong?.Invoke();

            lastEventZoneIndex = detectedZone;
        }

        if (detectedZone != -1 && currentZoneIndex == -1)
            checkButton.gameObject.SetActive(true);

        currentZoneIndex = detectedZone;

        if (currentZoneIndex == -1)
            checkButton.gameObject.SetActive(false);
    }

    // ---------------- CHECK ----------------
    void CheckAnswer()
    {
        checkButton.gameObject.SetActive(false);

        ShowZoneImage(currentZoneIndex);

        if (currentZoneIndex == correctZoneIndex)
        {
            retryButton.gameObject.SetActive(false);
            if (correctObject) correctObject.SetActive(true);
            if (wrongObject) wrongObject.SetActive(false);

            // 🔥 NEW EVENT (ONLY WHEN CHECK BUTTON PRESSED)
            onCheckCorrect?.Invoke();
        }
        else
        {
            retryButton.gameObject.SetActive(true);
            if (wrongObject) wrongObject.SetActive(true);
            if (correctObject) correctObject.SetActive(false);

            // 🔥 NEW EVENT (ONLY WHEN CHECK BUTTON PRESSED)
            onCheckWrong?.Invoke();
        }
    }

    void ShowZoneImage(int index)
    {
        HideAllImages();
        zoneImages[index].SetActive(true);
        lastShownImageIndex = index;
    }

    // ---------------- RETRY ----------------
    void Retry()
    {
        draggableObject.position = startPos;
        currentZoneIndex = -1;
        lastEventZoneIndex = -1;

        retryButton.gameObject.SetActive(false);
        checkButton.gameObject.SetActive(false);

        HideCurrentImage();
        HideResults();
    }

    void HideAllImages()
    {
        for (int i = 0; i < zoneImages.Length; i++)
            zoneImages[i].SetActive(false);

        lastShownImageIndex = -1;
    }

    void HideCurrentImage()
    {
        if (lastShownImageIndex != -1)
            zoneImages[lastShownImageIndex].SetActive(false);

        lastShownImageIndex = -1;
    }

    void HideResults()
    {
        if (correctObject) correctObject.SetActive(false);
        if (wrongObject) wrongObject.SetActive(false);
    }
}
