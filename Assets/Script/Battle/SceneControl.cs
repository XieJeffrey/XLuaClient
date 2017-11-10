using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour {
    // Use this for initialization
    public static SceneManager sceneManager;
	void Start () {
        sceneManager=gameObject.AddComponent<SceneManager>();
        sceneManager.Parent = transform;
	}
}
