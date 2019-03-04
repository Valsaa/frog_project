using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	private Rigidbody2D controller;
	private basicCharacter character;
	public enum Hypothese {FRGO_43_HYPOTHESE_1, FRGO_35_HYPOTHESE_2, FRGO_55_HYPOTHESE_3};
	public Hypothese controles;

	// mouse mouvement param
	Vector3 clicPosition = new Vector3(0f,0f);
	Vector3 currentPosition;
	float precision = 0.1f;
	float totalDistance = 0f;

	float TauntTime;

	private string[] phrases = {
		"salaud !",
		"meurt ignoble créature !",
		"tu pue !",
		"enculé !",
		"ta mère la pute !"
	};

	private enum Actions {NONE=0, EPEE=1, DASH=2, FIRE=3, TAUNT=4};
	private Actions selectAction;

	// Use this for initialization
	void Start () {
		selectAction = Actions.NONE;
		TauntTime = Time.time;
		clicPosition = currentPosition = getFeetPosition ();;
		this.character = this.gameObject.GetComponent<basicCharacter> ();
		this.controller = this.gameObject.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		switch (controles) {
		case Hypothese.FRGO_43_HYPOTHESE_1:
			F43H1 ();
			break;
		case Hypothese.FRGO_35_HYPOTHESE_2:
			F35H2 ();
			break;
		case Hypothese.FRGO_55_HYPOTHESE_3:
			F55H3 ();
			break;
		}
	}

	void F43H1()
	{
		/* mouvements :
		 **/
		Vector2 move = new Vector2 ();
		bool moveupdown = false;
		bool moverightleft = false;

		if (!(Input.GetKey (KeyCode.D) && Input.GetKey (KeyCode.Q))) {
			if (Input.GetKey (KeyCode.D)) {
				move.x += this.character.speed;
			} else if (Input.GetKey (KeyCode.Q)) {
				move.x -= this.character.speed;
			}
			moveupdown = true;
		}
		if (!(Input.GetKey (KeyCode.S) && Input.GetKey (KeyCode.Z))) {
			if (Input.GetKey (KeyCode.Z)) {
				move.y += this.character.speed;
			} else if (Input.GetKey (KeyCode.S)) {
				move.y -= this.character.speed;
			}
			moverightleft = true;
		}

		if (moveupdown && moverightleft) {
			move = move / 2;
		}

		if(!(this.character.pushEffectOn || this.character.dashEffectOn))
			this.controller.velocity = move;

		/* actions :
		 **/
		DirectAction ();
	}

	void F35H2()
	{
		MouseMouvement ();
		DirectAction ();
	}

	void F55H3()
	{
		MouseMouvement ();

		if (Input.GetKey (KeyCode.Alpha1)) { // epee
			this.selectAction = Actions.EPEE;
			GameObject.FindGameObjectWithTag ("persoAction").GetComponent<Text> ().text = "1: EPEE";
		}
		else if (Input.GetKey (KeyCode.Alpha2)) { // dash
			this.selectAction = Actions.DASH;
			GameObject.FindGameObjectWithTag ("persoAction").GetComponent<Text> ().text = "2: DASH";
		}
		else if (Input.GetKey (KeyCode.Alpha3)) { // fireball
			this.selectAction = Actions.FIRE;
			GameObject.FindGameObjectWithTag ("persoAction").GetComponent<Text> ().text = "3: FIRE";
		}
		else if (Input.GetKey (KeyCode.Alpha4)) { // say 'taunt'
			this.selectAction = Actions.TAUNT;
			GameObject.FindGameObjectWithTag ("persoAction").GetComponent<Text> ().text = "4: TAUNT";
		}
		else if (Input.GetKey (KeyCode.Alpha5)) { // say 'taunt'
			this.selectAction = Actions.NONE;
			GameObject.FindGameObjectWithTag ("persoAction").GetComponent<Text> ().text = "0: NONE";
		}


		if (Input.GetKey (KeyCode.Mouse0) && this.selectAction != Actions.NONE) {
			switch (selectAction) {
			case Actions.EPEE:
				this.character.cut ();
				break;
			case Actions.DASH:
				this.character.dash (Camera.main.ScreenToWorldPoint (Input.mousePosition));
				break;
			case Actions.FIRE:
				this.character.attack();
				break;
			case Actions.TAUNT:
				this.Taunt ();
				break;
			}
		}
	}

	void DirectAction()
	{
		if (Input.GetKey (KeyCode.Alpha1)) { // epee
			this.character.cut();
		}
		if (Input.GetKey (KeyCode.Alpha2)) { // dash
			this.character.dash();
		}
		if (Input.GetKey (KeyCode.Alpha3)) { // fireball
			this.character.attack();
		}
		if (Input.GetKey (KeyCode.Alpha4)) { // say 'taunt'
			Taunt();
		}

	}

	void Taunt()
	{
		if ((Time.time - TauntTime) >= 3f) {
			TauntTime = Time.time;
			GameObject.FindGameObjectWithTag ("TauntUI").GetComponent<Text>().text = phrases [(int)Mathf.Floor ((Random.value - 0.0001f) * 5)];
		}
	}

	void MouseMouvement()
	{
		Vector2 move = new Vector2 ();

		if (Input.GetMouseButtonDown (1)) {
			// get clic point in the world
			clicPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			clicPosition.z = 0f;
		}

		// get player position with Y adjusted to feet
		currentPosition = this.getFeetPosition();

		// get distance between player and clic position
		totalDistance = Vector3.Distance (currentPosition, clicPosition);

		if (totalDistance > precision) {
			move = clicPosition - currentPosition;
			move = this.character.speed * move.normalized;
		} else
			move = Vector2.zero;

		if (this.character.pushEffectOn)
			this.clicPosition = currentPosition;
		else
			this.controller.velocity = move;
	}

	Vector3 getFeetPosition()
	{
		Vector3 temp = this.transform.position;
		temp.y -= this.GetComponent<BoxCollider2D> ().size.y / 2;
		return temp;
	}
}
