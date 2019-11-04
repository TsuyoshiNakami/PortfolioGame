using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BGMTriggerReceiver : MonoBehaviour
{



    [SerializeField] List<BGMTransitionData> currentTriggers = new List<BGMTransitionData>();
    [SerializeField] string currentBGM;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        MessageBroker.Default.Receive<BgmTriggerMessage>().Subscribe(msg =>
        {
            OnReceiveBgmTriggerMessage(msg);
        });
    }

    void OnReceiveBgmTriggerMessage(BgmTriggerMessage msg)
    {
        AddOrRemoveTriggers(msg);

        if (BGMManager.Instance.CurrentBGMAttributesContains(BGMAttribute.NotChangeByTrigger))
        {
            return;
        }

        BGMTransitionData tmpData = GetHighestPriorityData();
        SetBgmByTransitionData(tmpData);

    }

    void SetBgmByTransitionData(BGMTransitionData tmpData)
    {
        if (tmpData == null)
        {

            BGMManager.Instance.StopAllBGM();
            currentBGM = "";
            return;
        }

        if (tmpData.bgmName != currentBGM)
        {
            BGMManager.Instance.ChangeBGM(tmpData);
            currentBGM = tmpData.bgmName;
        }


    }


    BGMTransitionData GetHighestPriorityData()
    {
        int priorityMax = -1;
        BGMTransitionData tmpBGM = null;
        foreach (BGMTransitionData bgmEnum in currentTriggers)
        {
            BGMTransitionData data = bgmEnum;
            if (data == null)
            {
                continue;
            }
            if (data.priority > priorityMax)
            {
                priorityMax = data.priority;
                tmpBGM = data;
            }
        }
        return tmpBGM;

    }

    void AddOrRemoveTriggers(BgmTriggerMessage msg)
    {
        switch (msg.type)
        {
            case BgmTriggerMessageType.Enter:
                if (!currentTriggers.Contains(msg.bgmTransitionData))
                {
                    currentTriggers.Add(msg.bgmTransitionData);
                }
                break;
            case BgmTriggerMessageType.Exit:
                if (currentTriggers.Contains(msg.bgmTransitionData))
                {
                    currentTriggers.Remove(msg.bgmTransitionData);
                }
                break;
            default:
                break;
        }

    }
}
