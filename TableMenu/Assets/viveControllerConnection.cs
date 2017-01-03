using UnityEngine;
using System.Collections;

public class viveControllerConnection : MonoBehaviour{
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;
    public TableCalibration tableCalib;
    // Use this for initialization
    void Start(){
        trackedObject = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update(){
        device = SteamVR_Controller.Input((int)trackedObject.index);
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
            device.TriggerHapticPulse(3000);
            device.TriggerHapticPulse(3000);
            tableCalib.attemptCalibration();
        }
    }
}