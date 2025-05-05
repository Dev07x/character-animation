using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    //public Slider healthSlider;
    public UnityEngine.UI.Slider healthSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetSlider(float value)
    {
        healthSlider.value = value;
    }

    public void SetSliderMax(float value)
    {
        healthSlider.maxValue = value; // Corrected property name

        SetSlider(value);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
