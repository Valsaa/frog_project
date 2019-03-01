﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class basicCharacter : MonoBehaviour {

	public int pv = 100;
	public float speed = 0.5f;
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
		if (this.pv <= 0)
		{
			SceneManager.LoadScene (0);
		}
	}

	public void SetOutsideStatus(bool status, int DPS)
	{
		this.oustideGroundOn = status;
		this.areaExitDamagePerSeconds = DPS;
		this.actualTime = this.previousTime = Time.time;
	}
}
