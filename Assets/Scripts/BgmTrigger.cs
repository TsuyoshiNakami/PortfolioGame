using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public enum BGMEnum
{
    None,
    Field,
    Lake,
    Mountain,
    MountainBoss,
    MyHouse
}

public enum BgmTriggerMessageType
{
    Enter,
    Exit
}

public class BgmTriggerMessage
{
    public BGMTransitionData bgmTransitionData { get; private set; }
    public BgmTriggerMessageType type { get; private set; }

    public BgmTriggerMessage(BGMTransitionData bgmData, BgmTriggerMessageType type)
    {
        this.bgmTransitionData = bgmData;
        this.type = type;
    }
}


public class BgmTrigger : MonoBehaviour
{
    bool isPlayerIn;
    [SerializeField] BGMEnum bgm;
    BGMTransitionData bgmTransitionData;
    

    void Start()
    {
        bgmTransitionData = BGMTransitionDataList.Entity.GetDataByEnum(bgm);
    }

    Subject<BgmTriggerMessage> playerEnterSubject = new Subject<BgmTriggerMessage>();
    public IObservable<BgmTriggerMessage> OnPlayerEnterSubject
    {
        get
        {
            return playerEnterSubject;
        }
    }


    Subject<BgmTriggerMessage> playerExitSubject = new Subject<BgmTriggerMessage>();
    public IObservable<BgmTriggerMessage> OnPlayerExitSubject
    {
        get
        {
            return playerExitSubject;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Enter" + bgm);

            MessageBroker.Default.Publish(new BgmTriggerMessage(bgmTransitionData, BgmTriggerMessageType.Enter));
            playerEnterSubject.OnNext(new BgmTriggerMessage(bgmTransitionData, BgmTriggerMessageType.Enter));
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Exit" + bgm);
            playerEnterSubject.OnNext(new BgmTriggerMessage(bgmTransitionData, BgmTriggerMessageType.Exit));
            MessageBroker.Default.Publish(new BgmTriggerMessage(bgmTransitionData, BgmTriggerMessageType.Exit));
            isPlayerIn = false;
        }
    }
}
