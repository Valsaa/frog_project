using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using XDaddy.Character;

public class MapSizeGenerator : MonoBehaviour {

    public int mapMultiplier = 0;

    // Use this for initialization
    void Start () {
        Tile ground = Resources.Load<Tile>("ground");
        Tilemap map =  this.gameObject.GetComponent<Tilemap>();
        map.ClearAllTiles();

        float PlayerSize = GameObject.Find("MainCharacter").GetComponent<SpriteRenderer>().sprite.bounds.extents.y;
        int mapsize = (int)Mathf.Floor(mapMultiplier * (PlayerSize * 2) / (ground.sprite.bounds.extents.y * 2) /2); // on divise par 2, on va utilisé -/+ au lieu de 0/+
        //BoxFill(map, ground, new Vector3Int(-mapsize, -mapsize, 0), new Vector3Int(mapsize, mapsize, 0));
        Debug.Log("created map :"+mapsize);
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BoxFill(Tilemap map, TileBase tile, Vector3Int start, Vector3Int end)
    {
        //Determine directions on X and Y axis
        var xDir = start.x < end.x ? 1 : -1;
        var yDir = start.y < end.y ? 1 : -1;
        //How many tiles on each axis?
        int xCols = 1 + Mathf.Abs(start.x - end.x);
        int yCols = 1 + Mathf.Abs(start.y - end.y);
        //Start painting
        for (var x = 0; x < xCols; x++)
        {
            for (var y = 0; y < yCols; y++)
            {
                var tilePos = start + new Vector3Int(x * xDir, y * yDir, 0);
                map.SetTile(tilePos, tile);
            }
        }
    }

    //Small override, to allow for world position to be passed directly
    public void BoxFill(Tilemap map, TileBase tile, Vector3 start, Vector3 end)
    {
        BoxFill(map, tile, map.WorldToCell(start), map.WorldToCell(end));
    }
}
