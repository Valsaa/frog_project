using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotDangerZone : MapSizeGenerator
{
    private int currentTileSize;

    // Use this for initialization
    void Start () {

        this.Init();

        this.BoxFill(new Vector3Int(-initialTileSize, -initialTileSize, 0), new Vector3Int(initialTileSize, initialTileSize, 0));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
