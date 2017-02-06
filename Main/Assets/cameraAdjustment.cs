using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraAdjustment : MonoBehaviour {

    public Camera viveCamera;
    public float smoothing = 5f;
    private Vector3 offset;
    // private Vector3 forward
    // Use this for initialization
    void Start () {
        offset = viveCamera.transform.forward;//.normalized;
    }
	
	// Update is called once per frame
	void Update () {                
    }

    void LateUpdate(){
        // Create a postion the camera is aiming for based on the offset from the target.
        Vector3 targetCamPos = viveCamera.transform.position + offset;

        // Smoothly interpolate between the camera's current position and it's target position.
        viveCamera.transform.position = Vector3.Lerp(viveCamera.transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
