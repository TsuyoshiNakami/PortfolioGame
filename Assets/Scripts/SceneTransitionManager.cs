using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;

public class SceneTransitionManager
{
    static SceneTransitionManager sceneTransitionmanager;
    public static SceneTransitionManager Instance
    {
        get
        {
            if (sceneTransitionmanager == null)
            {
                sceneTransitionmanager = new SceneTransitionManager();
            }
            return sceneTransitionmanager;
        }
    }

    static SceneChangeData sceneChangeData;

    public static void Initialize()
    {
        if (sceneTransitionmanager == null)
        {
            sceneTransitionmanager = new SceneTransitionManager();
        }
        MessageBroker.Default.Receive<SceneChangeData>().Subscribe(data =>
        {
            ChangeScene(data);
        });
    }


    static void ChangeScene(SceneChangeData data)
    {
        sceneChangeData = data;
        SceneManager.LoadScene(data.sceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.IsNullOrEmpty(sceneChangeData.playerStartObjectName)) {
            GameObject.Find("Player").transform.position = GameObject.Find(sceneChangeData.playerStartObjectName).transform.position;
        }
    }
}
