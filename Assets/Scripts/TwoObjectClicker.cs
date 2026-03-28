using UnityEngine;
using UnityEngine.Events;

public class TwoObjectClicker : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;

    [Header("Raycast Settings")]
    public LayerMask clickLayer;

    [Header("Swap Materials Per Click")]
    public bool swapMaterials = false;

    public GameObject targetObject1;
    public GameObject targetObject2;
    public GameObject targetObject3;
    public GameObject targetObject4;
    public GameObject targetObject5;
    public GameObject targetObject6;

    public Material materialForTarget1;
    public Material materialForTarget2;

    public UnityEvent onBothClicked;

    private bool obj1Clicked = false;
    private bool obj2Clicked = false;

    void Start()
    {
        object1.SetActive(true);
        object2.SetActive(true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, clickLayer))
            {
                // ===== CLICK OBJECT 1 =====
                if (hit.collider.gameObject == object1 && !obj1Clicked)
                {
                    obj1Clicked = true;
                    object1.SetActive(false);

                    if (swapMaterials)
                    {
                        ApplyForObject1();
                    }
                }

                // ===== CLICK OBJECT 2 =====
                else if (hit.collider.gameObject == object2 && !obj2Clicked)
                {
                    obj2Clicked = true;
                    object2.SetActive(false);

                    if (swapMaterials)
                    {
                        ApplyForObject2();
                    }
                }

                // When both done
                if (obj1Clicked && obj2Clicked)
                {
                    onBothClicked.Invoke();
                }
            }
        }
    }

    void ApplyForObject1()
    {
        if (targetObject1 != null)
            targetObject1.GetComponent<Renderer>().material = materialForTarget2;

        if (targetObject3 != null)
            targetObject3.GetComponent<Renderer>().material = materialForTarget2;
            targetObject5.GetComponent<Renderer>().material = materialForTarget2;

    }

    void ApplyForObject2()
    {
        if (targetObject2 != null)
            targetObject2.GetComponent<Renderer>().material = materialForTarget1;

        if (targetObject4 != null)
            targetObject4.GetComponent<Renderer>().material = materialForTarget1    ;
            targetObject6.GetComponent<Renderer>().material = materialForTarget1;
    }
}
