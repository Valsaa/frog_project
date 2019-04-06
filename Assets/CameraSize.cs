using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSize : MonoBehaviour {

	[Range (1, 8)]
	public int Zoom;

	// Use this for initialization
	void Start () {
		this.GetComponent<Camera> ().orthographicSize = Screen.height / 2 / Zoom;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
