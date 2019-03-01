using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class basicCharacter : MonoBehaviour {

	public int pv = 100;
	public float speed = 0.5f;
	public bool oustideGroundOn = false;
	private int areaExitDamagePerSeconds = 0;
	private float previousTime;
	private float actualTime;
	private float lastAttackTime;
	public GameObject fireballprefab;

	// Use this for initialization
	void Start () {
		lastAttackTime = Time.time;
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
}
