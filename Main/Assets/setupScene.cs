using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class setupScene : MonoBehaviour{
    private GameObject[] markerCubes;

    // Marker data received over TCP
    private Marker[] networkMarkers;
    private Marker[] networkMarkersPrevFrame;

    private bool markerArraySet = false;
    private GameObject table;
    private GameObject parent;

    [Header("Dependencies")]
    // TableCalibration script input
    public TableCalibration tableCalib;
    private readInNetworkData networkData;

    // This is overwritten by inspector input
    [Header("Scene Settings")]
    // Maximum number of markers that can be displayed (virtual markers)
    private int markersToRender;

    // Global scale of each marker to fit size of virtual to real markers
    public float markerScale = 0.05f;    
    public float planeHeightOffset = -0.023f;
    public float markerHeightOffset = -0.023f;

    // Calibrated positions for plane
    private Vector3 calibratedLL;
    private Vector3 calibratedUR;
    private bool calibDone;

    // State for the main loop
    public enum state { planeCalib, poseAndPlaneCalib, poseAndPlaneCalibDone, startScene }
    bool statusChanged;
    int currentState;
    bool doRender;

    // Can be called to set the current state of the "Update()-loop"
    public void setState(int state){
        currentState = state;
        statusChanged = true;
        doRender = false;
        Debug.Log("[STATE LOOP] State changed to: " + Enum.GetName(typeof(state), state));
    }

    // Is called by the menu that lets the user set the scale
    public void setScale(int scale){
        markerScale = 10 / (float)scale;
    }

    public void noCalibration(){
        Debug.Log("No Calibration is done. Loading old information.");
        string[] planeCalibDatText = System.IO.File.ReadAllLines(Application.dataPath + "/Resources/planeCalibData.txt");
        if (planeCalibDatText.Length != 6){
            Debug.LogError("No calibration has been selected, but no valid text file has been read.");
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("firstmenu"));
            SceneManager.LoadScene("secondmenu", LoadSceneMode.Additive);
        }
        else{
            Vector3 lowerLeft = new Vector3();
            Vector3 upperRight = new Vector3();
            lowerLeft.x = float.Parse(planeCalibDatText[0]);
            lowerLeft.y = float.Parse(planeCalibDatText[1]);
            lowerLeft.z = float.Parse(planeCalibDatText[2]);
            upperRight.x = float.Parse(planeCalibDatText[3]);
            upperRight.y = float.Parse(planeCalibDatText[4]);
            upperRight.z = float.Parse(planeCalibDatText[5]);

            Debug.Log("[LOADING CALIBRATION DATA] lowerLeft: " + lowerLeft);
            Debug.Log("[LOADING CALIBRATION DATA] upperRight: " + upperRight);

            calibrationDone(lowerLeft, upperRight);

            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("firstmenu"));
            SceneManager.LoadScene("thirdmenu", LoadSceneMode.Additive);
        }
    }

    // Is called when table calibration finishes (in TableCalibration.cs)
    public void calibrationDone(Vector3 lowerLeft, Vector3 upperRight){
        // Make marker positions available globally
        calibratedLL = lowerLeft;
        calibratedUR = upperRight;

        // Create plane (table surface)
        table = GameObject.CreatePrimitive(PrimitiveType.Plane);
        table.name = "TablePlane";
        table.transform.parent = parent.transform;
        float width = Math.Abs(calibratedUR.x - calibratedLL.x);
        float height = Math.Abs(calibratedUR.z - calibratedLL.z);
        Vector3 position = new Vector3(
                                        (calibratedLL.x + calibratedUR.x) / 2,
                                        ((calibratedLL.y + calibratedUR.y) / 2) + planeHeightOffset,
                                        (calibratedLL.z + calibratedUR.z) / 2
                                      );
        table.transform.position = position;
        table.transform.localScale = new Vector3(width / 10, 1, height / 10);

        GameObject tableMenue = GameObject.Find("TableMenuParent");
        //tableMenue.transform.parent = table.transform;
        //tableMenue.transform.position = new Vector3(position.x+width/2, position.y, position.z + height/2); 
        
        calibDone = true;
    }

    void Start() {
        // Initialization
        calibDone = false;
        tableCalib.enabled = false;        
        networkMarkersPrevFrame = new Marker[0];
        networkData = gameObject.GetComponent<readInNetworkData>();
        markersToRender = networkData.getMarkersToReceive() + 1;
        markerCubes = new GameObject[markersToRender];

        // Create parent object (plane and cubes are attached to this)
        parent = new GameObject();
        parent.transform.name = "TableObject";

        // Create markers (cubes)
        GameObject MarkerMaster = GameObject.Find("MarkerMaster");
        for (int i = 0; i < markersToRender; i++) {
            markerCubes[i] = Instantiate(MarkerMaster);
            markerCubes[i].transform.SetParent(parent.transform);
            markerCubes[i].SetActive(false);
            markerCubes[i].transform.name = "Marker" + i;
           // markerCubes[i].transform.FindChild("Pivot").transform.FindChild("Cube").GetComponent<Renderer>().material.color = new Color(0, 255, 0);
            markerCubes[i].transform.localScale = new Vector3(markerScale, markerScale, markerScale);
        }
        MarkerMaster.SetActive(false);        
    }

    // Set whether the marker array has been filled
    public void setMarkerArraySet(bool state){
        markerArraySet = state;
    }

    // Returns the position on the plane for the tracked (normalized) marker position
    private Vector3 getCalibratedMarkerPos(Vector3 position){
        // Linear interpolation of X
        float xMin = calibratedLL.x;
        float xMax = calibratedUR.x;
        float newX = xMin + position.x * (xMax - xMin);

        // Linear interpolation of Y
        float newY = (calibratedUR.y + calibratedLL.y) / 2;
        newY += markerHeightOffset;

        // Linear interpolation of Z
        float zMin = calibratedUR.z;
        float zMax = calibratedLL.z;
        float newZ = zMin + position.z * (zMax - zMin);

        return new Vector3(newX, newY, newZ);
    }

    private void renderMarkersFromTCP(){
        if (markerArraySet){
            // Pull markers for current frame from readInNetworkData script
            networkMarkers = networkData.getMarkers();

            Debug.Log("networkMarkers.Length: " + networkMarkers.Length);

            for (int i = 0; i < networkMarkers.Length; i++){
                Marker cur = networkMarkers[i];
                if (cur == null) // Not necessary any more, but can't hurt
                    continue;
                if (cur.getID() == -1){
                    markerCubes[i].SetActive(false);
                    continue;
                }
                if (cur.getID() == -2) // End of frame reached
                    break;

                // This makes no sense, but is a temporary fix
                // for a problem perhaps caused by the Vive HMD
                Vector3 position = new Vector3(1 - cur.getPosY(), 0.0f, 1 - cur.getPosX());

                if (calibDone)
                    markerCubes[i].transform.position = getCalibratedMarkerPos(position);
                else
                    markerCubes[i].transform.position = position;
                markerCubes[i].transform.rotation = Quaternion.Euler(0.0f, cur.getAngle(), 0.0f);

                if (cur.getStatus() == 1) // Is marker visible?
                    markerCubes[i].SetActive(true);
                else
                    markerCubes[i].SetActive(false);
            }
            markerArraySet = false;
        }
    }

    void Update(){
        if (doRender)
            renderMarkersFromTCP();
        else if (statusChanged) {
            statusChanged = false;
            switch (currentState){
                case (int)state.planeCalib:
                    Debug.Log("Entered state: planeCalib");
                    tableCalib.enabled = true;
                    networkData.sendTCPstatus((int)readInNetworkData.TCPstatus.planeOnlyCalib);
                    // Continue in TableCalibration.cs, when done set currentState to get cracking
                    break;
                case (int)state.poseAndPlaneCalib:
                    Debug.Log("Entered state: poseAndPlaneCalib");
                    tableCalib.enabled = true;
                    tableCalib.setCalibrateBoth(true);
                    networkData.sendTCPstatus((int)readInNetworkData.TCPstatus.planeAndPoseCalib);
                    break;
                case (int)state.poseAndPlaneCalibDone:
                    Debug.Log("Entered state: poseAndPlaneCalibDone");
                   
                    if (networkData.receiveTCPstatus() == (int)readInNetworkData.TCPstatus.poseCalibDone){
                        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("doPoseCalibInVS"));
                        SceneManager.LoadScene("CalibDone", LoadSceneMode.Additive);
                    }
                    break;
                case (int)state.startScene:
                    Debug.Log("Entered state: startScene");
                    networkData.sendTCPstatus((int)readInNetworkData.TCPstatus.sceneStart);
                    networkData.setSceneStarted(true);
                    doRender = true;                  
                    break;
                default: Debug.Log("setupScene state loop: no state specified."); break;
            }
        }
    }    
}