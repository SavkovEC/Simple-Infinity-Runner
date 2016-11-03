using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SoundObject : RegisteredComponent 
{
    #region Variables

    [SerializeField] bool mute = false;
    [SerializeField] bool loop = false;

    [SerializeField] int priority = 128;
    [SerializeField] float volume = 1f;
    [SerializeField] float pitch = 1f;
    [SerializeField] float pan = 0;
    [SerializeField] float spatialBlend = 0;

    [SerializeField] float spread = 0;
    [SerializeField] float minDistance = 1;
    [SerializeField] float maxDistance = 500;

    [System.NonSerialized] SoundSubItem subItem;
    protected bool isDirty = false;
    protected float masterVolume = 1f;
    protected float startPlayingTime;
    protected bool isPaused = false;
	protected bool isPlaying = true;
    protected bool isStopped = false;
    protected float playingTime = 0;
    protected bool scheduledNextItem = false;

	HashSet<string> pauseSources = new HashSet<string>();
    SoundFader fader = new SoundFader();

    #endregion



    #region Properties

    #region Meta
    public bool Mute
    {
        get
        {
            return mute;
        }
        set
        {
            if (mute != value)
            {
                mute = value;
                IsDirty = true;
                ApplyMuteChange();
            }
        }
    }


    public bool Loop
    {
        get
        {
            return loop;
        }
        set
        {
            if(loop != value)
            {
                loop = value;
                IsDirty = true;
                ApplyLoopChange();
            }
        }
    }


    public int Priority
    {
        get
        {
            return priority;
        }
        set
        {
            priority = value;
        }
    }


    public float Volume
    {
        get
        {
            return volume;
        }
        set
        {
            if (volume != value)
            {
                volume = value;
                IsDirty = true;
                ApplyVolumeChange();
            }
        }
    }


    public float MasterVolume
    {
        get
        {
            return masterVolume;
        }
        set
        {
            if (masterVolume != value)
            {
                masterVolume = value;
                IsDirty = true;
                ApplyVolumeChange();
            }
        }
    }


    public float TotalVolume
    {
        get
        {
            return MasterVolume * Volume * fader.Fade;
        }
    }


    public float Pitch
    {
        get
        {
            return pitch;
        }
        set
        {
            if (pitch != value)
            {
                pitch = value;
                IsDirty = true;
                ApplyPitchChange();
            }
        }
    }


    public float Pan
    {
        get
        {
            return pan;
        }
        set
        {
            if (pan != value)
            {
                pan = value;
                IsDirty = true;
                ApplyPanChange();
            }
        }
    }


    public float Spread
    {
        get
        {
            return spread;
        }
        set
        {
            if (spread != value)
            {
                spread = value;
                IsDirty = true;
                ApplySpreadChange();
            }
        }
    }


    public float SpatialBlend
    {
        get
        {
            return spatialBlend;
        }
        set
        {
            if (spatialBlend != value)
            {
                spatialBlend = value;
                IsDirty = true;
                ApplySpatialBlendChange();
            }
        }
    }
        

    public float MinDistance
    {
        get
        {
            return minDistance;
        }
        set
        {
            if (minDistance != value)
            {
                minDistance = value;
                IsDirty = true;
                ApplyMinDistanceChange();
            }
        }
    }


    public float MaxDistance
    {
        get
        {
            return maxDistance;
        }
        set
        {
            if (maxDistance != value)
            {
                maxDistance = value;
                IsDirty = true;
                ApplyMaxDistanceChange();
            }
        }
    }
    #endregion Meta


    public string SoundID
    {
        get
        {
            return subItem.Item.Name;
        }
    }


    public SoundSubItem SubItem
    {
        get
        {
            return subItem;
        }
        internal set
        {
            subItem = value;
            ApplySubItem();
        }
    }


    public bool IsMusic
    {
        get;
        set;
    }


    public float StartPlayingTime
    {
        get
        {
            return startPlayingTime;
        }
        internal set
        {
            startPlayingTime = value;
        }
    }


    public float PlayingTime
    {
        get
        {
            return playingTime;
        }
        set
        {
            //not implemented yet
        }
    }


	public bool IsPlaying
	{
		get
		{
			return isPlaying;
		}
	}


	public bool IsPaused
    {
        get
		{
			return isPaused;
		}
		protected set
		{
			if(isPaused != value)
			{
				isPaused = value;
				ApplyPause();
			}
		}
    }


    protected bool IsDirty
    {
        get
        {
            return isDirty;
        }
        set
        {
            isDirty = value;
        }
    }

    #endregion



    #region Impementation

	protected virtual void ApplySubItem(){}
	protected virtual void ApplyPause(){}
    protected virtual void ApplyMuteChange(){}
    protected virtual void ApplyLoopChange(){}
    protected virtual void ApplyPriorityChange(){}
    protected virtual void ApplyVolumeChange(){}
    protected virtual void ApplyPitchChange(){}
    protected virtual void ApplyPanChange(){}
    protected virtual void ApplySpatialBlendChange(){}
    protected virtual void ApplySpreadChange(){}
    protected virtual void ApplyMinDistanceChange(){}
    protected virtual void ApplyMaxDistanceChange(){}

    #endregion



    #region Public

    public virtual void Play(float delay, float fadeIn = 0)
    {
        StopAllCoroutines ();

		if(gameObject.activeInHierarchy)
		{
			StartCoroutine(WaitForSeconds(delay, ()=> 
			                              {
                playingTime = 0;
				isPlaying = true;
				isPaused = false;
				pauseSources.Clear();
				
				fader.Reset();
				FadeIn(fadeIn);
				
				OnPlay();
				
				StartCoroutine(ElapseFader(fadeIn));
			}));
		}
    }


    public virtual void OnPlay()
    {

    }


    public void Pause(bool pause, string pauseSource, float fadeTime = 0)
    {
        if (isStopped)
        {
            return;
        }

		if(pause)
		{
			StopAllCoroutines ();

			FadeOut(fadeTime);

			pauseSources.Add(pauseSource);
			StartCoroutine(ElapseFader(fadeTime, ()=>
			{
				IsPaused = true;
			})); 
		}
		else
		{
			if(pauseSources.Remove(pauseSource) && pauseSources.Count == 0)
			{
				StopAllCoroutines ();

				FadeIn(fadeTime);

				StartCoroutine(ElapseFader(fadeTime, ()=>
				{
					IsPaused = false;
				}));  
			}
		}
    }


    public virtual void Stop(float fadeOut = 0)
    {
        if(!isStopped)
        {
            isStopped = true;

            StopAllCoroutines ();
            FadeOut(fadeOut);

            if(gameObject.activeInHierarchy && fadeOut > 0 && !isPaused)
            {
                StartCoroutine(ElapseFader(fadeOut, ()=>
                {
                    StopSound();
                })); 
            }
            else
            {
                StopSound();
            }
        }
    }


    internal virtual void StopSound()
    {
        playingTime = 0;
        isPlaying = false;
        isPaused = false;
        pauseSources.Clear();
        scheduledNextItem = false;
    }

    #endregion



	#region Private

	protected IEnumerator ElapseFader(float time, System.Action action = null)
	{
        time = Mathf.Max(0, time);
		float currentTime = 0;

		while(currentTime <= time)
		{
			fader.Elapsed = currentTime;
			IsDirty = true;
			ApplyVolumeChange();

			yield return null;

			currentTime += Time.deltaTime;
		}

		yield return null;

		if (action != null)
		{
			action();
		}
	}

	IEnumerator WaitForSeconds(float delay, System.Action action )
	{
		yield return new WaitForSeconds( delay );
		if (action != null)
		{
			action();
		}
	}

	protected void FadeIn(float fadeTime)
	{
		fader.FadeIn(fadeTime);
	}
	
	
	protected void FadeOut(float fadeTime)
	{
		fader.FadeOut(fadeTime);
	}

	#endregion



    #region Internal

    internal bool DoesBelongToCategory( string categoryName )
    {
        SoundsCategory curCategory = SubItem.Item.Category;

        while( curCategory != null )
        {
            if ( curCategory.Name == categoryName )
            {
                return true;
            }
            curCategory = curCategory.ParentCategory;
        }
        return false;
    }


    internal static void CopyParameters(SoundObject source, SoundObject target)
    {
        target.Mute = source.mute;
        target.Priority = source.priority;
        target.Volume = source.volume;
        target.masterVolume = source.masterVolume;
        target.Pitch = source.pitch;
        target.Pan = source.pan;
        target.SpatialBlend = source.spatialBlend;
        target.Spread = source.spread;
        target.MinDistance = source.minDistance;
        target.MaxDistance = source.maxDistance;
    }


    internal virtual void ApplySettings(SoundObject source)
    {
        SoundObject.CopyParameters(source, this);
    }


    internal virtual void DestroyAudioObject()
    {
		StopAllCoroutines ();
        ObjectPoolController.Destroy( gameObject );
    }


    internal virtual bool IsFinish()
    {
        return (!IsPlaying && !IsPaused);
    }

    #endregion



    #region Unity

    protected virtual void OnEnable()
    {

    }


    protected virtual void OnDisable()
    {
        Stop(0);
        DestroyAudioObject();
    }


    protected virtual void Update()
    {
        if (isPlaying && !isPaused)
        {
            MasterVolume = SoundsManager.Instance.Volume * SubItem.Item.Volume * SubItem.Item.Category.VolumeTotal; //TODO do it reactive
            playingTime += Time.unscaledDeltaTime;
        }
    }


    protected virtual void LateUpdate()
    {
        if (SubItem.Item.OverlapTime > 0 && !isStopped && PlayingTime > SubItem.Item.OverlapTime && !scheduledNextItem)
        {
            scheduledNextItem = true;
            if (IsMusic)
            {
                SoundsManager.PlayMusic(SoundID);
            }
            else
            {
                SoundsManager.Play(SoundID);
            }
            this.Stop(SubItem.FadeOut);
        }

        if (IsFinish())
        {
            DestroyAudioObject();
        }
        IsDirty = false;
    }
   

    protected override void OnDestroy()
    {
        Stop(0);
        base.OnDestroy();
    }

    #endregion
}
