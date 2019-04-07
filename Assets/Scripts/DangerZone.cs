using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MapSizeGenerator
{
    private int Delta = 0;
	// Use this for initialization
	void Start () {

        this.Init();

        UpdateDangerZone();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateDangerZone()
    {
        this.OuterBoxFill(new Vector2Int(initialTileSize-Delta, initialTileSize-Delta), new Vector2Int(initialTileSize, initialTileSize));
    }
}
