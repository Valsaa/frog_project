using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireBall : MonoBehaviour {

	public float speed = 5f;
	public int damage = 1;
	public string origin = "";
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.name == "perso" || col.gameObject.name == "boss")
		{
			if (col.gameObject.name != this.origin)
			{
				col.gameObject.GetComponent<basicCharacter> ().pv -= this.damage;

				// TODO : push effect

				Destroy (gameObject);
			}
		}
		if (col.gameObject.name == "void")
			Destroy (gameObject);
	}
}
