using UnityEngine;
using System.Collections;

public class ControllerPos : MonoBehaviour
{
    public Transform TableCalibration;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controllerdevice;
    
   // public 
    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

    }

    // Update is called once per frame
    void Update()
    {
        controllerdevice = SteamVR_Controller.Input((int)trackedObj.index);
        if (controllerdevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
          //  Debug.Log("ControllerPos.cs: " + controllerdevice.transform.pos);
            TableCalibration.GetComponent<TableCalibration>().setPosition((Vector3) controllerdevice.transform.pos);
           // centerCube.localPosition = controllerdevice.transform.pos;
        }
    }
}
