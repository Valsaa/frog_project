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
    private List<Sort> SortList;
    private float LastSizeUpdate;
	// Use this for initialization
	void Start () {

        this.Init();

        foreach(string sortName in SortFileNameList)
        {
            StreamReader file = new StreamReader("Assets/Data/Sorts/" + sortName + ".json", false);
            if (file.ToString() == "")
            {
                Debug.Log("can't get sort '" + sortName + "' can't open file (" + "Assets/Data/Sorts/" + sortName + ")");
                return;
            }
            SortList.Add(JsonUtility.FromJson<Sort>(file.ReadToEnd()));
        }

        UpdateDangerZone();

        LastSizeUpdate = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
		if( (Time.time - LastSizeUpdate) >= SizeUpdatePeriod)
        {
            Delta -= SizeReduction;
            UpdateDangerZone();
        }
	}

    void UpdateDangerZone()
    {
        this.OuterBoxFill(new Vector2Int(initialTileSize-Delta, initialTileSize-Delta), new Vector2Int(initialTileSize, initialTileSize));

        
    }
}
