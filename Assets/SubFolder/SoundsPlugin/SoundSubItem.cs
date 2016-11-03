using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundSubItem
{
//    public SoundClipType subItemType = SoundClipType.Clip;
    public UnityEngine.AudioClip clip;
    public AssetLink clipref;

    public float Probability = 1.0f;
    public float Volume = 1.0f;
    public float PitchShift = 0f;
    public float Pan2D = 0;
    public float Delay = 0;
    public float RandomPitch = 0;
    public float RandomVolume = 0;
    public float RandomDelay = 0;
	public float FadeIn = 0;
	public float FadeOut = 0;
    public bool RandomStartPosition = false;

    [System.NonSerialized] public SoundItem Item;
    private float _summedProbability = -1.0f;


    public string Name
    {
        get
        {
            return clipref.name;
        }
    }


    public SoundClipType SubItemType
    {
        get
        {
            if (clip != null)
            {
                return SoundClipType.Clip;
            }

            if (clipref != null && clipref.StreamingAssetPath.Length > 0)
            {
                if (clipref.StreamingAssetPath.Contains(".wav"))
                {
                    return SoundClipType.StreamingClip;
                }
                else
                {
                    return SoundClipType.Native;
                }
            }

            return SoundClipType.Unknown;
        }
        set
        {
//            subItemType = value;
        }
    }


    public UnityEngine.Object Clip
    {
        get
        {
            if (SubItemType == SoundClipType.Clip)
            {
                return clip;
            }
            else if(clipref != null)
            {
                return clipref.asset;
            }

            return null;
        }
        set
        {
            clip = value as AudioClip;
            clipref = new AssetLink(value);
        }
    }


    internal float SummedProbability
    {
        get { return _summedProbability; }
        set { _summedProbability = value; }
    }


    internal float GetPitch()
    {
        float pitch = SoundUtility.TransformPitch( PitchShift );
        if ( RandomPitch != 0 )
        {
            pitch *= SoundUtility.TransformPitch( UnityEngine.Random.Range( -RandomPitch, RandomPitch ) );
        }

        return pitch;
    }


    internal void Initialize( SoundItem item )
    {
        Item = item;
    }


    public override string ToString()
    {
        return base.ToString();
    }
}
