using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour {

	private Rigidbody2D controller;
	private InputManager inputManager;
	public float speed = 10f;
	public float precision = 0.1f;
	public float totalDistance = 0f;

	// Use this for initialization
	void Start () {
		this.controller = GetComponent<Rigidbody2D> ();
		this.inputManager = GetComponent<InputManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 currentPosition = this.transform.position;
		totalDistance = Vector3.Distance (currentPosition, this.inputManager.clicPosition);

		//if (currentPosition != this.inputManager.clicPosition)
		if( totalDistance > precision )
		{
			Debug.Log ("move "+totalDistance);
			float deltaDistance = this.speed * Time.deltaTime;
			float factor = deltaDistance / totalDistance;

			Vector2 mouvement = new Vector2 ();
			mouvement.x = factor * (this.inputManager.clicPosition.x - currentPosition.x);
			mouvement.y = factor * (this.inputManager.clicPosition.y - currentPosition.y);

			this.controller.position += mouvement;
		}
	}
}
