using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIControl : MonoBehaviour 
{
    [System.Serializable]
    public abstract class ShowableObject
    {
        public float showDelay;
        public float showDelayVariance;
        public float hideDelay;
        public float hideDelayVariance;
    }

    [System.Serializable]
    public class TweenData : ShowableObject
    {
        public Tweener tween;
        public float showDurationScale = 1;
        public float hideDurationScale = 1;
    }

    [System.Serializable]
    public class ControlData : ShowableObject
    {
        public GUIControl control;
    }

	public struct ActionWithInvokes
	{
		public System.Action<GUIControl> onFinished;
		public bool callInvokes;

		public ActionWithInvokes(System.Action<GUIControl> action, bool call)
		{
			onFinished = action;
			callInvokes = call;
		}
	}

	#region Variables

    [HideInInspector][SerializeField] List<TweenData> tweensData = new List<TweenData>();
    [HideInInspector][SerializeField] List<ControlData> controlsData = new List<ControlData>();
	[SerializeField] bool notUsedForScreenAnimatingCondition;
    [SerializeField] GameObject[] deactivateOnHideObjects;

    private Transform cachedTransform1;

    private Transform CachedTransform1
    {
        get 
        {
            if (cachedTransform1 == null)
            {
                cachedTransform1 = transform;
            }

            return cachedTransform1;
        }
    }


	public bool NotUsedForScreenAnimatingCondition 
	{ 
		get 
		{ 
			return notUsedForScreenAnimatingCondition; 
		}
		set
		{
			notUsedForScreenAnimatingCondition = value;
		}
	}


    public List<TweenData> TweensData
    {
        get
        {
            return tweensData;
        }
    }

    public List<ControlData> ControlsData
    {
        get
        {
            return controlsData;
        }
    }

    public bool IsShown
    {
        get 
        {
            for (int i = 0; i < tweensData.Count; i++)
            {
                TweenData t = tweensData[i];
                if (t.tween == null || !t.tween.NotUsedForScreenAnimatingCondition && !t.tween.IsEndStateSet || t.tween.enabled)
                {
                    return false;
                }
            }

            for (int i = 0; i < controlsData.Count; i++)
            {
                ControlData c = controlsData[i];
                if (c.control == null && !c.control.NotUsedForScreenAnimatingCondition && !c.control.IsShown)
                {
                    return false;
                }
            }                
           
            return true; 
        }
    }

    public bool IsHidden
    {
        get 
        {
            for (int i = 0; i < tweensData.Count; i++)
            {
                TweenData t = tweensData[i];
                if (t.tween == null || !t.tween.NotUsedForScreenAnimatingCondition && !t.tween.IsBeginStateSet || t.tween.enabled)
                {
                    return false;
                }
            }

            for (int i = 0; i < controlsData.Count; i++)
            {
                ControlData c = controlsData[i];
                if (c.control == null || !c.control.NotUsedForScreenAnimatingCondition && !c.control.IsHidden)
                {
                    return false;
                }
            }                

            return true; 
        }
    }

    public bool IsAnimating
    {
        get 
        {
            return !IsHidden && !IsShown; 
        }
    }

    public float ShowDuration
    {
        get
        {
            float maxDuration = 0;

            foreach (TweenData t in tweensData)
            {
                if ((t.showDelay + t.tween.duration) * t.showDurationScale > maxDuration)
                {
                    maxDuration = t.showDelay + t.tween.duration * t.showDurationScale;
                }
            }

            foreach (ControlData c in controlsData)
            {
                if ((c.showDelay + c.control.ShowDuration) > maxDuration)
                {
                    maxDuration = c.showDelay + c.control.ShowDuration;
                }
            }

            return maxDuration;
        }
    }

    public float HideDuration
    {
        get
        {
            float maxDuration = 0;

            foreach (TweenData t in tweensData)
            {
                if ((t.hideDelay + t.tween.duration) * t.showDurationScale > maxDuration)
                {
                    maxDuration = t.hideDelay + t.tween.duration * t.showDurationScale;
                }
            }

            foreach (ControlData c in controlsData)
            {
                if (c.control != null && (c.hideDelay + c.control.HideDuration) > maxDuration)
                {
                    maxDuration = c.hideDelay + c.control.HideDuration;
                }
            }

            return maxDuration;
        }
    }

    public void ActivateControl()
    {
        gameObject.SetActive(true);

        foreach (var td in tweensData)
        {
            td.tween.gameObject.SetActive(true);
        }

        foreach (var cd in controlsData)
        {
            cd.control.ActivateControl();
        }
    }

    public void DeactivateControl()
    {
        gameObject.SetActive(false);

        foreach (var td in tweensData)
        {
            td.tween.gameObject.SetActive(false);
        }

        foreach (var cd in controlsData)
        {
            cd.control.DeactivateControl();
        }
    }

	#endregion
 
	#region Public

    public virtual void Show()
    {
        Show(null);
    }

    public virtual void ShowImmediately()
    {
        Show(null, true);
    }

    public virtual void Show(System.Action<GUIControl> onFinished)
    {
        Show(onFinished, false);
    }

	public virtual void Show(System.Action<GUIControl> onFinished, bool immediately, bool callInvokes = true)
    {
        Show(onFinished, immediately, 0f, callInvokes);
    }

	public virtual void Show(System.Action<GUIControl> onFinished, bool immediately, float delay, bool callInvokes)
    {
        gameObject.SetActive(true);

        foreach (var obj in deactivateOnHideObjects)
        {
            obj.SetActive(true);
        }

        if (!gameObject.activeInHierarchy && !immediately)
        {
			Show(onFinished, true, delay, callInvokes);
            return;
        }

		ShowTweenData(immediately, delay);

        foreach (ControlData c in controlsData)
        {
			c.control.Show(null, immediately, c.showDelay + Random.Range(-c.showDelayVariance * 0.5f, c.showDelayVariance * 0.5f) + delay, callInvokes);
        }

        if(gameObject.activeInHierarchy)
			StartCoroutine(StartAnimation(new ActionWithInvokes(onFinished,callInvokes)));
    }

    public virtual void Hide()
    {
        Hide(null);
    }

    public virtual void HideImmediately()
    {
        Hide(null, true);
    }

    public virtual void Hide(System.Action<GUIControl> onFinished)
    {
        Hide(onFinished, false);
    }

	public virtual void Hide(System.Action<GUIControl> onFinished, bool immediately, bool callInvokes = true)
    {
        Hide(onFinished, immediately, 0f, callInvokes);
    }

	public virtual void Hide(System.Action<GUIControl> onFinished, bool immediately, float delay, bool callInvokes)
    {
        if (!gameObject.activeInHierarchy && !immediately)
        {
			Hide(onFinished, true, delay,callInvokes);
            return;
        }

		HideControls(immediately, delay, callInvokes);
       
        if (immediately)
        {
            DeactivateObjectsToHide();
            
            if (onFinished != null)
            {
                onFinished(this);
            }
        }
        else
        {          
            if (gameObject.activeInHierarchy)
                StartCoroutine(StartAnimation(new ActionWithInvokes(onFinished,callInvokes)));
        }
    }


	public virtual void HideControls()
	{
		HideControls(false, 0f, true);
	}


	public virtual void HideControls(bool immediately, float delay, bool callInvokes)
	{
		HideTweenData(immediately, delay);

		foreach (ControlData c in controlsData)
		{
			if (c.control == null)
			{
				CustomDebug.LogError(gameObject.name + ": Child control on GUIControl component not set!");
			}
			else
			{
				c.control.Hide(null, immediately, c.hideDelay + Random.Range(-c.showDelayVariance * 0.5f, c.showDelayVariance * 0.5f) + delay, callInvokes);
			}
		}
	}

    void DeactivateObjectsToHide()
    {
        foreach (var obj in deactivateOnHideObjects)
        {
            obj.SetActive(false);
        }
    }

	protected virtual IEnumerator StartAnimation(ActionWithInvokes actionWithInvokes)
    {
        while (IsAnimating)
        {
            yield return new WaitForSeconds(0.1f);           
        }

		if (actionWithInvokes.onFinished != null)
        {
			actionWithInvokes.onFinished(this);
        }        

        if (IsHidden)   
        {
            DeactivateObjectsToHide();

            if (CachedTransform1.localScale.x < 0.01f || CachedTransform1.localScale.y < 0.01f)
            {
                gameObject.SetActive(false);
            }
        }	
    }
    #endregion

	#region Private methods

	protected virtual void ShowTweenData(bool immediately, float delay)
	{
		foreach (TweenData t in tweensData)
		{
			if (immediately)
			{
				t.tween.SetEndStateImmediately();
				t.tween.enabled = false;
			}
			else
			{
				t.tween.scaleDuration = t.showDurationScale;
                t.tween.SetBeginStateImmediately();
				t.tween.SetEndState(t.showDelay + Random.Range(-t.showDelayVariance * 0.5f, t.showDelayVariance * 0.5f) + delay);
			}
		}
	}


	protected virtual void HideTweenData(bool immediately, float delay)
	{
		foreach (TweenData t in tweensData)
		{
			if (t.tween == null)
			{
				CustomDebug.LogError(gameObject.name + ": Tween on GUIControl component not set!");
			}
			else
			{
				if (immediately)
				{
					t.tween.SetBeginStateImmediately();
					t.tween.enabled = false;
				}
				else
				{
					t.tween.scaleDuration = t.hideDurationScale;
					t.tween.SetBeginState( t.hideDelay + Random.Range(-t.showDelayVariance * 0.5f, t.showDelayVariance * 0.5f) + delay);
				}
			}
		}

	}

	#endregion
}