﻿using System;
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
            Sort newSort = Sort.GetSortFromFile(sortName);
            if (newSort != null)
                SortList.Add(newSort);
        }

        return SortList;
    }

    public static Sort GetSortFromFile(string FileName)
    {
        Sort s = null;

        StreamReader file = new StreamReader("Assets/Data/Sorts/" + FileName + ".json", false);

        if (file == null || file.ToString() == "")
        {
            Debug.Log("can't get sort '" + FileName + "' can't open file (" + "Assets/Data/Sorts/" + FileName + ")");
        }
        else s = JsonUtility.FromJson<Sort>(file.ReadToEnd());

        return s;
    }

    public void LoadSounds()
    {
        throwSound = Resources.Load(throwSoundFile) as AudioClip;
        travelSound = Resources.Load(travelSoundFile) as AudioClip;
        impactSound = Resources.Load(impactSoundFile) as AudioClip;
    }
}
