using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Enable_Disable : MonoBehaviour
{
    public bool sliderEnabled;
    public GameObject slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SliderEnable()
    {
        if (sliderEnabled)
        {
            sliderEnabled = false;
            slider.gameObject.SetActive(false);
        }
        else if (!sliderEnabled)
        {
            sliderEnabled = true;
            slider.gameObject.SetActive(true);
        }
    }
}
