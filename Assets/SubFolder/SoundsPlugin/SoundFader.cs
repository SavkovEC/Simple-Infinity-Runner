using UnityEngine;
using System.Collections;

public class SoundFader
{
    float fadeDuration;
    float initialValue = 1f;
    float targetValue = 1f;


    public float Elapsed
    {
        get;
        set;
    }


    public bool IsFading
    {
        get
        {
            return Elapsed < fadeDuration;
        }
    }


    public float Fade
    {
        get
        {
            float fade = (fadeDuration <= 0) ? targetValue : Mathf.Lerp(initialValue, targetValue, Elapsed/fadeDuration );
            return Mathf.Clamp01(fade);
        }
    }


    public void FadeIn(float time)
    {
		initialValue = IsFading ? Fade : 0f;
        Elapsed = 0;
        fadeDuration = time;
        targetValue = 1f;
    }


    public void FadeOut(float time)
    {
		initialValue = IsFading ? Fade : 1f;
        Elapsed = 0;
        fadeDuration = time;
        targetValue = 0f;
    }


    public void Reset()
    {
        Elapsed = 0;
        fadeDuration = 0;
        initialValue = 1f;
        targetValue = 1f;
    }
}
