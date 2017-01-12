using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneLoad : MonoBehaviour {

    public void Close(string sceneClose){
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(sceneClose).buildIndex);
    }

    public void Open(string sceneOpen){
       SceneManager.LoadScene(sceneOpen, LoadSceneMode.Additive);
    }
        
}

