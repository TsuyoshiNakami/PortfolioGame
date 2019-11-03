using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class TimelineTrigger : MonoBehaviour
{
    [SerializeField] PlayableDirector playableDirector;
    bool hasPlayed;

    private void OnTriggerEnter(Collider other)
    {
        if(hasPlayed)
        {
            return;
        }
        if(other.CompareTag("Player"))
        {
            hasPlayed = true;
            playableDirector.Play();
        }
    }
}
