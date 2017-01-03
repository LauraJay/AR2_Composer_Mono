using UnityEngine;
using System.Collections;

public class MarkerScale : MonoBehaviour {

    // NOTE: Z is Y in Unity!
    private Transform xHandle;
    private Transform yHandle;
    private Transform zHandle;
    private Vector2 originalPosXY;
    private Vector2 originalPosZ;
    private Vector3 newScale;

    // Use this for initialization
    void Start () {
        xHandle = gameObject.transform.parent.FindChild("X_Handle");
        originalPosXY.x = xHandle.position.x;
        yHandle = gameObject.transform.parent.FindChild("Y_Handle");
        originalPosXY.y = yHandle.position.z;
        zHandle = gameObject.transform.parent.FindChild("Z_Handle");
        originalPosZ.x = zHandle.position.x;
        originalPosZ.y = zHandle.position.z;
    }
	
	// Update is called once per frame
	void Update () {
        newScale.x = - xHandle.position.x + originalPosXY.x + 1.0f;
        if (newScale.x < 1){
            newScale.x = 1;
            xHandle.position = new Vector3(originalPosXY.x, xHandle.position.y, xHandle.transform.position.z);
        }
        newScale.y = - yHandle.position.z + originalPosXY.y + 1.0f;
        if (newScale.y < 1){
            newScale.y = 1;
            yHandle.position = new Vector3(yHandle.position.x, yHandle.position.y, originalPosXY.y);
        }
        gameObject.transform.localScale = new Vector3(newScale.x, gameObject.transform.localScale.y, newScale.y);
        zHandle.position = new Vector3(originalPosZ.x - newScale.x + 1.0f, zHandle.position.y, originalPosZ.y - newScale.y + 1.0f);
    }
}
