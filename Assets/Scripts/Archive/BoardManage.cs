using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManage : MonoBehaviour
{
    public int columns = 8;              //Number of columns in our game board.
    public int rows = 8;                 //Number of rows in our game board.
    public GameObject[] floorTiles;      //Array of floor prefabs.
    public GameObject[] outerWallTiles;  //Array of outer tile prefabs.


    /*
     *  PRIVATE FUNCTION
     */
    private void BoardSetup()
    {
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate;

                //Check if we current position is at board edge
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                else
                    toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x * 0.1595f, y * 0.1595f, 0f), Quaternion.identity) as GameObject;

                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent(this.transform);
            }
        }
    }



    /*
     *  UNITY 3D FUNCTION
     */
    void Start()
    {
        BoardSetup();
    }

    void Update()
    {

    }


}
