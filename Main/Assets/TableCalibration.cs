using UnityEngine;
using UnityEngine.SceneManagement;

public class TableCalibration : MonoBehaviour{
    private int markerCounter;
    private bool LLset;
    private bool URset;

    [Header("Dependencies")]
    public setupScene setupScene;
    public readInNetworkData networkData;
    private SteamVR_TrackedObject controller;
    private SteamVR_Controller.Device controllerdevice;

    [Header("Calibration")]
    public Vector3 lowerLeft;
    public Vector3 upperRight;
    public float planeHeightOffset;
    private bool calibrateBoth;

    public void setCalibrateBoth(bool status){
        calibrateBoth = status;
    }

    // Use this for initialization
    void Start(){
        calibrateBoth = false;
        planeHeightOffset = -0.15f;
        LLset = false;
        URset = false;
        controller = GetComponent<SteamVR_TrackedObject>();
    }

    public void setPosition(Vector3 position){
        int statusReceived = networkData.receiveTCPstatus();
        switch (statusReceived){
            case (int)readInNetworkData.TCPstatus.arucoFound1:
                if(!LLset && !URset) {
                    lowerLeft = position;
                    LLset = true;
                    Debug.Log("Plane calibration [lower left]: calibrated to " + position);
                }else{
                    Debug.LogError("Plane calibration [lower left]: received status arucoFound1," +
                        " but one position has already been set.");
                }
                break;
            case (int)readInNetworkData.TCPstatus.arucoFound2:
                if (LLset && !URset){
                    upperRight = position;
                    URset = true;
                    Debug.Log("Plane calibration [upper right]: calibrated to " + position);
                }else{
                    Debug.LogError("Plane calibration [upper right]: received status arucoFound2," +
                        " but either lower left has not been set yet or upper right already has been.");
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

            // Tell setupScene that the calibration has been completed
            setupScene.calibrationDone(lowerLeft, upperRight);
            if (calibrateBoth){
                setupScene.setState((int)setupScene.state.poseAndPlaneCalibDone);
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("doPlaneCalibInVS"));
                SceneManager.LoadScene("doPoseCalibInVS", LoadSceneMode.Additive);
            }else{
                setupScene.setState((int)setupScene.state.planeCalibDone);
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("doPlaneCalibInVS"));
                SceneManager.LoadScene("CalibDone", LoadSceneMode.Additive);
            }

            // Disable controller position script
            //controller.GetComponent<ControllerPos>().enabled = false;

            // Disable this script
            this.enabled = false;
        }
    }
}
