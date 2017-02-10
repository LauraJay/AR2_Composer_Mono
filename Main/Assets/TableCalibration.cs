using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TableCalibration : MonoBehaviour{
    private int markerCounter;
    private bool LLset;
    private bool URset;

    [Header("Dependencies")]
    public setupScene setupScene;
    public readInNetworkData networkData;
    public ControllerPos controllerPos;

    // Needed for haptic feedback
    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controllerdevice;

    [Header("Calibration")]
    public Vector3 lowerLeft;
    public Vector3 upperRight;
    private bool calibrateBoth;

    public void setCalibrateBoth(bool status){
        calibrateBoth = status;
    }

    void Start(){
        calibrateBoth = false;
        LLset = false;
        URset = false;
    }

    // This is called by ControllerPos.cs when the trigger on
    // the right vive controller is pressed during calibration
    public void setPosition(Vector3 position){
        int statusReceived = networkData.receiveTCPstatus();
        switch (statusReceived){
            case (int)readInNetworkData.TCPstatus.arucoFound1:
                if(!LLset && !URset) {
                    lowerLeft = position;
                    LLset = true;
                    StartCoroutine(LongVibration(0.5f, 3999));
                    Debug.Log("[PLANE CALIBRATION] Lower left corner calibrated to " + position);
                }else{
                    Debug.LogError("[PLANE CALIBRATION] Lower left corner: received status arucoFound1," +
                        " but one position has already been set.");
                }
                break;
            case (int)readInNetworkData.TCPstatus.arucoFound2:
                if (LLset && !URset){
                    upperRight = position;
                    URset = true;
                    StartCoroutine(LongVibration(0.5f, 3999));
                    System.Threading.Thread.Sleep(2000);
                    Debug.Log("[PLANE CALIBRATION] Upper right corner calibrated to " + position);
                }else{
                    Debug.LogError("[PLANE CALIBRATION] Upper right corner: received status arucoFound2," +
                        " but either lower left has not been set yet or upper right already has been.");
                }
                break;
            case (int)readInNetworkData.TCPstatus.arucoNotFound: Debug.LogError("[PLANE CALIBRATION] AruCo marker not found, please try again."); break;
            case -1: Debug.LogError("[PLANE CALIBRATION] Failed, because of a socket error."); break;
            default: Debug.LogError("[PLANE CALIBRATION] Unknown status received: " + statusReceived); break;
        }
    }

    IEnumerator LongVibration(float length, float strength){
        for (float i = 0; i < length; i += Time.deltaTime){
            controllerdevice.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
            yield return null;
        }
    }

    void Update(){
        // Needed for haptic feedback
        controllerdevice = SteamVR_Controller.Input((int)trackedObj.index);

        if (LLset && URset){ // Calibration successful
            Debug.Log("Plane calibration: completed successfully.");

            // Tell setupScene that the calibration has been
            // completed and load / unload corresponding scenes
            setupScene.calibrationDone(lowerLeft, upperRight);

            // Write text file            
            string[] CalibPos = { "" + lowerLeft.x, "" + lowerLeft.y, "" + lowerLeft.z, "" + upperRight.x, "" + upperRight.y, "" + upperRight.z };            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.dataPath + "/Resources/planeCalibData.txt")){
                foreach (string line in CalibPos){
                    file.WriteLine(line);
                }
            }

            if (calibrateBoth){
                setupScene.setState((int)setupScene.state.poseAndPlaneCalibDone);
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("doPlaneCalibInVS"));
                SceneManager.LoadScene("doPoseCalibInVS", LoadSceneMode.Additive);
            }
            else if(networkData.receiveTCPstatus() == (int)readInNetworkData.TCPstatus.planeCalibDone){
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("doPlaneCalibInVS"));
                SceneManager.LoadScene("CalibDone", LoadSceneMode.Additive);
            }

            // Disable controller position script
            controllerPos.enabled = false;

            // Disable this script
            this.enabled = false;
        }                
    }
}
