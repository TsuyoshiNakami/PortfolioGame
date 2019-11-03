using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvent : MonoBehaviour
{
    [SerializeField] List<string> footstepSoundNames;
    public void OnFootstep()
    {
        int groundNum = CheckTerrainTexture.Instance.GetMainGroundNum();
        if(groundNum < 0)
        {
            return;
        }
        SEManager.Instance.PlayOneShot(footstepSoundNames[groundNum]);
    }
}
