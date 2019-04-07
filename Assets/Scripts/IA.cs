using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour {

    public enum Status { NULL_0, DEP_OFF_1, AGGRO_2, DEP_DEF_3, ESQ_4, DEP_MAP_5};
    public Status currentStatus;

	// Use this for initialization
	void Start () {
        currentStatus = Status.NULL_0;

    }
	
	// Update is called once per frame
	void Update () {
		switch(currentStatus)
        {
            default:
            case Status.NULL_0:
                // do stuff

                // check stuff
                break;
            case Status.DEP_OFF_1:
                break;
            case Status.AGGRO_2:
                break;
            case Status.DEP_DEF_3:
                break;
            case Status.ESQ_4:
                break;
            case Status.DEP_MAP_5:
                break;
        }
	}
}
