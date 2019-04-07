using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour {

    public enum CameraPositionBehaviours { FOLLOW_MAIN_CHARACTER };
    public CameraPositionBehaviours cameraBehaviour = CameraPositionBehaviours.FOLLOW_MAIN_CHARACTER;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		switch(cameraBehaviour)
        {
            default:
            case CameraPositionBehaviours.FOLLOW_MAIN_CHARACTER:
                Vector3 PlayerPosition = GameObject.Find("MainCharacter").GetComponent<Transform>().position;
                Vector3 CameraPosition = this.GetComponent<Transform>().position;
                this.GetComponent<Transform>().position = new Vector3(PlayerPosition.x, PlayerPosition.y, CameraPosition.z);
                break;
        }
	}
}
