using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class SoundPlayableBehavior : PlayableBehaviour
{

    bool hasBGMStarted;
    ulong currentFrame = 0;
    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        BGMManager.Instance.StopAllBGM(); 
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {

    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        Debug.Log(currentFrame);
        if(currentFrame == 0)
        {
            currentFrame++;// info.frameId;
        } else
        {
            currentFrame++;
        }
        if(hasBGMStarted)
        {
            return;
        }

        if (currentFrame >= 250)
        {
            hasBGMStarted = true;
            BGMTransitionData newBGM = BGMTransitionDataList.Entity.GetDataByName("MountainBoss");
            newBGM.attributes.Add(BGMAttribute.NotChangeByTrigger);
            BGMManager.Instance.ChangeBGM(newBGM);
        }
    }
}
