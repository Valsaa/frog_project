using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sort {

    public string name;

    public float coolDown;

    public List<Effect> effectList = new List<Effect> ();
    public List<Projectile> projectileList = new List<Projectile> ();

    public Sort()
    {

    }

}
