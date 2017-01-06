using UnityEngine;

public class ControllerPos : MonoBehaviour
{
    public Transform tableCalib;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controllerdevice;
    private readInNetworkData networkData;

    void Start(){
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    
    void Update(){
        controllerdevice = SteamVR_Controller.Input((int)trackedObj.index);
        if (controllerdevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
            networkData.sendTCPstatus((int)readInNetworkData.TCPstatus.controllerButtonPressed);
            tableCalib.GetComponent<TableCalibration>().setPosition((Vector3)controllerdevice.transform.pos);
        }
    }
}
