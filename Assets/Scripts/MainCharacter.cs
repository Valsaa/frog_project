using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour {

	private Rigidbody2D controller;
	private InputManager inputManager;
	public float speed = 0.1f;

	// Use this for initialization
	void Start () {
		this.controller = GetComponent<Rigidbody2D> ();
		this.inputManager = GetComponent<InputManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 mouvement = new Vector2 ();

		if (!(this.inputManager.mouvement.up && this.inputManager.mouvement.down))
		{
			Debug.Log ("u="+this.inputManager.mouvement.up + "d=" + this.inputManager.mouvement.down);
			if (this.inputManager.mouvement.up) {
				mouvement.y += this.speed * Time.deltaTime;
			}
			else if (this.inputManager.mouvement.down) {
				mouvement.y -= this.speed * Time.deltaTime;
			}
		}
		if (!(this.inputManager.mouvement.right && this.inputManager.mouvement.left))
		{
			Debug.Log ("r="+this.inputManager.mouvement.right + "l=" + this.inputManager.mouvement.left);
			if (this.inputManager.mouvement.right) {
				mouvement.x += this.speed * Time.deltaTime;
			}
			else if (this.inputManager.mouvement.left) {
				mouvement.x -= this.speed * Time.deltaTime;
			}
		}
		Debug.Log ("m="+mouvement);
		this.controller.position += mouvement;
	}
}
