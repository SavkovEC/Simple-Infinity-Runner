#if !__cplusplus && !__OBJC__
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

public class TextureHelper : SingletonMonoBehaviour<TextureHelper>
{
#endif



	enum Format
	{
		Format_RGBA32 		= 0,

		Format_RGB16		= 8,

		Format_A8			= 16,
	};


	struct LoadResult
	{
#if !__cplusplus && !__OBJC__
		public 
#endif
		IntPtr	texturePtr;

#if !__cplusplus && !__OBJC__
		public
#endif
		int		width;

#if !__cplusplus && !__OBJC__
		public
#endif
		int 		height;
	};



#if !__cplusplus && !__OBJC__
	static Format TextureFormatToInternal(TextureFormat fmt)
	{
		switch (fmt)
		{
		case TextureFormat.RGB565:
			return Format.Format_RGB16;

		case TextureFormat.Alpha8:
			return Format.Format_A8;

		default:
			return Format.Format_RGBA32;
		}
	}


	struct AsyncResult
	{
		public long texturePtr;
		public int width;
		public int height;
		public string loadKey;
	}


	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	static extern LoadResult LLTextureHelperLoadImageAtPath(string imagePath, int doMipMaps, int format);

	[DllImport ("__Internal")]
	static extern string LLTextureHelperLoadImageAtPathAsync(string imagePath, int doMipMaps, int format);

	[DllImport ("__Internal")]
	static extern void LLTextureHelperReleaseTexture(IntPtr textureName);
	#endif


	struct AsyncRequest
	{
		public System.Action<Texture2D> callback;
		public bool doMipMaps;
		public TextureFormat textureFormat;
        public string path;
	}

	Dictionary<string, AsyncRequest> requests = new Dictionary<string, AsyncRequest>();
    static TextureHelper inst = null;


	public static new TextureHelper Instance
	{
		get
		{
            if (!inst)
            {
                inst = SingletonMonoBehaviour<TextureHelper>.InstanceIfExist;
                if (inst == null)
                {
                    GameObject g = new GameObject("TextureHelper");
                    inst = g.AddComponent<TextureHelper>();
                    g.hideFlags = HideFlags.HideAndDontSave;
                }
            }

			return inst;
		}
	}


	public Texture2D LoadImageToTexture(string imagePath, bool doMipMaps = false, TextureFormat curFormat = TextureFormat.RGBA32)
	{
		Texture2D resultTexture = null;

		#if UNITY_IPHONE && !UNITY_EDITOR
		LoadResult lResult = LLTextureHelperLoadImageAtPath(imagePath, doMipMaps ? 1 : 0, (int)TextureFormatToInternal(curFormat));
		if (lResult.texturePtr != IntPtr.Zero)
		{
			resultTexture = Texture2D.CreateExternalTexture(lResult.width, lResult.height, curFormat, doMipMaps, false, lResult.texturePtr);
		}
        #elif UNITY_ANDROID && !UNITY_EDITOR

        Texture2D result2DTexture = new Texture2D(16, 16, curFormat, doMipMaps);

        AndroidJavaClass fileManagerClass = new AndroidJavaClass("com.inventain.android.FileManager");
        string imgInfo = fileManagerClass.CallStatic<string>("GetTexturePointer", imagePath);

        string[] parts = imgInfo.Split(' ');

        if (parts.Length > 2)
        {
            result2DTexture = Texture2D.CreateExternalTexture(int.Parse(parts[0]), int.Parse(parts[1]), TextureFormat.ARGB32, doMipMaps, false, (IntPtr)int.Parse(parts[2]));                 
        }
        resultTexture = result2DTexture;

        #else
		Texture2D result2DTexture = new Texture2D(16, 16, curFormat, doMipMaps);

		byte[] curImageBytes = null;
		if (File.Exists(imagePath))
		{
			curImageBytes = File.ReadAllBytes(imagePath);
		}
		else
		{
			result2DTexture = null;
			CustomDebug.LogError("File <" + imagePath + "> does not exist");
		}
		
		if (curImageBytes == null || !result2DTexture.LoadImage(curImageBytes))
		{
			if (curImageBytes == null)
			{
				CustomDebug.LogError("Image bytes is null");
			}
			else
			{
				CustomDebug.Log("Can't load image", DebugGroup.Resources);
			}
			result2DTexture = null;
		}

        if(result2DTexture && 
            (curFormat == TextureFormat.RGBA32 ||
                curFormat == TextureFormat.ARGB32))
        {
            Color[] colors = result2DTexture.GetPixels();
            for (int i = 0, colorsLength = colors.Length; i < colorsLength; i++)
            {
                Color c = colors[i];
                c.r *= c.a;
                c.g *= c.a;
                c.b *= c.a;
                colors[i] = c;
            }
            result2DTexture.SetPixels(colors);
            result2DTexture.Apply();
        }

		resultTexture = result2DTexture;

		#endif


		if (resultTexture != null)
		{
            resultTexture.name = System.IO.Path.GetFileName(imagePath);
			resultTexture.hideFlags = HideFlags.DontSave;
            resultTexture.wrapMode = TextureWrapMode.Clamp;
		}


		return resultTexture;
	}


	//<summary>
	// returns load key with witch request can be cancelled
	//</summary>
	public string LoadImageToTextureAsync(string imagePath, System.Action<Texture2D> callback, bool doMipMaps = false, TextureFormat curFormat = TextureFormat.RGBA32)
	{
		#if UNITY_IPHONE && !UNITY_EDITOR
		string loadKey = LLTextureHelperLoadImageAtPathAsync(imagePath, doMipMaps ? 1 : 0, (int)TextureFormatToInternal(curFormat));

		AsyncRequest request;
		request.callback = callback;
		request.doMipMaps = doMipMaps;
		request.textureFormat = curFormat;
        request.path = imagePath;
		requests.Add(loadKey, request);

		return loadKey;
		#else
		callback(LoadImageToTexture(imagePath, doMipMaps, curFormat));
		return "";
		#endif
	}


	public void CancelLoadAsync(string loadKey)
	{
		requests.Remove(loadKey);
	}


	public void UnloadTexture(Texture2D texture)
	{
		#if UNITY_IPHONE && !UNITY_EDITOR
		LLTextureHelperReleaseTexture(texture.GetNativeTexturePtr());
        #elif UNITY_ANDROID  && !UNITY_EDITOR
        AndroidJavaClass fileManagerClass = new AndroidJavaClass("com.inventain.android.FileManager");
        fileManagerClass.CallStatic("DeleteTexture", (int)texture.GetNativeTexturePtr());
		#else

		#endif

		// Can be commented out,
		// left here to indicate a moment where Unity will start to release it's textures
		if(Application.isPlaying)
		{
			Destroy(texture);
		}
		else
		{
			DestroyImmediate(texture);
		}
	}


	void Native_AsyncLoadCallback(string JSON_loadResult)
	{
		var result = MiniJSON.Json.Deserialize<AsyncResult>(JSON_loadResult);
		IntPtr curPtr = new IntPtr(result.texturePtr);

		if (curPtr != IntPtr.Zero)
		{
			AsyncRequest curRequest;
			if (requests.TryGetValue(result.loadKey, out curRequest))
			{
				Texture2D resultTexture = null;

				#if UNITY_IPHONE && !UNITY_EDITOR
				resultTexture = Texture2D.CreateExternalTexture(result.width, result.height, curRequest.textureFormat, curRequest.doMipMaps, false, curPtr);
				#endif
                
                if (resultTexture != null)
                {
                    resultTexture.name = System.IO.Path.GetFileName(curRequest.path);
                    resultTexture.hideFlags = HideFlags.DontSave;
                    resultTexture.wrapMode = TextureWrapMode.Clamp;
                }

				curRequest.callback(resultTexture);
			}
			else
			{
				// there's no one to catch a texture -> release it
				// loading was cancelled
				#if UNITY_IPHONE && !UNITY_EDITOR
				LLTextureHelperReleaseTexture(curPtr);
				#endif
			}
		}
	}
}
#endif