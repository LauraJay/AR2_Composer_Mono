using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Webcam : MonoBehaviour {
    public RawImage rawImage;
    public Renderer rend;
    // Use this for initialization
    void Start () {
	 WebCamDevice[] devices  = WebCamTexture.devices;
        rend = GetComponent<Renderer>();
        rend.enabled = true;
         for( int i = 0; i< devices.Length; i++)
        {
            Debug.Log("Webcam found: " + devices[i].name);
        }
        WebCamTexture leftCam = new WebCamTexture();
        leftCam.deviceName = devices[1].name;
        //rawImage.texture = leftCam;
        //rawImage.material.mainTexture = leftCam;
        rend.material.mainTexture = leftCam;

        leftCam.Play();

        // Plane Position: x = 0.1, y = 0.01, z = 0.75 scale x= 0.16, scale y = 0.6 z = 0.1


    }

    // Update is called once per frame
    void Update () {
	
	}
}
