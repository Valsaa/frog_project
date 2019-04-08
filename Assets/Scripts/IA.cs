using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : BasicCharacter {

    public enum Status { NULL_0, DEP_OFF_1, AGGRO_2, DEP_DEF_3, ESQ_4, DEP_MAP_5};
    public Status currentStatus;
    private GameObject player = GameObject.Find("MainCharacter");

    private List<Sort> availableSorts;

    // Use this for initialization
    void Start ()
    {
        currentStatus = Status.NULL_0;

    }
	
	// Update is called once per frame
	void Update ()
    {
		switch(currentStatus)
        {
            default:
            case Status.NULL_0:
                currentStatus = Status.DEP_OFF_1;
                break;
            case Status.DEP_OFF_1:
                // on se déplace vers le joueur
                this.MoveToPoint(player.transform.position);

                availableSorts = IsInRange(player.transform.position);

                if (availableSorts.Count > 0)
                    currentStatus = Status.AGGRO_2;
                
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

    private void FixedUpdate()
    {
        
    }

    private void MoveToPoint(Vector3 point)
    {
        
    }
}
