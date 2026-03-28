using UnityEngine;

public class SplineToggle : MonoBehaviour
{
    public GameObject spline;
    public GameObject spline2;

    public void EnableSpline()
    {
        spline.SetActive(true);
    }

    public void DisableSpline()
    {
        spline.SetActive(false);
    }
    public void Reversespline()
    {
        spline2.SetActive(true);
    }
}
