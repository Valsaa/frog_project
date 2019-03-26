using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SortTest : MonoBehaviour {

    public List<string> listeDesSorts;
    public List<Sort> sortList = new List<Sort> ();
    private const string sortPath = "Assets/Data/Sorts/";
    private StreamReader file;

	// Use this for initialization
	void Start () {
        sortList = new List<Sort>();
        foreach (string filename in listeDesSorts)
        {
            file = new StreamReader(sortPath + filename + ".json", false);
            if(file.ToString() == "")
            {
                Debug.Log("can't get sort '" + filename + "' can't open file (" + sortPath + filename + ")");
                return;
            }
            sortList.Add(JsonUtility.FromJson<Sort>(file.ReadToEnd()));
            Debug.Log("added sort : " + filename);

        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
