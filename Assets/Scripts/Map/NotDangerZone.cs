using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotDangerZone : MapSizeGenerator
{
    [Tooltip("nom du fichier sans le .json")]
    public List<string> SortFileNameList;

    public List<Sort> SortList;

    // Use this for initialization
    void Start () {

        this.Init();

        SortList = Sort.GetSortListFromFileList(SortFileNameList);

        this.BoxFill(new Vector3Int(-initialTileSize, -initialTileSize, 0), new Vector3Int(initialTileSize, initialTileSize, 0));
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
