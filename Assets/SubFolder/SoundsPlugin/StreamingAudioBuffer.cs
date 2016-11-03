using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StreamingAudioBuffer : MonoBehaviour 
{
    const string STREAMING_ASSETS_FOLDER = "StreamingAssets";

	#region Helper types

	struct BufferItem
	{
		public AudioClip Clip;
		public float MemorySize;
		public float LastLoadTime;


		public BufferItem(AudioClip clipParam, float memorySizeParam)
		{
			Clip = clipParam;
			MemorySize = memorySizeParam;
			LastLoadTime = float.MaxValue;
		}


		public void UpdateLastLoadTime()
		{
			LastLoadTime = Time.realtimeSinceStartup;
		}
	}

	#endregion


	#region Variables

	const float MAX_BUFFER_SIZE = 10f; // in MB
	const float CLIPS_SIZE_TO_UNLOAD = 5f; // in MB
	const float MB_MULTIPLIER = 0.00000097f;
	const float DELAY_TO_UNLOAD_CLIP = 0.5f;


	Dictionary<string, BufferItem> cachedClips = new Dictionary<string, BufferItem>();
	List<string> loadingClips = new List<string>();
	string streamingAssetsPath;
	float currentBufferSize;
	bool isBufferUnloadingAssets;


	bool IsBufferUnloadingAssets
	{
		get { return isBufferUnloadingAssets; }
		set
		{
			if (isBufferUnloadingAssets != value)
			{
				isBufferUnloadingAssets = value;

				if (value)
				{
					UnloadAssetsFromBuffer();
				}
			}
		}
	}

	#endregion


	#region Unity lifecycle

	void Awake()
	{
		streamingAssetsPath = GetStreamingAssetsPath();
	}

	#endregion


	#region Public methods

    public void LoadClip(string clipName, string clipPath, Action<AudioClip> onFinish)
	{
        if (cachedClips.ContainsKey(clipName))
		{
            cachedClips[clipName].UpdateLastLoadTime();

            onFinish(cachedClips[clipName].Clip);
		}
        else if (!loadingClips.Contains(clipName))
		{
            loadingClips.Add(clipName);

            StartCoroutine(LoadClipCoroutine(clipName, clipPath, onFinish));
		}
	}


	public void UnloadClip(string audioClipNameToUnload, float time = 0)
	{
		if (cachedClips.ContainsKey(audioClipNameToUnload))
		{
			currentBufferSize -= cachedClips[audioClipNameToUnload].MemorySize;

			Destroy(cachedClips[audioClipNameToUnload].Clip, time);
			cachedClips.Remove(audioClipNameToUnload);
		}
	}


	public void UnloadAssets()
	{
		IsBufferUnloadingAssets = true;
	}

	#endregion


	#region Private methods

    IEnumerator LoadClipCoroutine(string clipName, string clipPath, Action<AudioClip> onFinish)
	{
        int startIndex = clipPath.IndexOf(STREAMING_ASSETS_FOLDER, System.StringComparison.Ordinal);
        if(startIndex > -1)
        {
            clipPath = clipPath.Substring(startIndex + STREAMING_ASSETS_FOLDER.Length);
        }

        string fullpath = streamingAssetsPath + clipPath;

        using (WWW www = new WWW(fullpath))
        {
            yield return www;
		
            AudioClip audioClip = www.GetAudioClip(false, false, AudioType.WAV);
            audioClip.name = clipName;

            float bufferItemSize = www.bytesDownloaded * MB_MULTIPLIER;

            cachedClips.Add(clipName, new BufferItem(audioClip, bufferItemSize));
            loadingClips.Remove(clipName);

            if (onFinish != null)
            {
                onFinish(audioClip);
            }

            IncreaseBufferSize(bufferItemSize);
        }
	}


	void IncreaseBufferSize(float newAudioClipSize)
	{
		currentBufferSize += newAudioClipSize;

		if (currentBufferSize > MAX_BUFFER_SIZE)
		{
			IsBufferUnloadingAssets = true;
		}
	}


	void UnloadAssetsFromBuffer()
	{
		float unloadedClipsSize = 0;

        List<string> activeClipNames = SoundsManager.GetUsedAudioClipNames();

		BufferItem[] cachedClipsItems = new BufferItem[cachedClips.Values.Count];
		cachedClips.Values.CopyTo(cachedClipsItems, 0);

		List<BufferItem> bufferItems = new List<BufferItem>(cachedClipsItems);

		bufferItems.Sort((a,b) => a.LastLoadTime.CompareTo(b.LastLoadTime));

		List<string> audioClipNamesToUnload = new List<string>();

		for (int i = 0; i < bufferItems.Count; i++)
		{
			BufferItem item = bufferItems[i];

			if (!activeClipNames.Contains(item.Clip.name))
			{
				audioClipNamesToUnload.Add(item.Clip.name);
				unloadedClipsSize += item.MemorySize;
				
				if (unloadedClipsSize > CLIPS_SIZE_TO_UNLOAD)
				{
					break;
				}
			}
		}

		cachedClipsItems = null;
		bufferItems.Clear();

		float delay = 0;
		foreach (string clipName in audioClipNamesToUnload)
		{
			UnloadClip(clipName, delay);
			delay += DELAY_TO_UNLOAD_CLIP;
		}

		IsBufferUnloadingAssets = false;
	}


	string GetStreamingAssetsPath()
	{
		string path;
		#if UNITY_EDITOR
		path = "file://" + Application.dataPath + "/StreamingAssets";
		#elif UNITY_ANDROID
		path = "jar:file://"+ Application.dataPath + "!/assets/";
		#elif UNITY_IOS
		path = "file://" + Application.dataPath + "/Raw";
		#else
		//Desktop (Mac OS or Windows)
		path = "file://"+ Application.dataPath + "/StreamingAssets";
		#endif
		return path;
	}

	#endregion
}