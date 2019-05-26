using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sort {

    public string name;

    public float coolDown;
    public float lastUsed;

    public string throwSoundFile;
    public AudioClip throwSound;
    public string travelSoundFile;
    public AudioClip travelSound;
    public string impactSoundFile;
    public AudioClip impactSound;

    public List<Effect> effectList = new List<Effect> ();
    public List<Projectile> projectileList = new List<Projectile> ();

    public Sort()
    {

    }

    public static List<Sort> GetSortListFromFileList(List<string> FileList)
    {
        List<Sort> SortList = new List<Sort>();

        foreach (string sortName in FileList)
        {
            StreamReader file = new StreamReader("Assets/Data/Sorts/" + sortName + ".json", false);

            if (file == null || file.ToString() == "")
            {
                Debug.Log("can't get sort '" + sortName + "' can't open file (" + "Assets/Data/Sorts/" + sortName + ")");
            }
            else SortList.Add(JsonUtility.FromJson<Sort>(file.ReadToEnd()));
        }

        return SortList;
    }

    public void LoadSounds()
    {
        throwSound = Resources.Load(throwSoundFile) as AudioClip;
        travelSound = Resources.Load(travelSoundFile) as AudioClip;
        impactSound = Resources.Load(impactSoundFile) as AudioClip;
    }
}
