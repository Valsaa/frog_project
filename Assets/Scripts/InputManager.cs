using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	public struct Mouvement{
		public bool up;
		public bool down;
		public bool left;
		public bool right;
	};
	public Mouvement mouvement;
	public bool replayModeOn = false;
	public bool saveModeOn = false;
	public string replayFilepath = "";

	// Use this for initialization
	void Start () {
		// setup
		this.mouvement.up = false;
		this.mouvement.down = false;
		this.mouvement.left = false;
		this.mouvement.right = false;
	}
	
	// Update is called once per frame
	void Update () {
		this.ReadMouvement ();
	}

	private void ReadMouvement()
	{
		this.mouvement.right = Input.GetKey (KeyCode.D);
		this.mouvement.left = Input.GetKey (KeyCode.Q);
		this.mouvement.up = Input.GetKey (KeyCode.Z);
		this.mouvement.down = Input.GetKey (KeyCode.S);
	}
}
