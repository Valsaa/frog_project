using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacter : MonoBehaviour {

    public Stats baseStats;

    public Stats finalStats;

    protected List<Effect> effectList;

    public List<string> sortFileList;
    protected List<Sort> sortList;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected void UpdateEffects ()
    {
        foreach(Effect e in this.effectList)
        {
            if (e.lifeSpan > 0)
            {
                e.lifeSpan -= 1;
                this.ApplyEffect(e);
            }
            else this.effectList.Remove(e);
        }
    }

    protected void ApplyEffect(Effect e)
    {

    }

    public void AddEffect (Effect e)
    {
        this.ApplyEffect(e);
        if (e.lifeSpan > 0)
            this.effectList.Add(e);
    }

    public void RemoveEffect (Effect e)
    {
        this.effectList.Remove(e);
    }

    protected List<Sort> IsInRange(Vector3 ennemyPosition)
    {
        float minRange = Vector3.Distance(this.transform.position, ennemyPosition);
        List<Sort> sortsInRange = new List<Sort>();
        
        foreach (Sort s in sortList)
        {
            foreach (Projectile p in s.projectileList)
            {
                if(p.range > minRange || (p.lifeSpan*p.speed) > minRange)
                {
                    sortsInRange.Add(s);
                    break;
                }
            }
        }

        return sortsInRange;
    }
}
