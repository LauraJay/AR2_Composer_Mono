using UnityEngine;

public class ControllerPos : MonoBehaviour
{
    [Header("Dependencies")]
    public Transform tableCalib;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controllerdevice;
    public readInNetworkData networkData;

    void Start(){
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Update(){
        controllerdevice = SteamVR_Controller.Input((int)trackedObj.index);
        if (controllerdevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
            networkData.sendTCPstatus((int)readInNetworkData.TCPstatus.controllerButtonPressed);
            tableCalib.GetComponent<TableCalibration>().setPosition(controllerdevice.transform.pos);
        }
    }
}