using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showTimeline : MonoBehaviour {
    public GameObject ScrollView; 
	// Use this for initialization
	void Start () {
		
	}

   public void  showHideTimeLine( )
    {
        if (ScrollView.active == true)
        ScrollView.SetActive(false);
       else if (ScrollView.active == false)
            ScrollView.active = true;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
