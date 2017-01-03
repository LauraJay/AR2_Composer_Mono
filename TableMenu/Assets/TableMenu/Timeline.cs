using UnityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Timeline : MonoBehaviour {
	public DirectoryInfo currentDirectory;
	public FileInformation[] files;
	//public Rect r;
	public GameObject TimeLineScrollView;
	public GameObject prefabButton;
	// Use this for initialization


	public void initTimeline(){

		string dir = Application.dataPath + "/saves/";
		currentDirectory = new DirectoryInfo(dir);
		FileInfo[] fia = currentDirectory.GetFiles();
		//FileInfo[] fia = searchDirectory(di,searchPattern);
		files = new FileInformation[fia.Length];
		int count=0;
		for(int f=0;f<fia.Length;f++){
			files[f] = new FileInformation(fia[f]);
			//Debug.Log (files[f].fi.Name);
			if(!files[f].fi.Name.Contains("meta")&&files[f].fi.Name.Contains(".xml")){
				count++;
				GameObject button = (GameObject)Instantiate(prefabButton.gameObject);
				button.name = files [f].fi.Name;
				button.GetComponentInChildren<Text>().text =button.name;
				button.transform.SetParent(TimeLineScrollView.transform, false);
				button.transform.localPosition = new Vector3 (0, -count * 40,0);
				Debug.Log ("reading file: " + button.name);
			}

		}

		prefabButton.SetActive (false);
		//Debug.Log (files.ToString);
		/*
		//FileInfo[] fia = searchDirectory(di,searchPattern);
		files = new FileInformation[fia.Length];
		for(int f=0;f<fia.Length;f++){
			if(fileTexture)
				files[f] = new FileInformation(fia[f],fileTexture);
			else
				files[f] = new FileInformation(fia[f]);
			*/


	}


	void Start () {
		initTimeline ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
