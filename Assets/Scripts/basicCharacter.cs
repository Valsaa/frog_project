using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class basicCharacter : MonoBehaviour {

	public int pv = 100;
	public float speed = 3f;
	public bool oustideGroundOn = false;
	public bool pushEffectOn = false;
	public bool dashEffectOn = false;
	private float pushEffectTime;
	private int areaExitDamagePerSeconds = 0;
	private float previousGroundDamageTime;
	private float actualTime;

	private float lastAttackTime;
	private float lastCutTime;
	private float startDashTime;

	private GameObject fireballprefab;

	// Use this for initialization
	void Start () {
		startDashTime = lastAttackTime = lastCutTime = Time.time;
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
		if(this.gameObject.name != "boss" && (Time.time - this.lastCutTime) >= 0.5f)
			this.gameObject.GetComponent<SpriteRenderer> ().sprite = AssetDatabase.LoadAssetAtPath<Sprite> ("Assets/Sprites/perso.png");
		if (dashEffectOn && (Time.time - this.startDashTime) >= 0.5f) {
			this.gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
			dashEffectOn = false;
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
		if ((Time.time - this.lastAttackTime) >= 3f) {
			this.lastAttackTime = Time.time;

			GameObject ball;
			ball = Instantiate (AssetDatabase.LoadAssetAtPath<GameObject> ("Assets/Prefabs/fireball.prefab"), transform);

			Vector2 direction;

			if (this.gameObject.name == "perso")
				direction = GameObject.Find ("boss").GetComponent<Transform> ().position - this.gameObject.transform.position;
			else if (this.gameObject.name == "boss")
				direction = GameObject.Find ("perso").GetComponent<Transform> ().position - this.gameObject.transform.position;
			else {
				Destroy (ball);
				return;
			}

			ball.GetComponent<Rigidbody2D> ().velocity = ball.GetComponent<fireBall>().speed * direction.normalized;
			ball.GetComponent<fireBall> ().origin = this.gameObject.name;
		}
	}

	public void cut()
	{
		if ((Time.time - this.lastCutTime) >= 1f) {
			this.lastCutTime = Time.time;

			this.gameObject.GetComponent<SpriteRenderer> ().sprite = AssetDatabase.LoadAssetAtPath<Sprite> ("Assets/Sprites/perso-epee.png");
		}
	}

	public void dash(Vector3 direction)
	{
		direction.z = 0f;
		applyDash(direction - this.gameObject.transform.position);
	}

	public void dash()
	{
		applyDash(this.gameObject.GetComponent<Rigidbody2D> ().velocity);
	}

	private void applyDash(Vector3 direction)
	{
		if (!this.pushEffectOn)
		{
			startDashTime = Time.time;
			this.gameObject.GetComponent<Rigidbody2D> ().velocity = direction.normalized * this.speed * 3;
			dashEffectOn = true;
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
