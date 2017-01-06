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
        originalPosXY.x = xHandle.localPosition.x;
        yHandle = gameObject.transform.parent.FindChild("Y_Handle");
        originalPosXY.y = yHandle.localPosition.z;
        zHandle = gameObject.transform.parent.FindChild("Z_Handle");
        originalPosZ.x = zHandle.localPosition.x;
        originalPosZ.y = zHandle.localPosition.z;
    }
	
    public void extrudeBuilding()
    {
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.y +1, gameObject.transform.localScale.z);
    }
    public void deExtrudeBuilding()
    {
        gameObject.transform.localScale = new Vector3( gameObject.transform.localScale.x, gameObject.transform.localScale.y - 1, gameObject.transform.localScale.z );
    }

    // Update is called once per frame
    void Update () {
        newScale.x = - xHandle.localPosition.x + originalPosXY.x + 1.0f;
        if (newScale.x < 1){
            newScale.x = 1;
            xHandle.localPosition = new Vector3(originalPosXY.x, xHandle.localPosition.y, xHandle.transform.localPosition.z);
        }
        newScale.y = - yHandle.localPosition.z + originalPosXY.y + 1.0f;
        if (newScale.y < 1){
            newScale.y = 1;
            yHandle.localPosition = new Vector3(yHandle.localPosition.x, yHandle.localPosition.y, originalPosXY.y);
        }
        gameObject.transform.localScale = new Vector3(newScale.x, gameObject.transform.localScale.y, newScale.y);
        zHandle.localPosition = new Vector3(originalPosZ.x - newScale.x + 1.0f, zHandle.localPosition.y, originalPosZ.y - newScale.y + 1.0f);
    }
}