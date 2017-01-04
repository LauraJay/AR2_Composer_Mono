using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonPressed : MonoBehaviour {
    public GameObject Pivot;
    public GameObject xHandle;
    public GameObject yHandle;
    public GameObject zHandle;
    public GameObject PinchDetector;
        private bool xButtonIsPressed;
	// Use this for initialization
	void Start () {
        xButtonIsPressed = false;
	}

  public void isPressed()
    {
        //Pivot.GetComponent<MarkerScale>().xIsPressed = true;
        xButtonIsPressed = true;
    }
    public void isNotPressed()
    {
        // Pivot.GetComponent<MarkerScale>().xIsPressed = false;
        xButtonIsPressed = false;
    }
    // Update is called once per frame
    void Update () {
		if(xButtonIsPressed)
        {
          // xHandle.transform.position = new Vector3(PinchDetector.transform.position.x, , xHandle.transform.position.z);
            xHandle.transform.position = (new Vector3(xHandle.transform.position.x ,xHandle.transform.position.y,PinchDetector.transform.position.z) );
        }
	}
}
