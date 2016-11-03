using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SoundsManager : SingletonMonoBehaviour<SoundsManager>
{
    #region Variables

    [SerializeField] public AudioListener AudioListenerReference;
    [SerializeField] private StreamingAudioBuffer streamingAudioBuffer;
    [SerializeField] public SoundsCategory[] AudioCategories;
    [SerializeField] public SoundSettings SoundSettings;

    [SerializeField] public SoundObject SimpleSoundPrefab;
    [SerializeField] public SoundObject NativeSoundPrefab;
    [SerializeField] public SoundObject StreamingSoundPrefab;

    [SerializeField] public bool Persistent = false;
    [SerializeField] public bool UnloadAudioClipsOnDestroy = false;
    [SerializeField] public bool UsePooledAudioObjects = true;
    [SerializeField] public bool PlayWithZeroVolume = false;

    [SerializeField] private bool audioDisabled = false;
    [SerializeField] private float mainVolume = 1.0f;

    protected AudioListener currentAudioListener = null;
    protected SoundObject currentMusic;

    private static double lastSystemTime = -1;
    private static double systemDeltaTime = -1;
    private static double systemTime = -1;
    private Vector3 listenerLastPosition;
	bool editorPaused = false;//editor only
    #endregion



    #region Properties

    public StreamingAudioBuffer StreamingAudioBuffer
    {
        get
        {
            return gameObject.GetRequiredComponent<StreamingAudioBuffer>(ref streamingAudioBuffer);
        }
        set
        {
            streamingAudioBuffer = value;
        }
    }


    public bool DisableAudio
    {
        get
        {
            return audioDisabled;
        }
        set
        {
            if (audioDisabled != value)
            {
				PauseAllSounds(value, "DisableAudio");
                audioDisabled = value;
            }
        }
    }


    public float Volume
    {
        get 
        { 
            return mainVolume;
        }
        set 
        { 
            if ( value != mainVolume ) 
            { 
                mainVolume = value; 
                ApplyVolumeChange(); 
            } 
        }
    }

    #endregion



    #region Music

    public static SoundObject PlayMusic( string audioID ) 
    { 
        return PlayMusic( audioID, 1, 0, 0 ); 
    }


    public static SoundObject PlayMusic( string audioID, float volume, float delay, float startTime )
    {
        AudioListener al = GetCurrentAudioListener();
        if ( al == null )
        {
            Debug.LogWarning( "No AudioListener found in the scene" );
            return null;
        }
        return Instance._PlayMusic( audioID, al.transform.position + al.transform.forward, null, volume, delay );
    }


    static public bool StopMusic( float fadeOut = 0 )
    { 
        return Instance._StopMusic( fadeOut ); 
    }


    public static bool PauseMusic( float fadeOut = 0 )
    { 
        return Instance._PauseMusic( fadeOut ); 
    }


    public static bool UnpauseMusic( float fadeIn = 0 )
    {
        return Instance._UnpauseMusic( fadeIn ); 
    }


    public static bool IsMusicPaused()
    {
        return Instance._IsMusicPaused();
    }


    static public SoundObject GetCurrentMusic()
    {
        return Instance.currentMusic;
    }



    protected bool _StopMusic( float fadeOut )
    {
        if ( currentMusic != null )
        {
            currentMusic.Stop( fadeOut );
            currentMusic = null;
            return true;
        }
        return false;
    }


    protected bool _PauseMusic( float fadeOut )
    {
        if ( currentMusic != null )
        {
            currentMusic.Pause( true, "", fadeOut );
            return true;
        }
        return false;
    }


    protected SoundObject _PlayMusic( string audioID, Vector3 position, Transform parentObj, float volume, float delay)
    {

        if ( currentMusic != null && currentMusic.IsPlaying )
        {
            currentMusic.Stop(currentMusic.SubItem.FadeOut);
        }

        currentMusic = Play(audioID, volume, position, parentObj, false, null);
        currentMusic.IsMusic = true;

        return currentMusic;
    }


    public bool _IsMusicPaused()
    {
        if ( currentMusic != null )
        {
            return currentMusic.IsPaused;
        }
        return false;
    }


    public bool _UnpauseMusic( float fadeIn = 0)
    {
        if ( currentMusic != null && currentMusic.IsPaused )
        {
			currentMusic.Pause( false, "" );
            return true;
        }
        return false;
    }


    #endregion Music



    #region Public interface

    public static SoundObject Play( string audioID )
    {
        AudioListener al = GetCurrentAudioListener();
        if ( al == null )
        {
            Debug.LogWarning( "No AudioListener found in the scene" );
            return null;
        }

        return Play( audioID, al.transform.position + al.transform.forward, null, 1);
    }


    public static SoundObject Play( string audioID, Transform parentObj)
    {
        return Instance.Play( audioID, 1f, parentObj.position, parentObj, false );
    }


    public static SoundObject Play( string audioID, Vector3 pos)
    {
        return Instance.Play( audioID, 1f, pos, null, false );
    }


    public static SoundObject Play( string audioID, Vector3 worldPosition, Transform parentObj, float volume)
    {
        return Instance.Play( audioID, volume, worldPosition, parentObj, false );
    }


    protected SoundObject Play( string audioID, float volume, Vector3 worldPosition, Transform parentObj, bool playWithoutAudioObject, SoundObject useExistingAudioObject = null)
    {
        if (audioDisabled)
        {
            return null;
        }

        SoundItem sndItem = GetAudioItem( audioID );
        if ( sndItem == null )
        {
            Debug.LogWarning( "Audio item with name '" + audioID + "' does not exist" );
            return null;
        }

        if ( sndItem.lastPlayedTime > 0)
        {
            if ( SoundsManager.systemTime < sndItem.lastPlayedTime + SoundItem.MIN_TIME_BETWEEN_PLAY_CALLS )
            {
                return null;
            }
        }

        if ( sndItem.MaxInstanceCount > 0 )
        {
            var playingAudios = GetPlayingAudioObjects( audioID );  // TODO: check performance of GetPlayingAudioObjects, maybe it always first
            bool isExceeding = playingAudios.Length >= sndItem.MaxInstanceCount;

            if ( isExceeding )
            {
                if (sndItem.SkipWhenExeeded)
                {
                    return null;
                }

                SoundObject oldestAudio = null;

                for ( int i = 0; i < playingAudios.Length; i++ )
                {
                    if ( oldestAudio == null || playingAudios[ i ].StartPlayingTime < oldestAudio.StartPlayingTime )
                    {
                        oldestAudio = playingAudios[ i ];
                    }
                }

                if ( oldestAudio != null )
                {
					oldestAudio.Stop( oldestAudio.SubItem.FadeOut );
                }
            }
        }

        return PlayAudioItem( sndItem, worldPosition, parentObj, playWithoutAudioObject, useExistingAudioObject);
    }


    public SoundObject PlayAudioItem( SoundItem sndItem, Vector3 worldPosition, Transform parentObj = null, bool playWithoutAudioObject = false, SoundObject useExistingAudioObj = null)
    {
        SoundSubItem[] sndSubItems = sndItem.ChooseSubItems( sndItem.SubItemPickMode );
        if ( sndSubItems == null || sndSubItems.Length == 0 )
        {
            return null;
        }
        sndItem.lastPlayedTime = SoundsManager.systemTime;

        SoundObject sndObj = null;

        for (int i = 0; i < sndSubItems.Length; i++)
        {
            var sndSubItem = sndSubItems[i];
            if (sndSubItem != null)
            {
                sndObj = PlayAudioSubItem(sndItem.Category, sndItem, sndSubItem, worldPosition, parentObj, playWithoutAudioObject, null, null);
            }
        }

        return sndObj;
    }



    public SoundObject PlayAudioSubItem(SoundsCategory audioCategory, SoundItem audioItem, SoundSubItem subItem,Vector3 worldPosition, Transform parentObj, bool playWithoutAudioObject, SoundObject existingAudioObj, System.Action<SoundObject> onLoad = null )
    {
        GameObject audioPrefab = GetPrefabForSoundType(subItem.SubItemType);
        if (audioPrefab == null)
        {
            Debug.LogError("missing prefab for sound : " + audioCategory.Name + "/"  + audioItem.Name + "   with type : " + subItem.SubItemType);
            Debug.LogError(subItem.clipref.FullPath);
            return null;
        }

        //volume
        float subItemVolume = subItem.Volume;
        if (subItem.RandomVolume != 0)
        {
            subItemVolume += UnityEngine.Random.Range(-subItem.RandomVolume, subItem.RandomVolume);
            subItemVolume = Mathf.Clamp01(subItemVolume);
        }
        float masterVolume = Volume * audioCategory.VolumeTotal * audioItem.Volume;
        float totalVolume = masterVolume * subItemVolume;
        if (!PlayWithZeroVolume && (totalVolume <= 0))
        {
            return null;
        }

        SoundSettings audioSettings = SoundSettings;
        if (audioCategory.SoundSettingsOverride != null)
        {
            audioSettings = audioCategory.SoundSettingsOverride;
        }
        if (audioItem.SoundSettingsOverride != null)
        {
            audioSettings = audioItem.SoundSettingsOverride;
        }


        GameObject sndObjInstance;
        SoundObject sndObj;
        if ( existingAudioObj == null )
        {
            sndObjInstance = (GameObject) ObjectPoolController.Instantiate( audioPrefab, worldPosition, Quaternion.identity );
            if (! audioItem.DestroyOnLoad )
            {
                Object.DontDestroyOnLoad( sndObjInstance );
            }

            sndObjInstance.transform.parent = parentObj ? parentObj : transform;
            sndObj = sndObjInstance.gameObject.GetComponent<SoundObject>();
        }
        else
        {
            sndObjInstance = existingAudioObj.gameObject;
            sndObj = existingAudioObj;
        }
        sndObjInstance.name = "SoundObject:" + subItem.Name;


        sndObj.SubItem = subItem;
        sndObj.ApplySettings(audioSettings);
        //sndObj.primaryAudioSource.rolloffMode = AudioRolloffMode.Linear;

//        float startTime = subItem.RandomStartPosition ? UnityEngine.Random.Range( 0, sndObj.clipLength ) : 0;
//        sndObj.primaryAudioSource.time = startTime + subItem.ClipStartTime;

        if (audioItem.overrideAudioSourceSettings)
        {
            sndObj.MinDistance = audioItem.audioSource_MinDistance;
            sndObj.MaxDistance = audioItem.audioSource_MaxDistance;
        }

        sndObj.Volume = subItemVolume;
        sndObj.MasterVolume = masterVolume;
        sndObj.Loop = ( audioItem.Loop == SoundLoopMode.LoopSubitem );
        sndObj.Pan = subItem.Pan2D;
        sndObj.Pitch = subItem.GetPitch();

        float delay = 0;
        if ( subItem.RandomDelay > 0 )
        {
            delay += UnityEngine.Random.Range( 0, subItem.RandomDelay );
        }

        sndObj.Play( delay + subItem.Delay + audioItem.Delay , subItem.FadeIn);


        if ( onLoad != null )
        {
            onLoad(sndObj);
        }

        return sndObj;
    }


    public static bool Stop( string audioID, float fadeOut = 0 )
    {
        SoundItem sndItem = Instance.GetAudioItem( audioID );

        if ( sndItem == null )
        {
            Debug.LogWarning( "Audio item with name '" + audioID + "' does not exist" );
            return false;
        }

        SoundObject[ ] audioObjs = GetPlayingAudioObjects( audioID );
        for (int i = 0; i < audioObjs.Length; i++)
        {
            SoundObject audioObj = audioObjs[i];
            audioObj.Stop(fadeOut);
        }
        return audioObjs.Length > 0;
    }


    public GameObject GetPrefabForSoundType(SoundClipType clipType)
    {
        GameObject prefab = null;
        switch( clipType )
        {
            case SoundClipType.Clip:
                prefab = SimpleSoundPrefab.gameObject;
                break;
            case SoundClipType.StreamingClip:
                prefab = StreamingSoundPrefab.gameObject;
                break;
            case SoundClipType.Native:
                prefab = NativeSoundPrefab.gameObject;
                break;
            case SoundClipType.Unknown:
                prefab = null;
                break;
        }

        return prefab;
    }


    public static AudioListener GetCurrentAudioListener()
    {
        SoundsManager soundManager = Instance;

        if (soundManager && !soundManager.currentAudioListener)
        {
            if (soundManager.AudioListenerReference != null)
            {
                soundManager.currentAudioListener = soundManager.AudioListenerReference;
            }
            else
            {
                soundManager.currentAudioListener = (AudioListener) FindObjectOfType( typeof( AudioListener ) );
            }
        }

        return soundManager.currentAudioListener;
    }

    static List<SoundObject> matchesList = new List<SoundObject>();
    public static SoundObject[ ] GetPlayingAudioObjects( string soundID, bool includePausedAudio = false )
    {
        matchesList.Clear();

        object[ ] objs = RegisteredComponentController.GetAllOfType( typeof( SoundObject ) );
        for ( int i = 0; i < objs.Length; i++ )
        {
            var sound = objs[ i ] as SoundObject;
            if ( sound.IsPlaying || ( includePausedAudio && sound.IsPaused ) )
            {
                if (string.IsNullOrEmpty(soundID) || sound.SoundID.Equals(soundID))
                {
                    matchesList.Add( sound );
                }
            }
        }

        return matchesList.ToArray();
    }

    static List<string> matchesListString = new List<string>();
    static public List<string> GetUsedAudioClipNames()
    {
        matchesListString.Clear();

        object[ ] objs = RegisteredComponentController.GetAllOfType(typeof( SoundObject ) );
        for ( int i = 0; i < objs.Length; i++ )
        {
            var sound = objs[ i ] as SoundObject;
            if ( sound.IsPlaying || sound.IsPaused )
            {
                if (sound.SubItem.SubItemType == SoundClipType.StreamingClip)
                {
                    matchesListString.Add(sound.SubItem.Name);
                }
            }
        }

        return matchesListString;
    }


    public static void StopAllSounds( float fadeOut = 0 )
    {
        SoundObject[ ] objs = GetPlayingAudioObjects(string.Empty, true );
        for (int i = 0; i < objs.Length; i++)
        {
            SoundObject o = objs[i];
            o.Stop(fadeOut);
        }
    }


	static public void PauseCategory( string categoryName, string pauseSource = "")
    {
        SoundObject[ ] objs = GetPlayingAudioObjects( string.Empty, false );
        for (int i = 0; i < objs.Length; i++)
        {
            SoundObject so = objs[i];
            if (so.DoesBelongToCategory(categoryName))
            {
                so.Pause(true, pauseSource);
            }
        }
    }


    public static bool IsPlaying( string audioID )
    {
        return GetPlayingAudioObjects( audioID ).Length > 0;
    }


    public static void SetCategoryVolume( string categoryName, float volume )
    {
        bool found = false;
        SoundsCategory cat = Instance.GetCategory( categoryName );

        if ( cat != null )
        {
            cat.Volume = volume;
            found = true;
        }

        if( !found )
        {
            Debug.LogWarning( "No audio category with name " + categoryName );
        }
    }


    static public float GetCategoryVolume( string categoryName )
    {
        SoundsCategory category = Instance.GetCategory( categoryName );
        if ( category != null )
        {
            return category.Volume;
        }
        else
        {
            Debug.LogWarning( "No audio category with name " + categoryName );
            return 0;
        }
    }

    #endregion



    #region Private

    private static void UpdateSystemTime()
    {
        double newSystemTime = SystemTime.timeSinceLaunch;

        if ( lastSystemTime >= 0 )
        {
            systemDeltaTime = newSystemTime - lastSystemTime;
            systemTime += systemDeltaTime;
        }
        else
        {
            systemDeltaTime = 0;
            systemTime = 0;
        }

        lastSystemTime = newSystemTime;
    }


    public void ValidateSounds()
    {
        for (int i = 0; i < AudioCategories.Length; i++)
        {
            AudioCategories[i].Initialize(this);
        }
    }


    internal SoundItem GetAudioItem( string audioID )
    {
        ValidateSounds();

        SoundItem sndItem;
        Dictionary<string, SoundItem> audioItems = AllSoundItems();

        if ( audioItems.TryGetValue( audioID, out sndItem ) )
        {
            return sndItem;
        }

        return null;
    }

    Dictionary<string, SoundItem> items = new Dictionary<string, SoundItem>();
    internal Dictionary<string, SoundItem> AllSoundItems()
    {
        items.Clear();
        for (int i = 0; i < AudioCategories.Length; i++)
        {
            SoundsCategory category = AudioCategories[i];
            for (int j = 0; j < category.AudioItems.Length; j++)
            {
                SoundItem item = category.AudioItems[j];
                items.Add(item.Name, item);
            }
        }

        return items;
    }


	public void PauseAllSounds( bool isPaused, string pauseSource)
    {
		SoundObject[ ] objs = GetPlayingAudioObjects(string.Empty, true );

		for (int i = 0; i < objs.Length; i++)
        {
            SoundObject o = objs[i];
            o.Pause(isPaused, pauseSource);
        }
    }


    private void ApplyVolumeChange()
    {
//        SoundObject[ ] objs = GetPlayingAudioObjects(string.Empty, true );
//
//        foreach ( SoundObject o in objs )
//        {
////            o._ApplyVolumeBoth();
//        }
    }


    public SoundsCategory GetCategory(string categoryName)
    {
        for (int i = 0; i < AudioCategories.Length; i++)
        {
            SoundsCategory cat = AudioCategories[i];
            if (cat.Name.Equals(categoryName))
            {
                return cat;
            }
        }
        return null;
    }

    #endregion



    #region Unity

    #if UNITY_EDITOR
    protected override void Awake()
    {
        base.Awake();

        UnityEditor.EditorApplication.update -= EditorUpdate;
        UnityEditor.EditorApplication.update += EditorUpdate;
    }


    void EditorUpdate()
    {
        if(editorPaused ^ UnityEditor.EditorApplication.isPaused)
        {
            editorPaused = UnityEditor.EditorApplication.isPaused;
            OnApplicationPause(editorPaused);
        }
    }
    #endif


    void Update()
    {
        UpdateListenerPosition();
        UpdateSystemTime();
    }


    void UpdateListenerPosition()
    {
        AudioListener listener = GetCurrentAudioListener();
        if (listener != null)
        {
            Vector3 pos = listener.transform.position;
            if (pos != listenerLastPosition)
            {
                LLSoundManager.SetListenerPosition(pos);
            }
            listenerLastPosition = pos;
        }
    }


    protected override void OnDestroy()
    {
        StopAllSounds();

        if ( UnloadAudioClipsOnDestroy )
        {
//            UnloadAllAudioClips();
        }

        base.OnDestroy();
    }


    void OnApplicationPause(bool pauseStatus) 
    {
		PauseAllSounds(pauseStatus, "OnApplicationPause");
    }
    #endregion



    #if UNITY_EDITOR

    #region Editor
    public InspectorSelection _currentInspectorSelection = new InspectorSelection();

    [System.Serializable]
    public class InspectorSelection
    {
        public int currentCategoryIndex = 0;
        public int currentItemIndex = 0;
        public int currentSubitemIndex = 0;
        public int currentPlaylistIndex = 0;
    }
    #endregion

    #endif

}