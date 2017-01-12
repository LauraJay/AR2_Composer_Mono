using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class performAction : MonoBehaviour {

	public void setState(int state){
        FindObjectOfType<setupScene>().setState(state);
    }

}
