using UnityEngine;
using UnityEngine.UI;

public class performAction : MonoBehaviour {

    [Header("Dependencies")]
    public Slider scaleSlider;

    public void setState(int state){
        FindObjectOfType<setupScene>().setState(state);
    }

    public void setSliderValue(){
        FindObjectOfType<setupScene>().setScale((int)scaleSlider.value);
    }

    public void noCalibration(){
        FindObjectOfType<setupScene>().noCalibration();
    }
}
