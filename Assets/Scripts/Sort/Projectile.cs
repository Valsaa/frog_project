using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Projectile
{
    public string name;

    public float lifeSpan;
    public float range;
    public float speed;

    public List<Effect> effectList = new List<Effect>();

    public Projectile()
    {

    }
    

}