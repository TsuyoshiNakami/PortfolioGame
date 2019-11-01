using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BGMTransitionData
{
    public BGMEnum bgmEnum;
    public AudioClip audioClip;
    public int priority;
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

    public BGMTransitionData GetDataByEnum(BGMEnum pEnum)
    {
        return list.Find(data => data.bgmEnum == pEnum);
    }

}
