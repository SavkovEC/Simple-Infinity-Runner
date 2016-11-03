using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public static class LLSoundManager
{
//	public static string SOUND_MANAGER = "com.inventain.android.SoundsManager";
//	public static string SOUND_MANAGER_INIT_OPTIONS = "InitOptions";
//	public static string SOUND_MANAGER_PLAY_SOUND = "LLSoundsManagerPlaySound";
//	public static string SOUND_MANAGER_UPDATE_SOUND = "LLSoundsManagerUpdateSound";
//	public static string SOUND_MANAGER_STOP_SOUND = "LLSoundsManagerStopSound";
//	public static string SOUND_MANAGER_PAUSE_SOUND = "LLSoundsManagerPauseSound";
//	public static string SOUND_MANAGER_LISTENER_POSITION = "LLSoundsManagerSetListenerPosition";
//
//
//    public delegate void LLCallbackDelegate( string str );
//
//
//	#if !UNITY_EDITOR || UNITY_STANDALONE_OSX
//
//    [DllImport ("LLLibSetOSX")]
//    static extern void LLSoundsManagerInit(string options);
//
//    [DllImport ("LLLibSetOSX")]
//    static extern void LLSoundsManagerRegisterCallback(LLCallbackDelegate callback);
//
//    [DllImport ("LLLibSetOSX")]
//    static extern void LLSoundsManagerPlaySound(
//        string uid, 
//        string playerName, 
//        string filePath, 
//        float volume, 
//        float pitch, 
//        bool looped,
//        float x, 
//        float y, 
//        float z, 
//        float minDistance, 
//        float maxDistance, 
//        float rolloff
//    );
//
//    [DllImport ("LLLibSetOSX")]
//    static extern void LLSoundsManagerUpdateSound(
//        string uid,
//        float volume, 
//        float pitch, 
//        bool looped,
//        float x, 
//        float y, 
//        float z, 
//        float minDistance, 
//        float maxDistance, 
//        float rolloff
//    );
//
//    [DllImport ("LLLibSetOSX")]
//    static extern void LLSoundsManagerStopSound(string uid);
//
//    [DllImport ("LLLibSetOSX")]
//    static extern void LLSoundsManagerPauseSound(string uid, bool pause);
//
//    [DllImport ("LLLibSetOSX")]
//    static extern void LLSoundsManagerSetListenerPosition(float x, float y, float z);
//
//    #elif UNITY_IPHONE
//
//    [DllImport ("__Internal")]
//    static extern void LLSoundsManagerInit(string options);
//
//    [DllImport ("__Internal")]
//    static extern void LLSoundsManagerRegisterCallback(LLCallbackDelegate callback);
//
//    [DllImport ("__Internal")]
//    static extern void LLSoundsManagerPlaySound(
//        string uid, 
//        string playerName, 
//        string filePath, 
//        float volume, 
//        float pitch, 
//        bool looped,
//        float x, 
//        float y, 
//        float z, 
//        float minDistance, 
//        float maxDistance, 
//        float rolloff
//    );
//
//    [DllImport ("__Internal")]
//    static extern void LLSoundsManagerUpdateSound(
//        string uid,
//        float volume, 
//        float pitch, 
//        bool looped,
//        float x, 
//        float y, 
//        float z, 
//        float minDistance, 
//        float maxDistance, 
//        float rolloff
//    );
//
//    [DllImport ("__Internal")]
//    static extern void LLSoundsManagerStopSound(string uid);
//
//    [DllImport ("__Internal")]
//    static extern void LLSoundsManagerPauseSound(string uid, bool pause);
//
//    [DllImport ("__Internal")]
//    static extern void LLSoundsManagerSetListenerPosition(float x, float y, float z);
//
//	#elif UNITY_ANDROID
//
//	static void LLSoundsManagerInit(string options)
//	{
//		AndroidPlugin.Instance.CallStatic(SOUND_MANAGER, SOUND_MANAGER_INIT_OPTIONS, options);
//	}
//
//
//	static void LLSoundsManagerRegisterCallback(LLCallbackDelegate callback)
//	{
//		AndroidPlugin.Instance.RegisterCallback(SOUND_MANAGER, LLSoundsManagerCallback);
//	}
//
//
//    static void LLSoundsManagerPlaySound(string uid, string playerName, string filePath, float volume, float pitch, bool looped, float x, float y, float z, float minDistance, float maxDistance, float rolloff)
//	{
//		AndroidPlugin.Instance.CallStatic
//		(
//			SOUND_MANAGER, 
//			SOUND_MANAGER_PLAY_SOUND, 
//
//			uid, 
//			playerName,
//			filePath,
//
//			volume,
//			pitch,
//			looped,
//
//			x,
//			y,
//			z,
//
//			minDistance,
//			maxDistance,
//			rolloff
//		);
//	}
//
//
//    static void LLSoundsManagerUpdateSound(string uid, float volume, float pitch, bool looped, float x, float y, float z, float minDistance, float maxDistance, float rolloff)
//	{
//		AndroidPlugin.Instance.CallStatic
//		(
//			SOUND_MANAGER, 
//			SOUND_MANAGER_UPDATE_SOUND, 
//
//			uid,
//
//			volume,
//			pitch,
//			looped,
//
//			x,
//			y,
//			z,
//
//			minDistance,
//			maxDistance,
//			rolloff
//		);
//	}
//
//
//	static void LLSoundsManagerStopSound(string uid)
//	{
//		AndroidPlugin.Instance.CallStatic(SOUND_MANAGER, SOUND_MANAGER_STOP_SOUND, uid);
//	}
//
//
//	static void LLSoundsManagerPauseSound(string uid, bool pause)
//	{
//		AndroidPlugin.Instance.CallStatic(SOUND_MANAGER, SOUND_MANAGER_PLAY_SOUND, uid, pause);
//	}
//
//
//	static void LLSoundsManagerSetListenerPosition(float x, float y, float z) 
//	{
//		AndroidPlugin.Instance.CallStatic(SOUND_MANAGER, SOUND_MANAGER_LISTENER_POSITION, x, y, z);
//	}
//	#endif



    public static event System.Action<string> OnCallbackRecieved;
    public static event System.Action<string> OnStoppedSound;

    private static readonly string TestPlayerName = "test_player";
    struct Options
    {
        public Dictionary<string, int> srcAmounts;
    }
    static bool initialized = false;


    public static void Initialize()
    {
//        if (!initialized)
//        {
//            Options curOptions;
//            curOptions.srcAmounts = new Dictionary<string, int>();
//            curOptions.srcAmounts.Add(TestPlayerName, 28);
//
//			LLSoundsManagerInit(JsonConvert.SerializeObject(curOptions));
//            LLSoundsManagerRegisterCallback(LLSoundsManagerCallback);
//
//			initialized = true;
//        }
    }  


//    [AOT.MonoPInvokeCallbackAttribute(typeof(LLCallbackDelegate))]
    static void LLSoundsManagerCallback(string json)
    {
//        if (OnCallbackRecieved != null)
//        {
//            OnCallbackRecieved(json);
//        }
//
//        Dictionary<string, string> dict = MiniJSON.Json.Deserialize<Dictionary<string, string>>(json);
//        if (dict.ContainsKey("stop"))
//        {
//            if (OnStoppedSound != null)
//            {
//                OnStoppedSound(dict["stop"]);
//            }
//        }
//        else if (dict.ContainsKey("error"))
//        {
//            Debug.LogError(dict["error"]);
//        }
    }


    public static void PlaySound(string fileName)
    {
//        PlaySound(fileName, fileName, 1f, 1f, false, Vector3.zero, 1f, 10000f, 1f);
    }


    public static void PlaySound(string uid, string filePath, float volume, float pitch, bool looped, Vector3 position, float minDistance, float maxDistance, float rolloff)
    {
//        Initialize ();
//        LLSoundsManagerPlaySound(uid, TestPlayerName, filePath, volume, pitch, looped, position.x, position.y, position.z, minDistance, maxDistance, rolloff);
    }


    public static void UpdateSound(string uid, float volume, float pitch, bool looped, Vector3 position, float minDistance, float maxDistance, float rolloff)
    {
//        LLSoundsManagerUpdateSound (uid, volume, pitch, looped, position.x, position.y, position.z, minDistance, maxDistance, rolloff);
    }


    public static void StopSound(string uid)
    {
//        LLSoundsManagerStopSound (uid);
    }


    public static void PauseSound(string uid, bool pause)
    {
//        LLSoundsManagerPauseSound (uid, pause);
    }


    public static void SetListenerPosition(Vector3 position)
    {
//        LLSoundsManagerSetListenerPosition(position.x, position.y, position.z);
    }
}

