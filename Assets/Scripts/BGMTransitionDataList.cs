using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BGMTransitionData
{
    public string bgmName;
    public List<BGMAttribute> attributes;
    public List<string> seamlessBGMs;
    public AudioClip audioClip;
    public int priority;
   [Range(0, 1)] public float volume;
}

public enum BGMAttribute
{
    NotChangeByTrigger
}

[CreateAssetMenu]
public class BGMTransitionDataList : ScriptableObject
{
    public const string PATH = "BGMList";

    //MyScriptableObjectの実体
    private static BGMTransitionDataList _entity;
    public static BGMTransitionDataList Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<BGMTransitionDataList>(PATH);

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }

            return _entity;
        }
    }
    [SerializeField]  List<BGMTransitionData> list;

    public BGMTransitionData GetDataByName(string name)
    {
        return list.Find(data => data.bgmName == name);
    }

}
