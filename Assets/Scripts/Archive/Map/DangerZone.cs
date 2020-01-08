using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MapSizeGenerator
{
    [Tooltip("Taille Initial externe de la zone en unitée du Player")]
    public int mapOutMultiplier = 0;
    [Tooltip("Taille Initial interne de la zone en unitée du Player")]
    public int mapInMultiplier = 0;
    [Tooltip("en secondes")]
    public int SizeUpdatePeriod;
    [Tooltip("par cran de 64px")]
    public int SizeReduction;
    [Tooltip("nom du fichier sans le .json")]
    public List<string> SortFileNameList;

    private int OutSizeInTile = 0;
    private int InSizeInTile = 0;

    public List<Sort> SortList;
    private float LastSizeUpdate = 0;
	// Use this for initialization
	void Start () {

        this.InitTileMap();

        SortList = Sort.GetSortListFromFileList(SortFileNameList);

        float PlayerSize = GameObject.Find("MainCharacter").GetComponent<SpriteRenderer>().sprite.bounds.extents.y * 2; // extends return half size
        float TileSize = tile.sprite.bounds.extents.y * 2;

        OutSizeInTile = (int)Mathf.Floor(mapOutMultiplier * PlayerSize / TileSize / 2); // on divise par 2, on va utilisé -/+ au lieu de 0/+
        InSizeInTile = (int)Mathf.Floor(mapInMultiplier * PlayerSize / TileSize / 2); // on divise par 2, on va utilisé -/+ au lieu de 0/+

        this.OuterBoxFill(new Vector2Int(InSizeInTile, InSizeInTile), new Vector2Int(OutSizeInTile, OutSizeInTile));    // create danger zone
    }
	
	// Update is called once per frame
	void Update () {
		if( (Time.time - LastSizeUpdate) >= SizeUpdatePeriod)
        {
            UpdateDangerZone(SizeReduction);
            LastSizeUpdate = Time.time;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach(Sort s in SortList)
        {
            foreach(Effect e in s.effectList)
            {
                collision.GetComponent<BasicCharacter>().AddEffect(e);
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (Sort s in SortList)
        {
            foreach (Effect e in s.effectList)
            {
                collision.GetComponent<BasicCharacter>().RemoveEffect(e);
            }
        }
    }

    void UpdateDangerZone(int reduction)
    {
        int NewInSize = InSizeInTile - SizeReduction;
        this.OuterBoxFill(new Vector2Int(NewInSize, NewInSize), new Vector2Int(InSizeInTile, InSizeInTile));
        InSizeInTile = NewInSize;
    }
}
