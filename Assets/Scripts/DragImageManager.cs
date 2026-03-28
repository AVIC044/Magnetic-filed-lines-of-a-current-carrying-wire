using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIDragFilteredRaycastManager : MonoBehaviour
{
    [System.Serializable]
    public class DragData
    {
        public int index;

        [Header("UI")]
        public Image draggableImage;
        public GameObject uiContainer;

        [Header("Camera Target")]
        public Transform cameraPoint;

        [Header("Drop Indicator (3D target)")]
        public List<GameObject> objectsToDisable;

        [Header("Result Objects")]
        public List<GameObject> objectsToEnable;

        [Header("Symbol Image")]
        public Image symbolImage;

        [Header("Drag Scale (Float)")]
        public float dragScale = 1f;
    }

    [Header("Raycast Camera")]
    public Camera mainCamera;

    [Header("Camera Move")]
    public Transform cameraTransform;
    public float cameraMoveSpeed = 3f;

    [Header("Event Camera Targets")]
    public Transform backAtFirstCameraTarget;
    public Transform nextAtLastCameraTarget;

    [Header("Drag Data (ORDER MATTERS)")]
    public List<DragData> dragDataList;

    [Header("Buttons")]
    public Button nextButton;
    public Button backButton;

    [Header("Enable when 2 drops succeed")]
    public GameObject extraContainer1;
    public GameObject extraContainer2;

    [Header("Events")]
    public UnityEvent onLastStepCompleted;
    public UnityEvent onBackAtFirstElement;
    public UnityEvent onNextAtLastElement;

    [Header("Event on EACH Successful Drag")]
    public UnityEvent onEachDropSuccess;

    [Header("Drag Layer (top canvas layer for dragging)")]
    public RectTransform dragLayer;

    int successDropCount = 0;
    bool extrasEnabled = false;
    Coroutine camRoutine;

    Dictionary<Image, Vector2> startPositions = new();
    Dictionary<Image, Vector3> originalScales = new();
    HashSet<int> completedSteps = new HashSet<int>();

    Dictionary<Image, Transform> originalParents = new();

    // Fix scale when switching parent
    void MatchScaleAcrossCanvases(RectTransform rt, Transform newParent)
    {
        Vector3 worldScale = rt.lossyScale;
        rt.SetParent(newParent, false);
        Vector3 parentScale = newParent.lossyScale;

        rt.localScale = new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z
        );
    }

    IEnumerator MoveThenInvoke(Transform target, UnityEvent evt)
    {
        if (target != null)
            yield return MoveRoutine(target);

        evt?.Invoke();
    }

    void Start()
    {
        dragDataList = dragDataList.OrderBy(d => d.index).ToList();

        foreach (var data in dragDataList)
        {
            SetupDragEvents(data);
            originalScales[data.draggableImage] = data.draggableImage.rectTransform.localScale;
        }

        if (extraContainer1) extraContainer1.SetActive(false);
        if (extraContainer2) extraContainer2.SetActive(false);

        nextButton.onClick.AddListener(OnNextClicked);
        backButton.onClick.AddListener(OnBackClicked);

        UpdateNextButtonState();
    }

    void SetupDragEvents(DragData data)
    {
        EventTrigger trigger = data.draggableImage.GetComponent<EventTrigger>();
        if (!trigger) trigger = data.draggableImage.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        AddEvent(trigger, EventTriggerType.BeginDrag, () => OnBeginDrag(data));
        AddEvent(trigger, EventTriggerType.Drag, () => OnDrag(data));
        AddEvent(trigger, EventTriggerType.EndDrag, () => OnEndDrag(data));
    }

    void AddEvent(EventTrigger trigger, EventTriggerType type, UnityAction action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(_ => action());
        trigger.triggers.Add(entry);
    }

    // --------------------------------------------------------------
    // ✔ DRAG SYSTEM WITH SCALE 3 / ORIGINAL + DRAGLAYER
    // --------------------------------------------------------------

    void OnBeginDrag(DragData data)
    {
        RectTransform rt = data.draggableImage.rectTransform;

        if (!startPositions.ContainsKey(data.draggableImage))
            startPositions[data.draggableImage] = rt.anchoredPosition;

        GetCanvasGroup(data.draggableImage).blocksRaycasts = false;

        // ⭐ SCALE = 3 WHILE DRAGGING
        rt.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        // store parent
        originalParents[data.draggableImage] = rt.parent;

        // move to dragLayer with scale fix
        MatchScaleAcrossCanvases(rt, dragLayer);

        rt.SetAsLastSibling();
    }

    void OnDrag(DragData data)
    {
        data.draggableImage.rectTransform.position = Input.mousePosition;
    }

    void OnEndDrag(DragData data)
    {
        CanvasGroup cg = GetCanvasGroup(data.draggableImage);
        cg.blocksRaycasts = true;

        // ⭐ RESTORE ORIGINAL SCALE
        data.draggableImage.rectTransform.localScale =
            originalScales[data.draggableImage];

        // move back to original parent
        MatchScaleAcrossCanvases(
            data.draggableImage.rectTransform,
            originalParents[data.draggableImage]
        );

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit) &&
            data.objectsToDisable.Contains(hit.collider.gameObject))
        {
            foreach (var obj in data.objectsToEnable)
                obj?.SetActive(true);

            foreach (var obj in data.objectsToDisable)
                if (obj != null) Destroy(obj);

            HandleSuccessfulDrop(data);
            return;
        }

        SnapBack(data);
    }

    void SnapBack(DragData data)
    {
        RectTransform rt = data.draggableImage.rectTransform;

        rt.anchoredPosition = startPositions[data.draggableImage];

        // ⭐ RESTORE ORIGINAL SCALE
        rt.localScale = originalScales[data.draggableImage];
    }

    void HandleSuccessfulDrop(DragData data)
    {
        successDropCount++;
        completedSteps.Add(data.index);

        nextButton.interactable = true;

        data.draggableImage.gameObject.SetActive(false);
        data.uiContainer?.SetActive(false);

        if (data.symbolImage != null)
            data.symbolImage.gameObject.SetActive(false);

        onEachDropSuccess?.Invoke();

        if (!extrasEnabled && successDropCount == 2)
        {
            extrasEnabled = true;
            extraContainer1?.SetActive(true);
            extraContainer2?.SetActive(true);
        }

        if (completedSteps.Count == dragDataList.Count)
            onLastStepCompleted?.Invoke();
    }

    void OnNextClicked()
    {
        if (camRoutine != null)
            StopCoroutine(camRoutine);

        camRoutine = StartCoroutine(MoveThenInvoke(nextAtLastCameraTarget, onNextAtLastElement));
    }

    void OnBackClicked()
    {
        if (camRoutine != null)
            StopCoroutine(camRoutine);

        camRoutine = StartCoroutine(MoveThenInvoke(backAtFirstCameraTarget, onBackAtFirstElement));
    }

    IEnumerator MoveRoutine(Transform target)
    {
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * cameraMoveSpeed;
            cameraTransform.position = Vector3.Lerp(startPos, target.position, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, target.rotation, t);
            yield return null;
        }
    }

    void UpdateNextButtonState()
    {
        nextButton.interactable = completedSteps.Count > 0;
    }

    CanvasGroup GetCanvasGroup(Image img)
    {
        CanvasGroup cg = img.GetComponent<CanvasGroup>();
        if (!cg) cg = img.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }
}
