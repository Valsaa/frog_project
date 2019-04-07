using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MapSizeGenerator
{
    private int Delta = 0;
    public List<string> SortFileList;
    public List<Sort> SortList;
	// Use this for initialization
	void Start () {

        this.Init();

        foreach(string sortName in SortFileList)
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
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateDangerZone()
    {
        this.OuterBoxFill(new Vector2Int(initialTileSize-Delta, initialTileSize-Delta), new Vector2Int(initialTileSize, initialTileSize));

        
    }
}
