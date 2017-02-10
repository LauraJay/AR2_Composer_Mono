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
    public int markersToReceive = 100; // This multiplied by bytesPerMarker has to match
    public int bytesPerMarker = 20;  // the length of the byte array that is sent over TCP
    public bool printMarkerDebugInfo = false;

    // TCP status enum for sending AND receiving statuses
    public enum TCPstatus { planeAndPoseCalib, planeOnlyCalib, sceneStart, planeCalibDone,
        poseCalibDone, controllerButtonPressed, arucoFound1, arucoFound2, arucoNotFound };
    private bool sceneStarted;

    // Return markers array (is called by setupScene.cs)
    public Marker[] getMarkers() {
        return markers;
    }

    // Is called by setupScene.cs
    public int getMarkersToReceive(){
        return markersToReceive;
    }

    // Initialization
    void Start(){
        readBufferLength = bytesPerMarker * markersToReceive + 4; // +4 because ID=-1 marks end of frame
        markers = new Marker[markersToReceive + 1];
        setupSocket();
        sceneStarted = false;
    }

    // This is called by the menu that starts the normal tracking and rendering operation
    public void setSceneStarted(bool status){
        sceneStarted = status;
    }

    // Set up and connect TCP socket
    private void setupSocket(){
        try{
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            socketReady = true;
            Debug.Log("[TCP] Socket set up successfully.");
        }catch (Exception e){
            Debug.LogError("[TCP] Socket setup failed. Error: " + e);
        }
    }

    // Send status over TCP according to TCPstatus enum
    public void sendTCPstatus(int status){
        if (socketReady) { 
            theStream.Write(System.BitConverter.GetBytes(status), 0, 4);
            Debug.Log("[TCP] Status sent: " + Enum.GetName(typeof(TCPstatus), status));
        }
        else
            Debug.LogError("[TCP] Failed to send status, because the socket is not ready: " + status);
    }

    // Receive status over TCP according to TCPstatus enum
    public int receiveTCPstatus(){
        if (socketReady){
            while (!theStream.DataAvailable){
                Debug.Log("[TCP] Waiting for status to be received.");
                System.Threading.Thread.Sleep(1000);
            }
            byte[] receivedBytes = new byte[4];
            theStream.Read(receivedBytes, 0, 4);
            int status = System.BitConverter.ToInt32(receivedBytes, 0);
            Debug.Log("[TCP] Status received: " + Enum.GetName(typeof(TCPstatus), status));
            return status;
        }
        Debug.LogError("[TCP] Failed to receive status, because the socket is not ready.");
        return -1;
    }

    // Returns the number of bytes that have been read from the stream in int
    private int receiveTCPdata(){
        if (socketReady && theStream.DataAvailable){
            readBuffer = new byte[readBufferLength];
            return theStream.Read(readBuffer, 0, readBufferLength);
        }
        return 0; // No data has been read
    }

    // Convert byte[] data received over TCP to usable marker data
    private void interpretTCPMarkerData(){
        for (int i = 0; i < readBufferLength; i += bytesPerMarker){
            int curID = System.BitConverter.ToInt32(readBuffer, i); // Convert the marker ID
            if (curID == 0 && printMarkerDebugInfo){
                Debug.Log("[READ IN NETWORK DATA] Start of frame " + frameCounter + ".");
                continue;
            }
            if (curID == -1) { // Marker is empty
                markers[i / bytesPerMarker] = new Marker(-1, 0.0f, 0.0f, 0.0f, 0);
                continue;
            }
            if (curID == -2){ // End of frame reached
                if(printMarkerDebugInfo)
                    Debug.Log("[READ IN NETWORK DATA] Last marker reached, suspending loop for current frame.");
                frameCounter++; // This is counted even if showMarkerDebugInfo is false, so that it can be enabled at any time
                markers[i / bytesPerMarker] = new Marker(-2, 0.0f, 0.0f, 0.0f, 0); // Set last marker as EOF (end of frame)
                break;                                                                 // and suspend loop
            }else if (curID < -2 || curID > markersToReceive){ // For debugging, this should not happen during normal operation
                Debug.LogError("[READ IN NETWORK DATA] Marker ID not valid: " + curID);
            }else{ // ID is valid and does not mark the end of the frame
                float curPosX = System.BitConverter.ToSingle(readBuffer, i + 4); // Convert the x-position
                float curPosY = System.BitConverter.ToSingle(readBuffer, i + 8); // Convert the y-position
                float curAngle = System.BitConverter.ToSingle(readBuffer, i + 12); // Conver the angle
                int status = System.BitConverter.ToInt32(readBuffer, i + 16); // Convert the status of the marker
                markers[i / bytesPerMarker] = new Marker(curID, curPosX, curPosY, curAngle, status); // Add new marker to array
                oneMarkerSet = true;    // Give permission to use marker array since at least
                                        // one marker has been set for the current frame
                TCPText.text = markers[i / bytesPerMarker].toStr(); // Set text on object menu canvas
                if (printMarkerDebugInfo)
                    Debug.Log(markers[i / bytesPerMarker].toStr()); // Print debug message containing marker data
            }
        }
    }

    void Update(){
        if (sceneStarted) { // Controlled by unity menu "thirdmenu"
            setupScene.setMarkerArraySet(false); // Reset for next frame
            oneMarkerSet = false;                // Reset for next frame
            if (receiveTCPdata() == readBufferLength){ // Receive marker data via TCP
                interpretTCPMarkerData(); // Interpret received data and fill markers[]
                if (oneMarkerSet) // This is set in interpretTCPMarkerData()
                    setupScene.setMarkerArraySet(true); // Notify setupScene that marker array for this frame has been set
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
}