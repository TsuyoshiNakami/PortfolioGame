using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BGMManager : MonoBehaviour
{
    [SerializeField] AudioSource[] bgmAudioSources;
    AudioSource seamlessBgmAudioSource;

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
        if (currentBGMData == null)
        {
            return false;
        }

        return currentBGMData.attributes.Contains(attribute);
    }

    // Start is called before the first frame update
    void Start()
    {
        seamlessBgmAudioSource = gameObject.AddComponent<AudioSource>();
        seamlessBgmAudioSource.playOnAwake = false;
        seamlessBgmAudioSource.loop = true;

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

        // シームレス遷移ありBGMならば
        if (data.seamlessBGMs.Count > 0)
        {
            ProcessSeamlessBGM(data);
            return;
        }

        int num = currentAudioSourceNum;

        if (data.seamlessBGMs.Count == 0)
        {
            FadeOutRx(seamlessBgmAudioSource, 1);
        }

        AudioClip clip = data.audioClip;

        int fadeTime = 2;

        FadeOutRx(bgmAudioSources[num], 2);
        currentAudioSourceNum = 1 - currentAudioSourceNum;
        num = currentAudioSourceNum;
        bgmAudioSources[num].clip = clip;
        Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
       {
           bgmAudioSources[num].volume = data.volume;
           bgmAudioSources[num].Play();
           if (seamlessBgmAudioSource.clip != null)
           {
               seamlessBgmAudioSource.Play();
           }

           //FadeInRx(bgmAudioSouraces[num], 0, 0);
       });
        currentBGMData = data;
    }

    void ProcessSeamlessBGM(BGMTransitionData data)
    {
        int num = currentAudioSourceNum;

        Debug.Log("currentBGM : " + currentBGMData.bgmName);
        foreach (string seamless in currentBGMData.seamlessBGMs)
        {
            Debug.Log("seamlessBGM : " + seamless);
        }

        // シームレス遷移する場合
        if (currentBGMData.seamlessBGMs.Contains(data.bgmName))
        {


            if (seamlessBgmAudioSource.clip != data.audioClip)
            {
   FadeInSeamless(bgmAudioSources[num], 2, data.volume);

                FadeOutSeamless(seamlessBgmAudioSource, 2);
            }
            else
            {
                FadeOutSeamless(bgmAudioSources[num], 2);

                FadeInSeamless(seamlessBgmAudioSource, 2, data.volume);
            }
            Debug.Log("Seamless Start");


            currentBGMData = data;
            return;
        }

        Debug.Log("Seamless BGM Set");
        // 同時再生を始める

        foreach (string seamlessBgmName in data.seamlessBGMs)
        {
            seamlessBgmAudioSource.clip = BGMTransitionDataList.Entity.GetDataByName(seamlessBgmName).audioClip;
            seamlessBgmAudioSource.volume = 0;
        }
        FadeOutRx(bgmAudioSources[num], 1);
        currentAudioSourceNum = 1 - currentAudioSourceNum;
        num = currentAudioSourceNum;
        bgmAudioSources[num].clip = data.audioClip;
        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            bgmAudioSources[num].volume = data.volume;
            bgmAudioSources[num].Play();

            seamlessBgmAudioSource.Play();

            //FadeInRx(bgmAudioSouraces[num], 0, 0);
        });
        currentBGMData = data;
    }
    private void FadeInRx(AudioSource source, float fadeTime)
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
                //Source.clip.UnloadAudioData();
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
    private void FadeInSeamless(AudioSource source, float fadeTime, float targetVolume = 1)
    {

        Observable.Interval(TimeSpan.FromMilliseconds(FADE_OUT_RES))
            .Select<long, float>(_ =>
            {
                source.volume += Mathf.Lerp(0f, targetVolume, 1 / (FADE_OUT_RATE * fadeTime));
                return source.volume;
            })
            .First(volume => volume >= targetVolume).Subscribe(_ =>
            {
                source.volume = targetVolume;
                isfading = false;
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
    private void FadeOutSeamless(AudioSource Source, float FadeTime)
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
                Source.volume = 0;
                //Source.clip.UnloadAudioData();
                isfading = false;
                //Debug.Log("フェードフラグ：False");
            })
            .AddTo(this);

    }
}
