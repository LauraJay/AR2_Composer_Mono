using UnityEngine;

public class TableCalibration : MonoBehaviour{
    private SteamVR_TrackedObject controller;
    private SteamVR_Controller.Device controllerdevice;
    private int markerCounter;
    public Vector3[] positions;
    public bool[] setPositions;
    public setupScene setupScene;
    public float planeZOffset;

    // Use this for initialization
    void Start(){
        planeZOffset = -0.15f;
        positions = new Vector3[2];
        markerCounter = 0;
        controller = GetComponent<SteamVR_TrackedObject>();
        setPositions = new bool[2] { false, false };
    }

    public void setPosition(Vector3 position){
        if (markerCounter < 2 && markerCounter >= 0){
            positions[markerCounter] = position;
            setPositions[markerCounter] = true;
            markerCounter++;
            if(markerCounter == 1)
                Debug.Log("Lower left table calibration position: " + position);
            if (markerCounter == 2)
                Debug.Log("Upper right table calibration position: " + position);
        }
    }

    void Update(){
        if (setPositions[0] && setPositions[1]){
            Debug.Log("Calibration successful.");
            setupScene.calibrationDone(positions);
            this.enabled = false;
        }
    }
}
