﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


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
        textArea = gameObject.GetComponent<Text>();
        contextMenu = textArea.transform.parent.gameObject;
        canvasTransform = contextMenu.transform.parent.gameObject;
        cube = canvasTransform.transform.parent.FindChild("MarkerPivot").gameObject;
        marker = cube.transform.parent.gameObject;
        String id = marker.name.Substring(6);
        if (!id.Equals("Master"))
            buildingID = System.Int32.Parse(marker.name.Substring(6));
        else
            buildingID = 0;
    }
	
	// Update is called once per frame
	void Update () {
        dims.x = cube.transform.localScale.x;
        dims.y = cube.transform.localScale.y;
        dims.z = cube.transform.localScale.z;
        floors = (int)dims.y;
        contextMenu.transform.position = new Vector3(contextMenu.transform.position.x, cube.transform.localScale.y + 3, contextMenu.transform.position.z);
        canvasTransform.transform.rotation = new Quaternion(canvasTransform.transform.rotation.x, cam.transform.rotation.y, canvasTransform.transform.rotation.z, 1.0f);
        textArea.text = "Building ID: ";
    }
}