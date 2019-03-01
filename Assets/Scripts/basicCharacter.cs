using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class basicCharacter : MonoBehaviour {

	public int pv = 100;
	public float speed = 0.5f;
	public bool oustideGroundOn = false;
	public bool pushEffectOn = false;
	private float pushEffectTime;
	private int areaExitDamagePerSeconds = 0;
	private float previousGroundDamageTime;
	private float actualTime;
	private float lastAttackTime;

	private GameObject fireballprefab;

	// Use this for initialization
	void Start () {
		lastAttackTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		actualTime = Time.time;
		if (oustideGroundOn)
		{
			if ((actualTime - previousGroundDamageTime) >= 1f)
			{
				previousGroundDamageTime = actualTime;
				this.pv -= this.areaExitDamagePerSeconds;
			}
		}
		if (pushEffectOn && (actualTime - pushEffectTime >1) )
		{
			pushEffectOn = false;
			this.gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
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
		this.previousGroundDamageTime = Time.time;
	}

	public void attack()
	{
		if ((Time.time - this.lastAttackTime) >= 1f) {
			this.lastAttackTime = Time.time;

			GameObject ball;
			ball = Instantiate (AssetDatabase.LoadAssetAtPath<GameObject> ("Assets/Prefabs/fireball.prefab"), transform);
			Vector2 direction = GameObject.Find ("boss").GetComponent<Transform> ().position - this.gameObject.transform.position;
			ball.GetComponent<Rigidbody2D> ().velocity = ball.GetComponent<fireBall>().speed * direction.normalized;
			ball.GetComponent<fireBall> ().origin = this.gameObject.name;
		}
	}

	public void push(GameObject fireball)
	{
		this.pushEffectOn = true;
		Vector2 pushDirection = this.transform.position - fireball.transform.position;
		this.gameObject.GetComponent<Rigidbody2D> ().velocity = fireball.GetComponent<fireBall> ().speed * pushDirection.normalized;
		this.pushEffectTime = Time.time;
	}
}
