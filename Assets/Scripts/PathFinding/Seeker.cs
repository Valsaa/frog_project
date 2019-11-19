using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class Seeker : MonoBehaviour
{
    static Seeker instance;
    

    void Awake()
    {
        instance = this;
    }
	
	void Update ()
    {
		
	}


    public void RequestPath(PathRequest request, OnPathDelegate callback)
    {
    }

}
