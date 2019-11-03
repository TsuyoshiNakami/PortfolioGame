using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SEDataList : ScriptableObject
{
    public const string PATH = "SEList";

    //MyScriptableObjectの実体
    private static SEDataList _entity;
    public static SEDataList Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<SEDataList>(PATH);

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }

            return _entity;
        }
    }
    [SerializeField] List<SEData> list;

    public SEData GetDataByName(string name)
    {
        return list.Find(data => data.seName == name);
    }

}
