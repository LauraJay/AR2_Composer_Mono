using UnityEngine;
using System.Collections;

public class TableCalibration : MonoBehaviour
{
    //Steam Controller Input shit
    public Transform xyz;
    private SteamVR_TrackedObject controller;
    private SteamVR_Controller.Device controllerdevice;
    private int markerCounter;
    public Vector3[] positions;
    public bool[] setPositions;
    public setupScene setupScene;
    public float planeZOffset;

    // Use this for initialization
    void Start()
    {
        planeZOffset = -0.15f;
        positions = new Vector3[2];
        markerCounter = 0;
        //Pulling the Controllers into the controller Array
        controller = GetComponent<SteamVR_TrackedObject>();
        // controllers[1] = GetComponent<SteamVR_TrackedObject>();
        setPositions = new bool[2] { false, false };
 
    }

    private bool isMarkerVisible(Vector3 markerPos)
    {
        /*
        if (markerPos.x < -1000.0f || markerPos.x > 1000.0f || markerPos.y < -1000.0f || markerPos.y > 1000.0f || markerPos.z < -1000.0f || markerPos.z > 1000.0f)
            return false;
        */
        return true;
    }

    public void attemptCalibration()
    {
        /*
        if (isMarkerVisible(m200.transform.position))
        {
            positions[0] = m200.transform.position;
            positions[0].y += planeZOffset;
            setPositions[0] = true;
            m200.GetComponentInChildren<Renderer>().material.color = new Color(0, 128, 0);
            Debug.Log("Marker 200 calibrated successfully.");
        }
        if (isMarkerVisible(m400.transform.position))
        {
            positions[1] = m400.transform.position;
            positions[1].y += planeZOffset;
            setPositions[1] = true;
            m400.GetComponentInChildren<Renderer>().material.color = new Color(0, 128, 0);
            Debug.Log("Marker 400 calibrated successfully.");
        }
        if (isMarkerVisible(m600.transform.position))
        {
            positions[2] = m600.transform.position;
            positions[2].y += planeZOffset;
            setPositions[2] = true;
            m600.GetComponentInChildren<Renderer>().material.color = new Color(0, 128, 0);
            Debug.Log("Marker 600 calibrated successfully.");
        }
        if (isMarkerVisible(m800.transform.position))
        {
            positions[3] = m800.transform.position;
            positions[3].y += planeZOffset;
            setPositions[3] = true;
            m800.GetComponentInChildren<Renderer>().material.color = new Color(0, 128, 0);
            Debug.Log("Marker 800 calibrated successfully.");
        }
        */
    }

    public void setPosition(Vector3 position)
    {
        // Debug.Log(markerCounter);

        if (markerCounter < 2 && markerCounter >= 0)
        {
            positions[markerCounter] = position;
            setPositions[markerCounter] = true;
            markerCounter++;
            Debug.Log("saving position: " + position);
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (setPositions[0] && setPositions[1])
        {
            Debug.Log("Calibration successful.");
            setupScene.calibrationDone(positions);
            this.enabled = false;
        }
    }
}
