using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MapSizeGenerator
{
    [ Tooltip ("en secondes") ]
    public int SizeUpdatePeriod;
    [ Tooltip ("par cran de 64px") ]
    public int SizeReduction;
    [ Tooltip ("nom du fichier sans le .json") ]
    public List<string> SortFileNameList;

    private int Delta = 0;
    public List<Sort> SortList;
    private float LastSizeUpdate = 0;
	// Use this for initialization
	void Start () {

        this.Init();

        foreach(string sortName in SortFileNameList)
        {
            StreamReader file = new StreamReader("Assets/Data/Sorts/" + sortName + ".json", false);
            if (file == null || file.ToString() == "")
            {
                Debug.Log("can't get sort '" + sortName + "' can't open file (" + "Assets/Data/Sorts/" + sortName + ")");
            }
            else SortList.Add(JsonUtility.FromJson<Sort>(file.ReadToEnd()));
        }
        UpdateDangerZone();
    }
	
	// Update is called once per frame
	void Update () {
		if( (Time.time - LastSizeUpdate) >= SizeUpdatePeriod)
        {
            Delta += SizeReduction;
            UpdateDangerZone();
            LastSizeUpdate = Time.time;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    void UpdateDangerZone()
    {
        // update the DangerZone area TileMap and Colliders
        this.OuterBoxFill(new Vector2Int(initialTileSize-Delta, initialTileSize-Delta), new Vector2Int(initialTileSize, initialTileSize));
    }
}
