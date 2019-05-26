using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacter : MonoBehaviour {

    [Tooltip("stats de base du character")]
    public Stats baseStats;
    [Tooltip("ne pas toucher")]
    public Stats finalStats;

    public List<Effect> effectList;

    public List<string> sortFileList;
    protected List<Sort> sortList;

    protected bool canMove = true;
    protected bool canPlay = true;

    protected void UpdateEffects ()
    {
        foreach(Effect e in this.effectList)
        {
            if (e.lifeSpan > 0)
            {
                e.lifeSpan -= 1;
                this.ReApplyEffect(e);
            }
            else this.effectList.Remove(e);
        }
    }

    protected void ApplyEffect(Effect e)
    {
        ReApplyEffect(e);
        finalStats.speed += e.altSpeed;
    }

    protected void ReApplyEffect(Effect e)
    {
        finalStats.health += e.altHealth;
        if ((finalStats.knockBack += e.altKnockBack) < 0)
        {
            this.PushBack(finalStats.knockBack);
            finalStats.knockBack = 0;
        }
        if (e.silence) canPlay = false;
        if(e.enracinement) canMove = false;
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

    public void PushBack(int knockbackValue)
    {

    }

    public void UpdateStats()
    {
        finalStats.health = baseStats.health;
        finalStats.knockBack = baseStats.knockBack;
        finalStats.speed = baseStats.speed;
    }
}
