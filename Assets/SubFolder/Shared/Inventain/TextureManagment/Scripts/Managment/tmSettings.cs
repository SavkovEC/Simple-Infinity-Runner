using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class tmSettings : ScriptableSingleton<tmSettings>
{
	public tmPlatform[] texturePlatforms = new tmPlatform[] 
	{
		new tmPlatform("iPhone", "@2x", 2.0f, tmUtility.NewGUID()),
		new tmPlatform("iPad Retina", "@4x", 4.0f, tmUtility.NewGUID())
	};


	public const string ResourceFolder = "tmResources/";
	public const string AssetsFolder = "Assets/Resources/" + ResourceFolder;
	public const string ResourceLinkPrefix = "tm";

	#if UNITY_EDITOR
	[SerializeField]
	#endif
	tmPlatform currentPlatform;
	tmPlatform lightmapPlatform;

	[SerializeField] 
	tmPlatform targetPlatform;

	[SerializeField] [Tooltip("used for autorescaling textures")]
	tmPlatform defaultPlatform;

	public bool autoRebuild;
	[SerializeField] bool forceStaticGeometry;
	public bool rebuildMesh;
	public bool batching;


	public bool ForceStaticGeometry 
	{
		get
		{
			#if UNITY_EDITOR
			return forceStaticGeometry;
			#else
			return true;
			#endif
		}
		set
		{
			forceStaticGeometry = value;
		}
	}


	public tmPlatform CurrentPlatform 
	{
		get 
		{
			#if !UNITY_EDITOR
			if(currentPlatform == null || string.IsNullOrEmpty(currentPlatform.name))
			{
				if(Screen.height < 1900)
				{
					currentPlatform = GetPlatformWithName("iPhone");
				}
				else
				{
					currentPlatform = GetPlatformWithName("iPad Retina");
				}
				CustomDebug.Log("texture platform : " + currentPlatform.name);
			}
			#endif

			return currentPlatform;
		}
		#if UNITY_EDITOR
		set
		{
			currentPlatform = value;
		}
		#endif
	}


	public tmPlatform LightmapPlatform 
	{
		get 
		{
			#if !UNITY_EDITOR
			if(lightmapPlatform == null || string.IsNullOrEmpty(lightmapPlatform.name))
			{
				if(DeviceInfo.CurrentClass > DeviceInfo.PerformanceClass.iPhone4S)
				{
					lightmapPlatform = GetPlatformWithName("iPad Retina");
				}
				else
				{
					lightmapPlatform = GetPlatformWithName("iPhone");
				}
				CustomDebug.Log("lightmap platform : " + lightmapPlatform.name);
			}
			return lightmapPlatform;
			#else
			return CurrentPlatform;
			#endif
		}
	}

	public tmPlatform TargetPlatform 
	{
		get { return targetPlatform; }
		set { targetPlatform = value; }
	}


	public tmPlatform DefaultPlatform 
	{
		get { return defaultPlatform; }
		set { defaultPlatform = value; }
	}


	public static tmPlatform[] allPlatfrorms 
	{
		get
		{
			return tmSettings.Instance.texturePlatforms;
		}
	}


	public static tmPlatform GetPlatformWithName(string platformName)
	{
		foreach(tmPlatform platform in tmSettings.Instance.texturePlatforms)
		{
			if(platform.name.Equals(platformName))
			{
				return platform;
			}
		}

		return null;
	}
}