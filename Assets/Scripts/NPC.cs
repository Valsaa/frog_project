using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

	public float timeBetweenAttackSeconds = 3;
	private float timeBeforeNextMove = 0;
	private float lastAttackTime;
	private float actualTime;
	private basicCharacter character;
	private Rigidbody2D controller;
	// Use this for initialization
	void Start () {
		actualTime = lastAttackTime = Time.time;
		this.character = this.gameObject.GetComponent<basicCharacter> ();
		this.controller = this.gameObject.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		actualTime = Time.time;

		if ((actualTime - lastAttackTime) >= timeBetweenAttackSeconds)
		{
			lastAttackTime = actualTime;
			this.character.attack ();
		}
		if (!this.character.pushEffectOn)
		{
			if(actualTime > timeBeforeNextMove || this.character.oustideGroundOn)
				this.move ();
		}
	}

	void move()
	{
		Vector2 Direction = new Vector2 (Random.value*2-1, Random.value*2-1);
		this.controller.velocity = Direction.normalized * this.character.speed*2;
		timeBeforeNextMove = Random.value * 3 + 2 + Time.time;
	}
}
