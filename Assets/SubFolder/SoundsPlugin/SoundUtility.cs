using UnityEngine;
using System.Collections;

public static class SoundUtility
{
    static public float TransformPitch( float pitchSemiTones )
    {
        return Mathf.Pow( 2, pitchSemiTones / 12.0f );
    }
}
