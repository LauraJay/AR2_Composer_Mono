using UnityEngine;
using System;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Collections;

public class readInNetworkData : MonoBehaviour {
    Boolean socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    byte[] readBuffer;
    int readBufferLength;
    Marker[] markers;
    long frameCounter = 0;
    bool oneMarkerSet = false;
    [Header("Dependencies")]
    public setupScene setupScene;
    public Text TCPText;

    // This is overwritten by inspector input
    [Header("Socket Settings")]
    public String Host = "192.168.0.7"; 
    public Int32 Port = 10000;

    // This is overwritten by inspector input
    [Header("Data Stream Settings")]
    public int maxMarkerCount = 100; // This multiplied by bytesPerMarker has to match
    public int bytesPerMarker = 20;  // the length of the byte array that is sent over TCP
    public bool printMarkerDebugInfo = false;

    // TCP status enum for sending AND receiving statuses
    public enum TCPstatus { planeAndPoseCalib, planeOnlyCalib, sceneStart, planeCalibDone, poseCalibDone, controllerButtonPressed, arucoFound1, arucoFound2, arucoNotFound };
    private bool sceneStarted;

    // Return markers array (used by setupScene.cs)
    public Marker[] getMarkers() {
        return markers;
    }

    // Initialization
    void Start(){
        readBufferLength = bytesPerMarker * maxMarkerCount + 4; // +4 because ID=-1 marks end of frame
        markers = new Marker[maxMarkerCount + 1];
        setupSocket();
        sceneStarted = false;
    }

    public void setSceneStarted(bool status){
        sceneStarted = status;
    }

    private void setupSocket(){
        try{
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            socketReady = true;
            Debug.Log("Socket set up successfully.");
        }catch (Exception e){
            Debug.LogError("Socket setup failed. Error: " + e);
        }
    }

    // Send status over TCP according to TCPstatus enum
    public void sendTCPstatus(int status){
        if (socketReady) { 
            theStream.Write(System.BitConverter.GetBytes(status), 0, 4);
            Debug.Log("Status sent: " + status);
        }
        else
            Debug.LogError("Failed to send status, because the socket is not ready: " + status);
    }

    // Receive status over TCP according to TCPstatus enum
    public int receiveTCPstatus()
    {
        if (socketReady)
        {
            while (!theStream.DataAvailable)
            {
                Debug.Log("Waiting for status to be received.");
                StartCoroutine(WaitForSeconds(1));
            }
            byte[] receivedBytes = new byte[4];
            theStream.Read(receivedBytes, 0, 4);
            int status = System.BitConverter.ToInt32(receivedBytes, 0);
            Debug.Log("Status received: " + status);
            return status;
        }
        Debug.LogError("Failed to receive status, because the socket is not ready.");
        return -1;
    }

    //// NEW IDEA: READ ASYNCHRONOUSLY FROM SOCKET
    //// Receive status over TCP according to TCPstatus enum
    //public int receiveTCPstatus()
    //{
    //    AsyncCallback callback = null;
    //    int status = -1;
    //    callback = ar =>
    //    {
    //        int bytesRead = theStream.EndRead(ar);
    //        if (bytesRead == 4)
    //        {
    //            byte[] receivedBytes = new byte[4];
    //            theStream.BeginRead(receivedBytes, 0, 4, callback, theStream);
    //            status = System.BitConverter.ToInt32(receivedBytes, 0);
    //            Debug.Log("Status received: " + status);
    //        }
    //        else
    //        {
    //            Debug.LogError("Failed to receive status, because the number of bytes read from the socket was incorrect (!= 4).");
    //        }
    //    };
    //    return status;
    //}

    // Returns the number of bytes that have been read from the stream in int
    private int receiveTCPdata(){
        if (socketReady && theStream.DataAvailable){
            readBuffer = new byte[readBufferLength];
            return theStream.Read(readBuffer, 0, readBufferLength);
        }
        Debug.LogError("Failed to receive marker data. Socket ready: " + socketReady + "; stream data available: " + theStream.DataAvailable);
        return -1;
    }

    private void interpretTCPMarkerData(){
        for (int i = 0; i < readBufferLength; i += bytesPerMarker){
            int curID = System.BitConverter.ToInt32(readBuffer, i); // Convert the marker ID
            if (curID == -1){ // End of frame reached?
                if(printMarkerDebugInfo)
                    Debug.Log("Last masker reached, suspending loop for current frame " + frameCounter + ".");
                frameCounter++; // This is counted even if showMarkerDebugInfo is false, so that it can be enabled at any time
                markers[i / bytesPerMarker + 1] = new Marker(-1, 0.0f, 0.0f, 0.0f, 0); // Set last marker as EOF (end of frame)
                break;                                                                 // and suspend loop
            }else if (curID < 0 || curID > maxMarkerCount){
                Debug.LogError("Marker ID not valid: " + curID);
            }else{
                float curPosX = System.BitConverter.ToSingle(readBuffer, i + 4); // Convert the x-position
                float curPosY = System.BitConverter.ToSingle(readBuffer, i + 8); // Convert the y-position
                float curAngle = System.BitConverter.ToSingle(readBuffer, i + 12); // Conver the angle
                int status = System.BitConverter.ToInt32(readBuffer, i + 16); // Convert the status of the marker
                markers[i / bytesPerMarker] = new Marker(curID, curPosX, curPosY, curAngle, status); // Add new marker to array
                oneMarkerSet = true;    // Give permission to use marker array since at least
                                        // one marker has been set for the current frame
                TCPText.text = markers[i / bytesPerMarker].toStr();
                if (printMarkerDebugInfo)
                    Debug.Log(markers[i / bytesPerMarker].toStr()); // Print debug message containing marker data
            }
        }
    }

    void Update(){
        if (sceneStarted) { 
            setupScene.setMarkerArraySet(false);
            oneMarkerSet = false;
            int bytesRead = receiveTCPdata(); // Receive marker data via TCP
            if (bytesRead == readBufferLength){
                interpretTCPMarkerData(); // Interpret received data and fill markers[]
                if (oneMarkerSet) // This is set in interpretTCPMarkerData()
                    setupScene.setMarkerArraySet(true); // Notify setupScene that marker array for this frame has been set
            }else{
                // This error should only occur when the stream length (in bytes) is not set correctly on both sides
                Debug.LogError("Number of bytes read from stream NOT equal to buffer length!");
            }
        }
    }

    // Close the TCP connection if one has been established
    void OnApplicationQuit(){
        if (!socketReady)
            return;
        theStream.Close();
        mySocket.Close();
        socketReady = false;
    }

    IEnumerator WaitForSeconds(int t){
            yield return new WaitForSeconds(t);
    }
}