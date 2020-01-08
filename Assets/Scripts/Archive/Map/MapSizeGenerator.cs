using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using XDaddy.Character;

public class MapSizeGenerator : MonoBehaviour {

    [Tooltip ("nom du fichier contenant le Tile à utilisé (exemple 'ground' pour ground.assset)")   ]
    public string TileName = "";

    protected Tile tile;
    protected Tilemap map;
    protected TilemapCollider2D mapCollider = new TilemapCollider2D();

    public void InitTileMap()
    {
        map = this.gameObject.GetComponent<Tilemap>();
        tile = Resources.Load<Tile>(TileName);
        mapCollider = new TilemapCollider2D();

        mapCollider = this.gameObject.AddComponent<TilemapCollider2D>() as TilemapCollider2D;
        mapCollider.isTrigger = true;
    }

    public void OuterBoxFill(Vector2Int innerSize, Vector2Int outerSize)
    {
        /*
         * We fill 4 boxes with 6 points :
         * 1 : A->B
         * 2 : C->D
         * 3 : C->E
         * 4 : F->B
         * 
         *      A-------------------------------------------+
         *      |                   1                       |
         *      +---E-----------------------------------+---B
         *      |   |                                   |   |
         *      |   |                                   |   |
         *      |   |                                   |   |
         *      | 3 |                                   | 4 |
         *      |   |                                   |   |
         *      |   |                                   |   |
         *      |   |                                   |   |
         *      C---+-----------------------------------F---+
         *      |                    2                      |
         *      +-------------------------------------------D
         *
        */
        Vector3Int A = new Vector3Int(-outerSize.x, outerSize.y, 0);
        Vector3Int B = new Vector3Int(outerSize.x, innerSize.y, 0);
        Vector3Int C = new Vector3Int(-outerSize.x, -innerSize.y, 0);
        Vector3Int D = new Vector3Int(outerSize.x, -outerSize.y, 0);
        Vector3Int E = new Vector3Int(-innerSize.x, innerSize.y, 0);
        Vector3Int F = new Vector3Int(innerSize.x, -innerSize.y, 0);

        BoxFill(A, B);
        BoxFill(C, D);
        BoxFill(C, E);
        BoxFill(F, B);
    }

    public void BoxFill(Vector3Int start, Vector3Int end)
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
    public void BoxFill(Vector3 start, Vector3 end)
    {
        BoxFill(map.WorldToCell(start), map.WorldToCell(end));
    }

}
