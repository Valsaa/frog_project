using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicCharacter : MonoBehaviour {

	public int pv = 100;
	public bool oustideGroundOn = false;
	private int areaExitDamagePerSeconds = 0;
	private float previousTime;
	private float actualTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (oustideGroundOn)
		{
			actualTime = Time.time;
			if ((actualTime - previousTime) >= 1f)
			{
				previousTime = actualTime;
				this.pv -= this.areaExitDamagePerSeconds;
			}
		}
	}

	public void SetOutsideStatus(bool status, int DPS)
	{
		this.oustideGroundOn = status;
		this.areaExitDamagePerSeconds = DPS;
		this.actualTime = this.previousTime = Time.time;
	}
}
