using UnityEngine;
using System.Collections;

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
    public Vector3[] planeEdges;

    Mesh createPlane(Vector3[] positions)
    {
        Mesh m = new Mesh();
        m.name = "ScriptedMesh";

        m.vertices = new Vector3[] {
         positions[0], new Vector3(positions[0].x,positions[1].y,positions[0].z), new Vector3(positions[1].x,positions[0].y,positions[0].z), positions[1]
     };
        for (int i = 0; i < m.vertices.Length; i++)
        {
            Debug.Log(m.vertices[i]);
        }
        m.uv = new Vector2[] {
         new Vector2 (0, 0),
         new Vector2 (0, 1),
         new Vector2(1, 1),
         new Vector2 (1, 0)
     };
        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        m.RecalculateNormals();

        return m;

    }

    public void calibrationDone(Vector3[] markerPositions)
    {
        // Create plane
        //table = new Plane();
        //table.Set3Points(markerPositions[0], markerPositions[1], markerPositions[2]);
        //GameObject point0 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //point0.transform.position = markerPositions[0];
        //point0.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        //GameObject point1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //point1.transform.position = markerPositions[1];
        //point1.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        //GameObject point2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //point2.transform.position = markerPositions[2];
        //point2.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        //GameObject point3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //point3.transform.position = markerPositions[3];
        //point3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        table = GameObject.CreatePrimitive(PrimitiveType.Plane);
        table.GetComponent<MeshFilter>().mesh = createPlane(markerPositions);
        table.transform.SetParent(parent.transform);
        parent.transform.Translate(markerPositions[0]);
        //table.transform.Translate(markerPositions[0]);
    }

    // Use this for initialization
    void Start()
    {
        tableCalib.enabled = false;
        // Initialization
        markerCubes = new GameObject[maxMarkers];

        // Create parent object (plane and cubes are attached to this)
        parent = new GameObject();
        parent.transform.name = "Table Object";

        // Create markers (cubes)
        GameObject MarkerMaster = GameObject.Find("Marker");
        for (int i = 0; i < maxMarkers; i++){
            // NEW
            markerCubes[i] = Instantiate(MarkerMaster);
            markerCubes[i].transform.SetParent(parent.transform);
            markerCubes[i].SetActive(false);
            markerCubes[i].transform.name = "Marker" + i;
            markerCubes[i].transform.FindChild("MarkerPivot").transform.FindChild("Cube").GetComponent<Renderer>().material.color = new Color(0, 255, 0); ;
            //markerCubes[i].transform.localScale = new Vector3(markerScale, markerScale, markerScale);
            
            //// OLD
            ////markerCubes[i] = new GameObject();
            //markerCubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //markerCubes[i].transform.SetParent(parent.transform);
            //markerCubes[i].SetActive(false);
            //markerCubes[i].transform.name = "Marker" + i;
            //markerCubes[i].transform.localScale = new Vector3(markerScale, markerScale, markerScale);
            //markerCubes[i].GetComponent<Renderer>().material.color = new Color(0, 255, 0);
        }
        MarkerMaster.SetActive(false);
        networkData = gameObject.GetComponent<readInNetworkData>();
        tableCalib.enabled = true;
    }

    public void setMarkerArraySet(bool state)
    {
        markerArraySet = state;
    }

    private void simulateMarkerMovement()
    {

        //frameIncrement += 0.0001f;
        //int numberOfMarkers = 5;
        //networkMarkers = new Marker[numberOfMarkers];
        //networkMarkers[0] = new Marker(1, -0.1f - frameIncrement, -0.1f - frameIncrement, 50.0f + frameIncrement * 10000);
        //networkMarkers[1] = new Marker(2, -0.1f - frameIncrement, 0.1f + frameIncrement, 10.0f - frameIncrement * 10000);
        //networkMarkers[2] = new Marker(3, 0.1f + frameIncrement, 0.1f + frameIncrement, 170.0f + frameIncrement * 10000);
        //networkMarkers[3] = new Marker(4, 0.1f + frameIncrement, -0.1f - frameIncrement, 90.0f - frameIncrement * 10000);
        //networkMarkers[4] = new Marker(-1, 0.0f, 0.0f, 0.0f);
        //markerArraySet = true;

    }


    // Update is called once per frame
    void Update()
    {
        if (bypassNetwork){
            simulateMarkerMovement();
            //networkMarkers = networkData.getMarkers();
        }else if (markerArraySet){
            networkMarkersPrevFrame = networkMarkers;
            for (int i = 0; i < networkMarkers.Length; i++)
            {
                Marker cur = networkMarkers[i];
                if (cur.getID() == -1)
                {
                    break;
                }
                markerCubes[i].SetActive(true);
                markerCubes[i].transform.position = new Vector3(cur.getPosX(), 0.0f, cur.getPosY());
                markerCubes[i].transform.rotation = Quaternion.Euler(0.0f, cur.getAngle(), 0.0f);
            }
            for (int j = 0; j < networkMarkersPrevFrame.Length; j++)
            {
                if (markerCubes[j] == null)
                {
                    markerCubes[j].SetActive(false);
                }
            }
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.001f);
    }
}
