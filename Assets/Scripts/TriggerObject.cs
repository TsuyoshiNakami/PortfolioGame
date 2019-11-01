using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx; 


[System.Serializable]
public class SceneChangeData
{
    [SerializeField] public string sceneName;
    [SerializeField] public string playerStartObjectName;
}

public class TriggerObject : MonoBehaviour
{

    [SerializeField] SceneChangeData sceneChangeData;

    private void OnTriggerEnter(Collider other)
    {
        MessageBroker.Default.Publish(sceneChangeData);
    }
}
