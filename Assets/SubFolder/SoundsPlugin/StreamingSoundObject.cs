using UnityEngine;
using System.Collections;

public class StreamingSoundObject : SingleSoundObject 
{
    protected bool waitForLoading;


    public override void Play(float delay, float fadeIn = 0)
    {
        waitForLoading = true;
        SoundsManager.Instance.StreamingAudioBuffer.LoadClip(SubItem.Name, SubItem.clipref.FullPath, delegate (AudioClip loadedAudioClip) {
            if (loadedAudioClip != null)
            {
                waitForLoading = false;
                source.clip = loadedAudioClip;
                base.Play(delay, fadeIn);
            }
            else
            {
                Debug.LogWarning("Loaded audio clip is null. Name : " + SubItem.clipref.name);
            }
        });     
    }


    internal override bool IsFinish()
    {
        return !waitForLoading && base.IsFinish();
    }
}
