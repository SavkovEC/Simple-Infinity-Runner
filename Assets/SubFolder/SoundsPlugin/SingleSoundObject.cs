using UnityEngine;
using System.Collections;


public class SingleSoundObject : SoundObject
{
    #region Variables
    protected AudioSource source;

    protected AudioSource Source
    {
        get
        {
            return gameObject.GetRequiredComponent<AudioSource>(ref source);
        }
    }

    #endregion



    #region Protected

    protected override void ApplyMuteChange()
    {
        Source.mute = Mute;
    }

    protected override void ApplyLoopChange()
    {
        Source.loop = Loop;
    }

    protected override void ApplyPriorityChange()
    {
        Source.priority = Priority;
    }

    protected override void ApplyVolumeChange()
    {
        Source.volume = TotalVolume;
    }

    protected override void ApplyPitchChange()
    {
        Source.pitch = Pitch;
    }

    protected override void ApplyPanChange()
    {
        Source.panStereo = Pan;
    }

    protected override void ApplySpatialBlendChange()
    {
        Source.spatialBlend = SpatialBlend;
    }

    protected override void ApplySpreadChange()
    {
        Source.spread = Spread;
    }

    protected override void ApplyMinDistanceChange()
    {
        Source.minDistance = MinDistance;
    }

    protected override void ApplyMaxDistanceChange()
    {
        Source.maxDistance = MaxDistance;
    }

    protected override void ApplySubItem()
    {
        Source.clip = SubItem.clip;
    }

    #endregion

    protected override void Update()
    {
        if (Source != null)
        {
            isPlaying = Source.isPlaying;
        }

        base.Update();
    }

    public override void OnPlay()
    {
        float delay = 0;

        if(delay > 0)
            Source.PlayDelayed(delay);
        else
            Source.Play();
    }


	protected override void ApplyPause()
    {
		if (IsPaused)
        {
            Source.Pause();
        }
        else
        {
            Source.UnPause();
        }
    }


    internal override void StopSound()
    {
        source.clip = null;
        Source.Stop();
        base.StopSound();
    }

}
