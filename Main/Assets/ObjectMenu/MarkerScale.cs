using UnityEngine;
using System.Collections;

public class MarkerScale : MonoBehaviour {

    // NOTE: Z is Y in Unity!
    private Transform xHandle;
    private Transform yHandle;
    private Transform zHandle;
    private Vector2 originalPosXY;
    private Vector2 originalPosZ;
    private Vector3 originalScale;
    private Vector3 newScale;

    // Use this for initialization
    void Start (){
        xHandle = gameObject.transform.parent.FindChild("X_Handle");
        originalPosXY.x = xHandle.localPosition.x;
        yHandle = gameObject.transform.parent.FindChild("Y_Handle");
        originalPosXY.y = yHandle.localPosition.z;
        zHandle = gameObject.transform.parent.FindChild("Z_Handle");
        originalPosZ.x = zHandle.localPosition.x;
        originalPosZ.y = zHandle.localPosition.z;
        originalScale = gameObject.transform.localScale;
    }
	
	// Update is called once per frame
	void Update (){
        newScale.x = - xHandle.localPosition.x + originalPosXY.x + 1.0f;
        if (newScale.x < originalScale.x)
        {
            newScale.x = originalScale.x;
            xHandle.localPosition = new Vector3(originalPosXY.x, xHandle.localPosition.y, xHandle.localPosition.z);
        }
        newScale.y = - yHandle.localPosition.z + originalPosXY.y + 1.0f;
        if (newScale.y < originalScale.y)
        {
            newScale.y = originalScale.y;
            yHandle.position = new Vector3(yHandle.localPosition.x, yHandle.localPosition.y, originalPosXY.y);
        }
        Debug.Log(newScale);
        gameObject.transform.localScale = new Vector3(newScale.x, gameObject.transform.localScale.y, newScale.y);
        zHandle.localPosition = new Vector3(originalPosZ.x - newScale.x + 1.0f, zHandle.localPosition.y, originalPosZ.y - newScale.y + 1.0f);
    }
}