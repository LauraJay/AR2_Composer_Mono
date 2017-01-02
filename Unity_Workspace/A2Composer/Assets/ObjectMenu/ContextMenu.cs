using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ContextMenu : MonoBehaviour {

    GameObject marker;
    GameObject cube;
    GameObject contextMenu;
    GameObject canvasTransform;
    int buildingID;
    float livingArea;
    int floors;
    Vector3 dims;
    Text textArea;
    Camera cam;

    // Use this for initialization
    void Start () {
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        textArea = GameObject.Find("ContextMenuText").GetComponent<Text>();
        contextMenu = GameObject.Find("ContextMenu");
        cube = GameObject.Find("MarkerPivot").gameObject;
        marker = cube.transform.parent.gameObject;
        canvasTransform = GameObject.Find("CanvasTransform");
        buildingID = System.Int32.Parse(marker.name.Substring(6));
    }
	
	// Update is called once per frame
	void Update () {
        dims.x = cube.transform.localScale.x;
        dims.y = cube.transform.localScale.y;
        dims.z = cube.transform.localScale.z;
        floors = (int)dims.y;
        contextMenu.transform.position = new Vector3(contextMenu.transform.position.x, cube.transform.localScale.y + 3, contextMenu.transform.position.z);
        canvasTransform.transform.rotation = new Quaternion(canvasTransform.transform.rotation.x, cam.transform.rotation.y, canvasTransform.transform.rotation.z, 1.0f);
        textArea.text = "Building ID: \t" + buildingID + "\n" +
            "Living area: \t" + livingArea + " m²\n" +
            "Floors: \t\t\t" + floors + "\n" +
            "Width: \t\t\t" + dims.x + " m\n" +
            "Height: \t\t" + dims.y + " m\n" +
            "Depth: \t\t\t" + dims.z + " m\n" +
            "Base area: \t" + dims.x * dims.z + " m²";
    }
}