using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class TimeBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var slider = this.GetComponent<Slider> ();
        slider.minValue = 0;
        slider.maxValue = 4096;
        slider.value = slider.maxValue;
    }

    public void setBarMaxValue(){
        var slider = this.GetComponent<Slider> ();
        slider.value = slider.value = slider.maxValue;
    }

    public void DecreaseTurnTimer(int val){
        this.GetComponent<Slider> ().value -= val;
    }

    public bool IsEmpty(){
        var slider = this.GetComponent<Slider> ();
        return slider.value == slider.minValue;
    }

    // Update is called once per frame
    void Update()
    {

    }
}