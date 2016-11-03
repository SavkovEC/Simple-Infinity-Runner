using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class AssetLink : Asset
{
	const string RESOURCE_FOLDER = "Resources/";
	const string STREAMING_ASSETS_FOLDER = "StreamingAssets/";


	public string name;
	public string assetGUID;

	protected string fullPath;
	protected string resourcePath;
	protected string streamingAssetPath;


	public string FullPath
	{
		get
		{
			if(string.IsNullOrEmpty(fullPath))
			{
				fullPath = GUIDMapper.GUIDToAssetPath(assetGUID);
			}
			return fullPath;
		}
	}


	public string ResourcePath
	{
		get
		{
			if(string.IsNullOrEmpty(resourcePath))
			{
				int startIndex = FullPath.IndexOf(RESOURCE_FOLDER, System.StringComparison.Ordinal);
				if(startIndex > -1)
				{
					startIndex += RESOURCE_FOLDER.Length;
					int lastIndex = FullPath.LastIndexOf('.');
					resourcePath = FullPath.Substring(startIndex, lastIndex - startIndex);
				}
				else
				{
					resourcePath = "";
				}
			}

			return resourcePath;
		}
	}


	public string StreamingAssetPath
	{
		get
		{
			if(string.IsNullOrEmpty(streamingAssetPath))
			{
				int startIndex = FullPath.IndexOf(STREAMING_ASSETS_FOLDER, System.StringComparison.Ordinal);
				if(startIndex > -1)
				{
					streamingAssetPath = Application.streamingAssetsPath + "/" + FullPath.Substring(startIndex + STREAMING_ASSETS_FOLDER.Length);
				}
				else
				{
                    streamingAssetPath = string.Empty;
				}
			}

			return streamingAssetPath;
		}
	}


    public string StreamingAssetPathRelative
    {
        get
        {
            string relativePath = string.Empty;
                
            int startIndex = FullPath.IndexOf(STREAMING_ASSETS_FOLDER, System.StringComparison.Ordinal);
            if(startIndex > -1)
            {
                relativePath = FullPath.Substring(startIndex + STREAMING_ASSETS_FOLDER.Length);
            }

            return relativePath;
        }
    }


	public override Object asset
	{
		get
		{
			#if UNITY_EDITOR
			return UnityEditor.AssetDatabase.LoadAssetAtPath(FullPath, type);
			#else
			return Resources.Load(ResourcePath, type);
			#endif
		}
	}

	
	protected virtual void SetAsset(Object asset)
	{
        #if UNITY_EDITOR
		if(asset != null)
		{
			string newAssetPath = UnityEditor.AssetDatabase.GetAssetPath(asset);
			assetGUID = UnityEditor.AssetDatabase.AssetPathToGUID(newAssetPath);
			name = asset.name;
		}
		else
		{
			assetGUID = string.Empty;
			name = string.Empty;
		}

		fullPath = string.Empty;
		resourcePath = string.Empty;
		streamingAssetPath = string.Empty;
        #endif
	}
	


	public AssetLink(Object asset)
	{
		this.SetAsset(asset);
	}


    public AssetLink(string path)
    {
        name = System.IO.Path.GetFileNameWithoutExtension(path);
        fullPath = path;
    }
}
