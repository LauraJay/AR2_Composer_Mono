using UnityEngine;
using System;

public class setupScene : MonoBehaviour{
    GameObject[] markerCubes;
    readInNetworkData networkData;
    Marker[] networkMarkers;
    Marker[] networkMarkersPrevFrame;
    bool markerArraySet = false;
    GameObject table;
    public bool bypassNetwork = true;
    //float frameIncrement = 0.0f;
    GameObject parent;

    [Header("Scene Settings")]
    // Maximum number of markers that can be displayed
    public int maxMarkers = 256;

    // Global scale of each marker to fit size of virtual to real markers
    public float markerScale = 0.5f;

    [Header("Calibration")]
    // TableCalibration script input
    public TableCalibration tableCalib;

    // Calibrated positions for plane
    public Vector3[] calibratedPositions;
    private bool calibDone;

    public void calibrationDone(Vector3[] markerPositions){
        // Make marker positions available globally
        calibratedPositions = markerPositions;

        // Create plane (table surface)
        table = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Debug.Log("table object created: " + table);
        table.transform.parent = parent.transform;
        float width = Math.Abs(calibratedPositions[1].x - calibratedPositions[0].x);
        float height = Math.Abs(calibratedPositions[1].z - calibratedPositions[0].z);
        Vector3 position = new Vector3(
                                        (calibratedPositions[0].x + calibratedPositions[1].x) / 2,
                                        (calibratedPositions[0].y + calibratedPositions[1].y) / 2,
                                        (calibratedPositions[0].z + calibratedPositions[1].z) / 2
                                        );
        table.transform.position = position;
        table.transform.localScale = new Vector3(width / 10, 1, height / 10);
        calibDone = true;
    }

    // Use this for initialization
    void Start()
    {
        calibDone = false;
        tableCalib.enabled = false;
        // Initialization
        markerCubes = new GameObject[maxMarkers];

        // Create parent object (plane and cubes are attached to this)
        parent = new GameObject();
        parent.transform.name = "TableObject";

        // Create markers (cubes)
        GameObject MarkerMaster = GameObject.Find("MarkerMaster");
        for (int i = 0; i < maxMarkers; i++){
            markerCubes[i] = Instantiate(MarkerMaster);
            markerCubes[i].transform.SetParent(parent.transform);
            markerCubes[i].SetActive(false);
            markerCubes[i].transform.name = "Marker" + i;
            markerCubes[i].transform.FindChild("MarkerPivot").transform.FindChild("Cube").GetComponent<Renderer>().material.color = new Color(0, 255, 0);
            markerCubes[i].transform.localScale = new Vector3(markerScale, markerScale, markerScale);
        }
        MarkerMaster.SetActive(false);
        networkData = gameObject.GetComponent<readInNetworkData>();
        
        tableCalib.enabled = true;
    }

    private GameObject initializeMarker(int index)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.transform.SetParent(parent.transform);
        marker.SetActive(false);
        marker.transform.name = "Marker" + index;
        marker.transform.localScale = new Vector3(markerScale, markerScale, markerScale);
        return marker;
    }

    // Set whether the marker array has been filled
    public void setMarkerArraySet(bool state){
        markerArraySet = state;
    }

    // Returns the position on the plane for the tracked (normalized) marker position
    private Vector3 getCalibratedMarkerPos(Vector3 position){
        // Linear interpolation of X
        float xMin = calibratedPositions[0].x;
        float xMax = calibratedPositions[1].x;
        float newX = xMin + position.x * (xMax - xMin);

        // Linear interpolation of Y (Z in unity)
        float yMin = calibratedPositions[1].z;
        float yMax = calibratedPositions[0].z;
        float newY = yMin + position.z * (yMax - yMin);

        // Linear interpolation of Z (Y in unity)
        float zMin = calibratedPositions[1].y;
        float zMax = calibratedPositions[0].y;
        float newZ = zMin + position.y * (zMax - zMin);

        return new Vector3(newX, newZ, newY);
    }

    // Simulate a little bit of movement for 4 markers for when TCP cannot be used
    private void simulateMarkerMovement(){
        //frameIncrement += 0.0001f;
        networkMarkers = new Marker[5];
        networkMarkers[0] = new Marker(1, 0.0f, 0.0f, 0.0f, 1); // Upper left
        networkMarkers[1] = new Marker(2, 1.0f, 0.0f, 0.0f, 1); // Upper right
        networkMarkers[2] = new Marker(3, 0.0f, 1.0f, 0.0f, 1); // Lower left
        networkMarkers[3] = new Marker(4, 1.0f, 1.0f, 0.0f, 1); // Lower right
        networkMarkers[4] = new Marker(-1, 0.0f, 0.0f, 0.0f, 0);
        markerArraySet = true;
    }

    // Update is called once per frame
    void Update(){
        //if (bypassNetwork)
        //    simulateMarkerMovement();            
        if(markerArraySet){
            networkMarkers = networkData.getMarkers();
            networkMarkersPrevFrame = networkMarkers;
            for (int i = 0; i < networkMarkers.Length; i++){
                Marker cur = networkMarkers[i];
                if (cur == null)
                    break;
                if (cur.getID() == -1){
                    break;
                }
                Vector3 position = new Vector3(1-cur.getPosX(), 0.0f, cur.getPosY());
                if (calibDone)
                    markerCubes[i].transform.position = getCalibratedMarkerPos(position);
                else
                    markerCubes[i].transform.position = position;
                if(i==0)
                    Debug.Log("Angle for ID 0: " + -cur.getAngle());
                markerCubes[i].transform.rotation = Quaternion.Euler(0.0f, -cur.getAngle() + 45.0f, 0.0f);
                if (cur.getStatus().Equals(1))
                    markerCubes[i].SetActive(true);
                else
                    markerCubes[i].SetActive(false);
            }
            markerArraySet = false;
            // Check if any markers have been deleted
            for (int j = 0; j < networkMarkersPrevFrame.Length - 1; j++)
            {
                if (networkMarkersPrevFrame[j] != null && networkMarkers[j] == null)
                    markerCubes[j] = initializeMarker(j); //Marker has been deleted, reinitialize GameObject
            }
        }  
    }
}
