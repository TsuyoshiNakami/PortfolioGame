using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SEData
{
    public string seName;
    public AudioClip audioClip;
    public int priority;
    [Range(0, 1)] public float volume;
}
