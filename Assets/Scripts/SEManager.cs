using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SEManager : MonoBehaviour
{
    [SerializeField] AudioSource[] audioSources;
    private const float FADE_OUT_RES = 100f;
    private const float FADE_OUT_RATE = 10f;
    private bool isfading;

    [SerializeField] public BGMTransitionData currentBGMData;

    int currentAudioSourceNum;
    static SEManager soundManager;

    public static SEManager Instance
    {
        get
        {

            if (soundManager == null)
            {
                soundManager = GameObject.Find("SoundManagers").GetComponentInChildren<SEManager>();
            }
            return soundManager;
        }
    }


    public void PlayOneShot(string name)
    {
        AudioClip clip = SEDataList.Entity.GetDataByName(name).audioClip;
        audioSources[0].PlayOneShot(clip);
    }
   
}
