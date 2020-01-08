using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotDangerZone : MapSizeGenerator
{
    [Tooltip("Taille Initial externe de la zone en unitée du Player")]
    public int mapMultiplier = 0;
    [Tooltip("nom du fichier sans le .json")]
    public List<string> SortFileNameList;

    public List<Sort> SortList;

    public int SizeInTile = 0;

    // Use this for initialization
    void Start () {

        this.InitTileMap();

        SortList = Sort.GetSortListFromFileList(SortFileNameList);

        float PlayerSize = GameObject.Find("MainCharacter").GetComponent<SpriteRenderer>().sprite.bounds.extents.y * 2; // extends return half size
        float TileSize = tile.sprite.bounds.extents.y * 2;

        SizeInTile = (int)Mathf.Floor(mapMultiplier * PlayerSize / TileSize / 2); // on divise par 2, on va utilisé -/+ au lieu de 0/+

        this.BoxFill(new Vector3Int(-SizeInTile, -SizeInTile, 0), new Vector3Int(SizeInTile, SizeInTile, 0));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (Sort s in SortList)
        {
            foreach (Effect e in s.effectList)
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
}
