using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Inventain/Tween/Transform")]
public class TweenTransform : Tweener 
{	
	public Transform beginTransform;
    public Transform endTransform;		
	
	override protected void TweenUpdateRuntime(float factor, bool isFinished) 
    {
        CachedTransform.position = beginTransform.position * (1f - factor) + endTransform.position * factor;

        CachedTransform.rotation = Quaternion.Euler(beginTransform.rotation.eulerAngles * (1f - factor) + endTransform.rotation.eulerAngles * factor);
	}		

    static public TweenTransform SetTransform(GameObject go, Transform transform, float duration = 1f) 
    {
        TweenTransform twt = Tweener.InitGO<TweenTransform>(go, duration);
        twt.beginTransform = twt.CachedTransform;
        twt.endTransform = transform;
        twt.Play(true);
		return twt;
	}
}