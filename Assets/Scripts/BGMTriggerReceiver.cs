using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BGMTriggerReceiver : MonoBehaviour
{


    [SerializeField] AudioSource[] bgmAudioSouraces;
    int currentAudioSourceNum;
    private const float FADE_OUT_RES = 100f;
    private const float FADE_OUT_RATE = 10f;
    private bool isfading;

    [SerializeField] List<BGMEnum> currentTriggers = new List<BGMEnum>();
    [SerializeField] BGMEnum currentBGM;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        MessageBroker.Default.Receive<BgmTriggerMessage>().Subscribe(msg =>
        {

            switch (msg.type)
            {
                case BgmTriggerMessageType.Enter:
                    if (!currentTriggers.Contains(msg.bgmEnum))
                    {
                        currentTriggers.Add(msg.bgmEnum);
                    }
                    break;
                case BgmTriggerMessageType.Exit:
                    if (currentTriggers.Contains(msg.bgmEnum))
                    {
                        currentTriggers.Remove(msg.bgmEnum);
                    }
                    break;
                default:
                    break;
            }

            int priorityMax = -1;
            BGMEnum tmpBGM = BGMEnum.None;
            foreach (BGMEnum bgmEnum in currentTriggers)
            {
                BGMTransitionData data = BGMTransitionDataList.Entity.GetDataByEnum(bgmEnum);
                if (data == null)
                {
                    continue;
                }
                if (data.priority > priorityMax)
                {
                    priorityMax = data.priority;
                    tmpBGM = data.bgmEnum;
                }
            }

            if (tmpBGM != BGMEnum.None)
            {
                if (tmpBGM != currentBGM)
                {
                    ChangeBGM(BGMTransitionDataList.Entity.GetDataByEnum(tmpBGM).audioClip);
                }
            }
            else
            {
                StopAllBGM();
            }
            currentBGM = tmpBGM;
        });
    }

    void StopAllBGM()
    {
        foreach (AudioSource a in bgmAudioSouraces)
        {
            FadeOutRx(a, 1);
        }
    }

    void ChangeBGM(AudioClip clip)
    {
        int num = currentAudioSourceNum;
        FadeOutRx(bgmAudioSouraces[num], 1);
        currentAudioSourceNum = 1 - currentAudioSourceNum;
        num = currentAudioSourceNum;
        bgmAudioSouraces[num].clip = clip;
        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
       {
           bgmAudioSouraces[num].volume = 1;
           bgmAudioSouraces[num].Play();
           //FadeInRx(bgmAudioSouraces[num], 0, 0);
       });
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
