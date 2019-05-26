using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Stats {

    public int health = 0;      // [U]
    public int knockBack = 0;   // [U]
    public float speed = 0f;    // [px]/[s]

    public void Check()
    {
        health = NotLessThanZero(health);
        knockBack = NotLessThanZero(knockBack);
        speed = NotLessThanZero(speed);
    }

    private int NotLessThanZero(int value)
    {
        return value < 0 ? 0 : value;
    }
    
    private float NotLessThanZero(float value)
    {
        return value < 0f ? 0 : value;
    }
}
