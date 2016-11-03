using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NativeSoundObject : SoundObject
{
    static int globalCounter;

    protected string uniqueID = string.Empty;
    protected Vector3 lastPosition;


    public override void OnPlay()
    {
        uniqueID = SubItem.clipref.name + "__" + (++globalCounter).ToString();
        string path = SubItem.clipref.StreamingAssetPath;
        #if !UNITY_EDITOR && UNITY_ANDROID
        path = SubItem.clipref.StreamingAssetPathRelative.Replace(".caf", ".ogg");
        path = path.Replace("iOS", "Android");//temp
        #endif
        LLSoundManager.PlaySound(uniqueID, path, TotalVolume, Pitch, Loop, transform.position, MinDistance, MaxDistance, SpatialBlend);
        Register(this, uniqueID);
    }


	protected override void ApplyPause()
    {
		LLSoundManager.PauseSound(uniqueID, IsPaused);
    }


    internal override void StopSound()
	{
        base.StopSound();
		LLSoundManager.StopSound(uniqueID);
		Unregister(this, uniqueID);
	}


    protected override void LateUpdate()
    {
        Vector3 pos = transform.position;
        if (lastPosition != pos)
        {
            IsDirty = true;
            lastPosition = pos;
        }

        if (IsDirty)
        {
            LLSoundManager.UpdateSound(uniqueID, TotalVolume, Pitch, Loop, transform.position, MinDistance, MaxDistance, SpatialBlend);
        }

        base.LateUpdate();
    }


    #region Static handler
    static Dictionary<string, NativeSoundObject> playingSounds = new Dictionary<string, NativeSoundObject>();


    static NativeSoundObject()
    {
        LLSoundManager.OnStoppedSound -= LLCallback;
        LLSoundManager.OnStoppedSound += LLCallback;
    }


    static void LLCallback(string uid)
    {
        NativeSoundObject so;
        if (playingSounds.TryGetValue(uid, out so))
        {
            so.isPlaying = false;
        }
    }


    static void Register(NativeSoundObject so, string uniqueID)
    {
        if (!playingSounds.ContainsKey(uniqueID))
        {
            playingSounds.Add(uniqueID, so);
        }
        else
        {
            Debug.LogError("register Native Sound Object twice : " + so, so ) ;
        }
    }


    static void Unregister(NativeSoundObject so, string uniqueID)
    {
        if (playingSounds.ContainsKey(uniqueID))
        {
            playingSounds.Remove(uniqueID);
        }
    }

    #endregion
   
}
