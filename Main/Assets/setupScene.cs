using UnityEngine;
using System.Collections;
using System;

public class setupScene : MonoBehaviour
{
    GameObject[] markerCubes;
    readInNetworkData networkData;
    Marker[] networkMarkers;
    Marker[] networkMarkersPrevFrame;
    bool markerArraySet = false;
    GameObject table;
    public bool bypassNetwork = true;
    float frameIncrement = 0.0f;
    GameObject parent;

    [Header("Scene Settings")]
    public int maxMarkers = 256;
    //public Vector3 planeScale = new Vector3(-0.14f, 0.782f, 0.08f);
    //public Vector3 planePosition = new Vector3(-0.146f, 0.782f, 0.084f);
    public float markerScale = 0.5f;
    [Header("Calibration")]
    public TableCalibration tableCalib;

    public void calibrationDone(Vector3[] markerPositions)
    {
        // Create plane
        table = GameObject.CreatePrimitive(PrimitiveType.Plane);
        float width = Math.Abs(markerPositions[1].x - markerPositions[0].x);
        float height = Math.Abs(markerPositions[1].z - markerPositions[0].z);
        Vector3 position = new Vector3(
                                        (markerPositions[0].x + markerPositions[1].x) / 2,
                                        (markerPositions[0].y + markerPositions[1].y) / 2,
                                        (markerPositions[0].z + markerPositions[1].z) / 2
        );
        table.transform.position = position;
        table.transform.localScale = new Vector3(width, 1, height);
    }

    // Use this for initialization
    void Start()
    {
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

    public void setMarkerArraySet(bool state){
        markerArraySet = state;
    }

    //// Returns the position on the plane for the tracked (normalized) marker position
    //private Vector3 getCalibratedMarkerPos(Vector3 position)
    //{
    //    // Linear interpolation of X
    //    float xMin = tablePositions[1].x;
    //    float xMax = tablePositions[0].x;
    //    float newX = xMin + position.x * (xMax - xMin);

    //    // Linear interpolation of Y (Z in unity)
    //    float yMin = tablePositions[1].z;
    //    float yMax = tablePositions[2].z;
    //    float newY = yMin + position.z * (yMax - yMin);

    //    // Linear interpolation of Z (Y in unity)
    //    float z1 = tablePositions[0].y * ((xMax - xMin) / (xMax - newX)) + tablePositions[1].y * ((xMax - xMin) / (newX - xMin));
    //    float z2 = tablePositions[3].y * ((xMax - xMin) / (xMax - newX)) + tablePositions[2].y * ((xMax - xMin) / (newX - xMin));
    //    float newZ = z2 * ((yMax - yMin) / (yMax - newY)) + z1 * ((yMax - yMin) / (newY - yMin));
    //    return new Vector3(newX, newZ, newY);
    //}

    private void simulateMarkerMovement(){
        frameIncrement += 0.0001f;
        int numberOfMarkers = 5;
        networkMarkers = new Marker[numberOfMarkers];
        networkMarkers[0] = new Marker(1, -0.1f - frameIncrement, -0.1f - frameIncrement, 50.0f + frameIncrement * 50, 1);
        networkMarkers[1] = new Marker(2, -0.1f - frameIncrement, 0.1f + frameIncrement, 10.0f - frameIncrement * 50, 1);
        networkMarkers[2] = new Marker(3, 0.1f + frameIncrement, 0.1f + frameIncrement, 170.0f + frameIncrement * 50, 1);
        networkMarkers[3] = new Marker(4, 0.1f + frameIncrement, -0.1f - frameIncrement, 90.0f - frameIncrement * 50, 1);
        networkMarkers[4] = new Marker(-1, 0.0f, 0.0f, 0.0f, 0);
        markerArraySet = true;

    }

    // Update is called once per frame
    void Update(){
        if (bypassNetwork){
            simulateMarkerMovement();
            //networkMarkers = networkData.getMarkers();
        }
        if(markerArraySet){
            networkMarkersPrevFrame = networkMarkers;
            for (int i = 0; i < networkMarkers.Length; i++){
                Marker cur = networkMarkers[i];
                if (cur.getID() == -1){
                    break;
                }
                markerCubes[i].SetActive(true);
                markerCubes[i].transform.position = new Vector3(cur.getPosX(), 0.0f, cur.getPosY());
                markerCubes[i].transform.rotation = Quaternion.Euler(0.0f, cur.getAngle(), 0.0f);
            }
            //for (int j = 0; j < networkMarkersPrevFrame.Length; j++){
            //    if (markerCubes[j] == null){
            //        markerCubes[j].SetActive(false);
            //    }
            //}
        }
    }
}
