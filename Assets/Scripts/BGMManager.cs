using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BGMManager : MonoBehaviour
{
    [SerializeField] AudioSource[] bgmAudioSources;
    private const float FADE_OUT_RES = 100f;
    private const float FADE_OUT_RATE = 10f;
    private bool isfading;

    [SerializeField] public BGMTransitionData currentBGMData;

    int currentAudioSourceNum;
    static BGMManager soundManager;

    public static BGMManager Instance
    {
        get
        {

            if (soundManager == null)
            {
                soundManager = GameObject.Find("SoundManagers").GetComponentInChildren<BGMManager>();
            }
            return soundManager;
        }
    }


    public bool CurrentBGMAttributesContains(BGMAttribute attribute)
    {
        if(currentBGMData == null)
        {
            return false;
        }

        return currentBGMData.attributes.Contains(attribute);
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    public void StopAllBGM()
    {
        foreach (AudioSource a in bgmAudioSources)
        {
            FadeOutRx(a, 1);
        }
        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            currentBGMData = null;
        });
    }

    public void ChangeBGM(BGMTransitionData data)
    {
        AudioClip clip = data.audioClip;

        int num = currentAudioSourceNum;
        FadeOutRx(bgmAudioSources[num], 1);
        currentAudioSourceNum = 1 - currentAudioSourceNum;
        num = currentAudioSourceNum;
        bgmAudioSources[num].clip = clip;
        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
       {
           bgmAudioSources[num].volume = data.volume;
           bgmAudioSources[num].Play();
           //FadeInRx(bgmAudioSouraces[num], 0, 0);
       });
        currentBGMData = data;
    }
    private void FadeInRx(AudioSource source, float fadeTime, float delay)
    {

        float VolumeTmp = 1;
        source.volume = 0;
        source.Play();
        Observable.Interval(TimeSpan.FromMilliseconds(FADE_OUT_RES))
            //徐々に音を小さくする
            .Select<long, float>(_ =>
            {
                source.volume += Mathf.Lerp(0f, VolumeTmp, 1 / (FADE_OUT_RATE * fadeTime));
                return source.volume;
            })
            .First(volume => volume >= 1).Subscribe(_ =>
            {
                source.volume = VolumeTmp;
                //Source.clip.UnloadAudioData();
                isfading = false;
                //Debug.Log("フェードフラグ：False");
            })
            .AddTo(this);

    }

    private void FadeOutRx(AudioSource Source, float FadeTime)
    {

        float VolumeTmp = Source.volume;
        Observable.Interval(TimeSpan.FromMilliseconds(FADE_OUT_RES))
            //徐々に音を小さくする
            .Select<long, float>(_ =>
            {
                Source.volume -= Mathf.Lerp(0f, VolumeTmp, 1 / (FADE_OUT_RATE * FadeTime));

                return Source.volume;
            })
            .First(volume => volume <= 0).Subscribe(_ =>
            {
                Source.Stop();
                Source.volume = VolumeTmp;
                //Source.clip.UnloadAudioData();
                isfading = false;
                //Debug.Log("フェードフラグ：False");
            })
            .AddTo(this);

    }

}
