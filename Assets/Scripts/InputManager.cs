using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public Vector3 clicPosition;
	public bool replayModeOn = false;
	public bool saveModeOn = false;
	public string replayFilepath = "";

	// Use this for initialization
	void Start () {
		this.clicPosition = new Vector3 (0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		this.ReadMouvement ();
	}

	private void ReadMouvement()
	{
		if (Input.GetMouseButtonDown (0))
		{
			Debug.Log ("clic");
			this.clicPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			this.clicPosition.z = 0;
			Debug.Log (this.clicPosition);
		}
	}
}
