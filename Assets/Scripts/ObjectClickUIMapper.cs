using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class ObjectClickUIMapper : MonoBehaviour
{
    [System.Serializable]
    public class ObjectUIMap
    {
        public GameObject object3D;
        public Image uiImage;
        public Image componentImage;

        [TextArea(3, 6)]
        public string infoText;

        [Header("✔️ Right / ❌ Wrong")]
        public bool isRight;
        public bool isWrong;
    }

    public Camera cam;

    [Header("Mappings")]
    public List<ObjectUIMap> mappings;

    [Header("Common Info Panel")]
    public GameObject infoPanel;
    public TMP_Text infoTextUI;

    [Header("🔥 Global Events")]
    public UnityEvent onRightObjectClicked;    // ✔ Fire when any RIGHT object is clicked
    public UnityEvent onWrongObjectClicked;    // ❌ Fire when any WRONG object is clicked
    public UnityEvent onAllObjectsClicked;     // 🎯 When all objects are clicked

    Dictionary<GameObject, ObjectUIMap> lookup;
    HashSet<GameObject> clickedObjects = new();

    bool eventFired = false;

    void Awake()
    {
        lookup = new Dictionary<GameObject, ObjectUIMap>();

        foreach (var map in mappings)
        {
            if (map.object3D != null)
            {
                lookup[map.object3D] = map;

                if (map.uiImage != null)
                    map.uiImage.gameObject.SetActive(false);

                if (map.componentImage != null)
                    map.componentImage.gameObject.SetActive(false);
            }
        }

        infoPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (lookup.TryGetValue(hit.collider.gameObject, out ObjectUIMap clicked))
                {
                    // 🟢 Enable UI visuals
                    if (clicked.uiImage != null)
                        clicked.uiImage.gameObject.SetActive(true);

                    if (clicked.componentImage != null)
                        clicked.componentImage.gameObject.SetActive(true);

                    // 📝 Update info panel
                    infoTextUI.text = clicked.infoText;
                    infoPanel.SetActive(true);

                    // 🔥 Track clicked object
                    clickedObjects.Add(clicked.object3D);

                    // ======================
                    // ✔ RIGHT / ❌ WRONG LOGIC
                    // ======================
                    if (clicked.isRight)
                    {
                        onRightObjectClicked?.Invoke();
                    }
                    else if (clicked.isWrong)
                    {
                        onWrongObjectClicked?.Invoke();
                    }

                    // ================================
                    // 🎯 FIRE EVENT WHEN ALL VISITED
                    // ================================
                    if (!eventFired && clickedObjects.Count == mappings.Count)
                    {
                        eventFired = true;
                        onAllObjectsClicked?.Invoke();
                    }
                }
            }
        }
    }
}
