using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonPressed : MonoBehaviour {
    public GameObject Pivot;
    public GameObject xHandle;
    public GameObject yHandle;
    public GameObject zHandle;
    public GameObject PinchDetector;
    Vector3 xHandlePosition;
        private bool xButtonIsPressed;
    Vector3 yHandlePosition;
    private bool yButtonIsPressed;
    // Use this for initialization
    void Start () {
        xButtonIsPressed = false;
        yButtonIsPressed = false;
	}

  public void xIsPressed()
    {
        //Pivot.GetComponent<MarkerScale>().xIsPressed = true;
        xButtonIsPressed = true;
        xHandlePosition = xHandle.transform.localPosition;
    }
    public void yIsPressed()
    {
        //Pivot.GetComponent<MarkerScale>().xIsPressed = true;
        yButtonIsPressed = true;
        yHandlePosition = yHandle.transform.localPosition;
    }
    public void yIsNotPressed()
    {
         yButtonIsPressed = false;
        //xButtonIsPressed = false;
    }
    public void xIsNotPressed()
    {
        // Pivot.GetComponent<MarkerScale>().xIsPressed = false;
        xButtonIsPressed = false;
    }
    // Update is called once per frame
    void Update () {
        if (xButtonIsPressed) { 

              xHandle.transform.position = (PinchDetector.transform.position);//(new Vector3(yHandlePosition.x,yHandlePosition.y, yHandle.transform.InverseTransformPoint(PinchDetector.transform.position).x));
             xHandle.transform.localPosition = new Vector3(xHandle.transform.localPosition.x , xHandlePosition.y, xHandlePosition.z);
      }
        if (yButtonIsPressed)
        {
            // xHandle.transform.position = new Vector3(PinchDetector.transform.position.x, , xHandle.transform.position.z);

                yHandle.transform.position = (PinchDetector.transform.position);//(new Vector3(yHandlePosition.x,yHandlePosition.y, yHandle.transform.InverseTransformPoint(PinchDetector.transform.position).x));
                yHandle.transform.localPosition = new Vector3(yHandlePosition.x, yHandlePosition.y, yHandle.transform.localPosition.z);

                // Debug.Log(yHandle.transform.InverseTransformPoint(PinchDetector.transform.position).y);
                //  Debug.Log("Handle:"+yHandle.transform.localPosition.y);


        }
    }
}
