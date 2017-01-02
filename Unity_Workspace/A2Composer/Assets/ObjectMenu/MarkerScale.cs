using UnityEngine;
using System.Collections;

public class MarkerScale : MonoBehaviour {

    // NOTE: Z is Y in Unity!
    private GameObject xHandle;
    private GameObject yHandle;
    private GameObject zHandle;
    private Vector2 originalPosXY;
    private Vector2 originalPosZ;
    private Vector3 newScale;

    // Use this for initialization
    void Start () {
        xHandle = GameObject.Find("X_Handle");
        originalPosXY.x = xHandle.transform.position.x;
        yHandle = GameObject.Find("Y_Handle");
        originalPosXY.y = yHandle.transform.position.z;
        zHandle = GameObject.Find("Z_Handle");
        originalPosZ.x = zHandle.transform.position.x;
        originalPosZ.y = zHandle.transform.position.z;
    }
	
	// Update is called once per frame
	void Update () {
        newScale.x = - xHandle.transform.position.x + originalPosXY.x + 1.0f;
        if (newScale.x < 1) { 
            newScale.x = 1;
            xHandle.transform.position = new Vector3(originalPosXY.x, xHandle.transform.position.y, xHandle.transform.position.z);
        }
        newScale.y = - yHandle.transform.position.z + originalPosXY.y + 1.0f;
        if (newScale.y < 1) { 
            newScale.y = 1;
            yHandle.transform.position = new Vector3(yHandle.transform.position.x, yHandle.transform.position.y, originalPosXY.y);
        }
        this.transform.localScale = new Vector3(newScale.x, this.transform.localScale.y, newScale.y);
        zHandle.transform.position = new Vector3(originalPosZ.x - newScale.x + 1.0f, zHandle.transform.position.y, originalPosZ.y - newScale.y + 1.0f);
    }
}
