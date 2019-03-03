using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playArea : MonoBehaviour {

	private float previousTime;
	private float actualTime;
	private Vector2 currentScale;
	public int areaReductionPeriodSeconds = 10;
	public float areaReductionScale = 1f;
	public int areaExitDamagePerSeconds = 5;

	// Use this for initialization
	void Start () {
		previousTime = actualTime = Time.time;
		currentScale = this.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		actualTime = Time.time;
		if ((actualTime - previousTime) >= areaReductionPeriodSeconds)
		{
			previousTime = actualTime;
			if (currentScale.x > 10 && currentScale.y > 10)
			{
				currentScale = new Vector2 (currentScale.x - areaReductionScale, currentScale.y - areaReductionScale);
				this.transform.localScale = currentScale;
			}
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.gameObject.name == "boss" || col.gameObject.name == "perso")
		{
			col.gameObject.GetComponent<basicCharacter> ().SetOutsideStatus (true, this.areaExitDamagePerSeconds);
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.name == "boss" || col.gameObject.name == "perso")
		{
			col.gameObject.GetComponent<basicCharacter> ().SetOutsideStatus (false, 0);
		}
	}
}
