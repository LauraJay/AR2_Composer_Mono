using UnityEngine;
using System;
using System.Net.Sockets;
using UnityEngine.UI;

public class readInNetworkData : MonoBehaviour {
    Boolean socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    byte[] readBuffer;
    byte[] writeBuffer;
    int readBufferLength;
    int writeBufferLength;
    int writeStatus;
    Marker[] markers;
    long frameCounter = 0;
    bool oneMarkerSet = false;
    setupScene setupScene;
    Text TCPText;

    [Header("Socket Settings")]
    public String Host = "192.168.0.5";
    public Int32 Port = 10000;
    [Header("Data Stream Settings")]
    public int maxMarkerCount = 1;
    public int bytesPerMarker = 20;

    // Return markers array (used by setupScene.cs)
    public Marker[] getMarkers() {
        return markers;
    }

    // Initialization
    void Start(){
        TCPText = GameObject.Find("TCPText").GetComponent<Text>();
        readBufferLength = bytesPerMarker * maxMarkerCount + 4; // +4 because ID=-1 marks end of frame
        writeBufferLength = 4;
        writeStatus = 0;
        markers = new Marker[maxMarkerCount + 1];
        setupScene = gameObject.GetComponent<setupScene>();
        setupSocket();
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

    // Is called once every frame
    void Update(){
        setupScene.setMarkerArraySet(false);
        oneMarkerSet = false;
        // Is the socket ready?
        if (socketReady){
            writeBuffer = System.BitConverter.GetBytes(writeStatus);
            theStream.Write(writeBuffer, 0, writeBufferLength);

            // Is the socket ready and does it have data waiting?
            if (socketReady && theStream.DataAvailable){
                Debug.Log("Socket is ready and stream data is available.");
                readBuffer = new byte[readBufferLength];
                int bytesRead = theStream.Read(readBuffer, 0, readBufferLength); // Read socket
                if (bytesRead == readBufferLength){ // Number of bytes read equal to expected number?
                    Debug.Log("bytesRead is equal to bufferLength.");
                    for (int i = 0; i < readBufferLength; i += bytesPerMarker){
                        int curID = System.BitConverter.ToInt32(readBuffer, i); // ID
                        if (curID == -1){ // End of frame reached?
                            Debug.Log("Last masker reached, suspending loop for current frame " + frameCounter + ".");
                            frameCounter++;
                            markers[i / bytesPerMarker + 1] = new Marker(-1, 0.0f, 0.0f, 0.0f, 0);
                            break;
                        }
                        else if (curID < 0){
                            Debug.LogError("Marker ID not valid: " + curID);
                        }
                        else{
                            float curPosX = System.BitConverter.ToSingle(readBuffer, i + 4); // X-position
                            float curPosY = System.BitConverter.ToSingle(readBuffer, i + 8); // Y-position
                            float curAngle = System.BitConverter.ToSingle(readBuffer, i + 12); // Angle
                            int status = System.BitConverter.ToInt32(readBuffer, i + 16); // isVisible
                            markers[i / bytesPerMarker] = new Marker(curID, curPosX, curPosY, curAngle, status); // Add new marker to array
                            oneMarkerSet = true;
                            //Debug.Log(markers[i / bytesPerMarker].toStr()); // Print debug message containing marker data
                            TCPText.text = markers[i / bytesPerMarker].toStr();
                        }
                    }
                    if (oneMarkerSet)
                        setupScene.setMarkerArraySet(true);
                }
                else
                {
                    Debug.LogError("Number of bytes read from stream NOT equal to buffer length!");
                }
            }
        }
    }

    // Wrap up
    void OnApplicationQuit(){
        if (!socketReady)
            return;
        theStream.Close();
        mySocket.Close();
        socketReady = false;
    }
}