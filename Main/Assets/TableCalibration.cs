using UnityEngine;

public class TableCalibration : MonoBehaviour{
    private int markerCounter;
    private bool LLset;
    private bool URset;

    [Header("Dependencies")]
    public setupScene setupScene;
    private readInNetworkData networkData;
    private SteamVR_TrackedObject controller;
    private SteamVR_Controller.Device controllerdevice;

    [Header("Calibration")]
    public Vector3 lowerLeft;
    public Vector3 upperRight;
    public float planeHeightOffset;

    // Use this for initialization
    void Start(){
        planeHeightOffset = -0.15f;
        LLset = false;
        URset = false;
        markerCounter = 0;
        controller = GetComponent<SteamVR_TrackedObject>();
    }

    public void setPosition(Vector3 position){
        int statusReceived = networkData.receiveTCPstatus();
        switch (statusReceived){
            case (int)readInNetworkData.TCPstatus.arucoFound1:
                if(!LLset) {
                    lowerLeft = position;
                    Debug.Log("Plane calibration [lower left]: calibrated to " + position);
                }
                break;
            case (int)readInNetworkData.TCPstatus.arucoFound2:
                if (LLset && !URset){
                    Debug.Log("Plane calibration [upper right]: calibrated to " + position);
                    markerCounter++;
                }
                break;
            case (int)readInNetworkData.TCPstatus.arucoNotFound: break;
            case -1: Debug.LogError("Plane calibration: failed, because of a socket error."); break;
            default: Debug.LogError("Plane calibration: unknown status received: " + statusReceived); break;
        }
    }

    void Update(){
        if (LLset && URset){
            Debug.Log("Plane calibration: completed successfully.");
            setupScene.calibrationDone(lowerLeft, upperRight);
            this.enabled = false;
        }
    }
}
