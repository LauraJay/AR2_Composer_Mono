using UnityEngine;
using System.Collections;

public class size : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public void changeX( Vector3 translation)
	{
		this.gameObject.transform.localScale += new Vector3(translation.x, 0,0);
		Debug.Log ("Dragging X!");
	}

	public void changeY( Vector3 translation)
	{
		this.gameObject.transform.localScale += new Vector3(0, translation.y,0);
		Debug.Log ("Dragging Y!");
	}

	public void changeZ( Vector3 translation)
	{
		this.gameObject.transform.localScale += new Vector3(0, 0, translation.z);
		Debug.Log ("Dragging Z!");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
