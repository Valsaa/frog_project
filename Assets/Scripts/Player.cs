using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private Rigidbody2D controller;
	private basicCharacter character;
	public bool useMouse = false;

	// mouxe mouvement param
	Vector3 clicPosition = new Vector3(0f,0f);
	Vector3 currentPosition;
	float precision = 0.1f;
	float totalDistance = 0f;

	// Use this for initialization
	void Start () {
		this.character = this.gameObject.GetComponent<basicCharacter> ();
		this.controller = this.gameObject.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.character.pushEffectOn)
		{
			if (useMouse)
				this.MouseMouvement ();
			else
				this.KeyboardMouvement ();

			if (Input.GetKey (KeyCode.Space))
				this.character.attack ();
		}
	}

	void KeyboardMouvement()
	{
		Vector2 move = new Vector2 ();
		bool moveupdown = false;
		bool moverightleft = false;

		if (!(Input.GetKey (KeyCode.D) && Input.GetKey (KeyCode.Q))) {
			if (Input.GetKey (KeyCode.D)) {
				move.x += this.character.speed * Time.deltaTime;
			} else if (Input.GetKey (KeyCode.Q)) {
				move.x -= this.character.speed * Time.deltaTime;
			}
			moveupdown = true;
		}
		if (!(Input.GetKey (KeyCode.S) && Input.GetKey (KeyCode.Z))) {
			if (Input.GetKey (KeyCode.Z)) {
				move.y += this.character.speed * Time.deltaTime;
			} else if (Input.GetKey (KeyCode.S)) {
				move.y -= this.character.speed * Time.deltaTime;
			}
			moverightleft = true;
		}

		if (moveupdown && moverightleft) {
			move = move / 2;
		}

		this.controller.position += move;

		this.controller.velocity = Vector2.zero;
	}

	void MouseMouvement()
	{

		if (Input.GetMouseButtonDown (0)) {
			// get clic point in the world
			clicPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			clicPosition.z = 0f;
		}

		// get player position with Y adjusted to feet
		currentPosition = this.transform.position;
		currentPosition.y -= this.GetComponent<BoxCollider2D> ().size.y / 2;

		// get distance between player and clic position
		totalDistance = Vector3.Distance (currentPosition, clicPosition);

		if (totalDistance > precision) {
			Vector2 move = clicPosition - currentPosition;
			this.controller.velocity = this.character.speed * move.normalized;
		} else
			this.controller.velocity = Vector2.zero;
	}
}
