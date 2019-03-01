using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private Rigidbody2D controller;
	private basicCharacter character;
	public bool useMouse = false;
	// Use this for initialization
	void Start () {
		this.character = this.gameObject.GetComponent<basicCharacter> ();
		this.controller = this.gameObject.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (useMouse)
			this.MouseMouvement ();
		else
			this.KeyboardMouvement ();
	}

	void KeyboardMouvement()
	{
		Vector2 move = new Vector2 ();

		if (!(Input.GetKey (KeyCode.D) && Input.GetKey (KeyCode.Q))) {
			if (Input.GetKey (KeyCode.D)) {
				move.x += this.character.speed * Time.deltaTime;
			} else if (Input.GetKey (KeyCode.Q)) {
				move.x -= this.character.speed * Time.deltaTime;
			}
		}
		if (!(Input.GetKey (KeyCode.S) && Input.GetKey (KeyCode.Z))) {
			if (Input.GetKey (KeyCode.Z)) {
				move.y += this.character.speed * Time.deltaTime;
			} else if (Input.GetKey (KeyCode.S)) {
				move.y -= this.character.speed * Time.deltaTime;
			}
		}

		this.controller.position += move;
	}

	void MouseMouvement()
	{

	}
}
