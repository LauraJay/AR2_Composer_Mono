using UnityEngine;

public class ControllerPos : MonoBehaviour
{
    public Transform TableCalibration;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controllerdevice;
    
    void Start(){
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        controllerdevice = SteamVR_Controller.Input((int)trackedObj.index); // Moved from update(), working??
    }
    
    void Update(){
        if (controllerdevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
            TableCalibration.GetComponent<TableCalibration>().setPosition(controllerdevice.transform.pos);
        }
    }
}
