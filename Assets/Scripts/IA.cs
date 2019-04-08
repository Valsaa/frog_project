using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : BasicCharacter {

    public enum Status { NULL_0, DEP_OFF_1, AGGRO_2, DEP_DEF_3, ESQ_4, DEP_MAP_5};
    public Status currentStatus;
    private GameObject player;

    private List<Sort> availableSorts;
    Vector3 projectileToEsquive = new Vector3();

    private void Awake()
    {
        player = GameObject.Find("MainCharacter");
        sortList = Sort.GetSortListFromFileList(sortFileList);
    }

    // Use this for initialization
    void Start ()
    {
        currentStatus = Status.NULL_0;

    }
	
	// Update is called once per frame
	void Update ()
    {
        availableSorts = IsInRange(player.transform.position);

        switch (currentStatus)
        {
            default:
            case Status.NULL_0:
                currentStatus = Status.DEP_OFF_1;
                break;
            case Status.DEP_OFF_1:
                // on se déplace vers le joueur
                this.MoveToPoint(player.transform.position);

                if (availableSorts.Count > 0)
                    currentStatus = Status.AGGRO_2;
                
                break;
            case Status.AGGRO_2:
                bool attaqued = false;
                foreach(Sort s in availableSorts)
                {
                    if((Time.time-s.lastUsed) > s.coolDown)
                    {
                        // TODO : attaque
                        s.lastUsed = Time.time;
                        attaqued = true;
                        break;
                    }
                }
                if (!attaqued)
                    currentStatus = Status.DEP_DEF_3;
                
                break;
            case Status.DEP_DEF_3:
                Vector3 playerOppositPoint = this.transform.position - player.transform.position;
                this.MoveToPoint(playerOppositPoint);
                if (IsPlayerAttacking())
                {
                    projectileToEsquive = player.transform.GetChild(i).transform.position;
                    currentStatus = Status.ESQ_4;
                }

                if (currentStatus == Status.DEP_DEF_3)
                    currentStatus = Status.AGGRO_2;
                break;
            case Status.ESQ_4:
                Vector3 esquive = transform.position - projectileToEsquive;
                this.MoveToPoint(this.transform.position + new Vector3(esquive.y,esquive.x,esquive.z));

                foreach (Effect e in effectList)
                {
                    if (e.name == "OutDamage")
                    {
                        currentStatus = Status.DEP_MAP_5;
                        break;
                    }
                    else currentStatus = Status.AGGRO_2;
                }

                break;
            case Status.DEP_MAP_5:
                this.MoveToPoint(new Vector3(0, 0, 0));
                bool onFire = false;
                foreach (Effect e in effectList)
                {
                    if (e.name == "OutDamage")
                        onFire = true;
                }
                if (!onFire) currentStatus = Status.DEP_OFF_1;
                else if (IsPlayerAttacking())
                {
                    projectileToEsquive = player.transform.GetChild(i).transform.position;
                    currentStatus = Status.ESQ_4;
                }
                break;

        }
	}

    private bool IsPlayerAttacking()
    {
        if (player.gameObject.transform.childCount > 0)
        {
            for (int i = 0; i < player.gameObject.transform.childCount; i++)
            {
                if (player.transform.GetChild(i).name.Contains("sort"))
                    return true;
            }
        }
        return false;
    }

    private void FixedUpdate()
    {
        
    }

    private void MoveToPoint(Vector3 point)
    {
        
    }
}
