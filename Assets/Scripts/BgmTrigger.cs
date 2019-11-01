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
    public BGMEnum bgmEnum { get; private set; }
    public BgmTriggerMessageType type { get; private set; }

    public BgmTriggerMessage(BGMEnum bgmEnum, BgmTriggerMessageType type)
    {
        this.bgmEnum = bgmEnum;
        this.type = type;
    }
}


public class BgmTrigger : MonoBehaviour
{
    bool isPlayerIn;
    [SerializeField] BGMEnum bgm;


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

            MessageBroker.Default.Publish(new BgmTriggerMessage(bgm, BgmTriggerMessageType.Enter));
            playerEnterSubject.OnNext(new BgmTriggerMessage(bgm, BgmTriggerMessageType.Enter));
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Exit" + bgm);
            playerEnterSubject.OnNext(new BgmTriggerMessage(bgm, BgmTriggerMessageType.Exit));
            MessageBroker.Default.Publish(new BgmTriggerMessage(bgm, BgmTriggerMessageType.Exit));
            isPlayerIn = false;
        }
    }
}
