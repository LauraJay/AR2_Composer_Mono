﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


public class save : MonoBehaviour {

	public XmlDocument doc;
	private String timeStamp; 
	string filepath;
	private  string projectPath;
	public XmlNode root;


	// Use this for initialization
	void Start () {
		projectPath =  Application.dataPath;
	}

	public void saveScene	(){
		
		String timeStamp = System.DateTime.Now.ToString();

		timeStamp = timeStamp.Replace("/", "-");
		timeStamp = timeStamp.Replace(":", "-");
		filepath  = Application.dataPath + "/saves/" + timeStamp +".xml"; 
		Debug.Log("TimeStamp: " + timeStamp);
		Debug.Log("Path: " + filepath);
		//Scene activeScene = EditorSceneManager.GetActiveScene();
		//SceneManager.UnloadScene (activeScene.buildIndex);

		//EditorSceneManager.SaveScene( activeScene );
		//EditorApplication.SaveAssets();

		//UnityEngine.Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject)); //returns Object[]
		doc = new XmlDocument();
		doc.LoadXml("<AR2COMPOSER_SCENE>" +
					"<time>" +  
						timeStamp  +
					"</time>" +
					"</AR2COMPOSER_SCENE>"); 
		root = doc.DocumentElement;


		Console.WriteLine("Display the modified XML document...");
		Console.WriteLine(doc.OuterXml);

        GameObject tableObject = GameObject.Find("TableObject");
        //Serialize (objects, filepath); 
        traverseHirarchy(tableObject, root);
		Debug.Log ("Objects crawled!");
		Debug.Log ("Path: " + filepath);
		doc.Save (filepath);
	}
	/*
	public static void Serialize(object item, string path) {
		XmlSerializer serializer = new XmlSerializer(item.GetType());
		StreamWriter writer = new StreamWriter(path);
		serializer.Serialize(writer.BaseStream, item);
		writer.Close();
	}
	*/


	void traverseHirarchy(GameObject obj, XmlNode parentNode){

        //if (obj.transform.childCount > 0) {
        if (!obj.name.Contains("TablePlane"))
        {
            Debug.Log(obj.name + " Mother of Childs");


            XmlNode newElem = doc.CreateNode("element", obj.name, "");

            XmlNode newName = doc.CreateNode("element", "Name", "");

            XmlNode newPosX = doc.CreateNode("element", "PositionX", "");
            XmlNode newPosY = doc.CreateNode("element", "PositionY", "");
            XmlNode newPosZ = doc.CreateNode("element", "PositionZ", "");

            XmlNode newRotationX = doc.CreateNode("element", "RotationX", "");
            XmlNode newRotationY = doc.CreateNode("element", "RotationY", "");
            XmlNode newRotationZ = doc.CreateNode("element", "RotationZ", "");

            XmlNode newScaleX = doc.CreateNode("element", "ScaleX", "");
            XmlNode newScaleY = doc.CreateNode("element", "ScaleY", "");
            XmlNode newScaleZ = doc.CreateNode("element", "ScaleZ", "");

            XmlNode active = doc.CreateNode("element", "Active", "");

            newName.InnerText = obj.name;
            newPosX.InnerText = obj.transform.localPosition.x.ToString();
            newPosY.InnerText = obj.transform.localPosition.y.ToString();
            newPosZ.InnerText = obj.transform.localPosition.z.ToString();

            newRotationX.InnerText = obj.transform.localEulerAngles.x.ToString();
            newRotationY.InnerText = obj.transform.localEulerAngles.y.ToString();
            newRotationZ.InnerText = obj.transform.localEulerAngles.z.ToString();

            newScaleX.InnerText = obj.transform.localScale.x.ToString();
            newScaleY.InnerText = obj.transform.localScale.y.ToString();
            newScaleZ.InnerText = obj.transform.localScale.z.ToString();

            active.InnerText = obj.activeSelf.ToString();

            foreach (Transform child in obj.transform)
            {
                if (!obj.name.Contains("TablePlane"))
                    traverseHirarchy(child.gameObject, newElem);
            }

            newElem.AppendChild(newName);

            newElem.AppendChild(newPosX);
            newElem.AppendChild(newPosY);
            newElem.AppendChild(newPosZ);

            newElem.AppendChild(newRotationX);
            newElem.AppendChild(newRotationY);
            newElem.AppendChild(newRotationZ);

            newElem.AppendChild(newScaleX);
            newElem.AppendChild(newScaleY);
            newElem.AppendChild(newScaleZ);

            newElem.AppendChild(active);

            parentNode.AppendChild(newElem);

        }

	}


	// Update is called once per fram
	void Update () {
		/*
		if (Input.GetMouseButtonDown (0)) {
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Collider2D hitCollider = Physics2D.OverlapPoint (mousePosition);

			Debug.Log ("mouse pos " + mousePosition.x + " y " + mousePosition.y + " ");    
			saveScene ();

			if(hitCollider){
				selectorSprite.transform.position.x = hitCollider.transform.position.x;
				selectorSprite.transform.position.y = hitCollider.transform.position.y;    
				Debug.Log("Hit "+hitCollider.transform.name+" x"+hitCollider.transform.position.x+" y "+hitCollider.transform.position.y);    
			}

		}*/
		
	}
}