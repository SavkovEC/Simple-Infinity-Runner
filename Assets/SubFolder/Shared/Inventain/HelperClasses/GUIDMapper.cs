using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using System.Text;
#endif


public static class GUIDMapper 
{
	#if UNITY_EDITOR

	[MenuItem("Assets/CollectGUIDs", false, 10001)]
	static void SvnAdd()
	{
		WriteGUIDs(Application.streamingAssetsPath + "/guids");
	}

//	[UnityEditor.Callbacks.PostProcessBuild(int.MaxValue - 1)]
//	public static void AddGUIDsToBuild(BuildTarget target, string path)
//	{
//		string filePath = path;
//		if (target == BuildTarget.iOS)
//		{
//			filePath += "/Data/Raw/guids";
//		}
//		else if (target == BuildTarget.Android)
//		{
//			filePath += "/" + Application.productName + "/assets/guids";
//		}
//
//		WriteGUIDs(filePath);
//	}


    [PreProcessBuild(1)]
    public static void AddGUIDsToBuild(BuildTarget target)
    {
        string filePath = Application.streamingAssetsPath + "/guids";
        WriteGUIDs(filePath);
    }


	public static void WriteGUIDs(string filePath)
	{
		string[] paths = AssetDatabase.GetAllAssetPaths();

		StringBuilder sb = new StringBuilder();
		for (int i = 0, pathsLength = paths.Length; i < pathsLength; i++) 
		{
			var path = paths[i];
			if 	(
				path.StartsWith("Assets/", System.StringComparison.Ordinal) && 
				!path.Contains("Editor") &&
				!path.Contains(".cs")
//                path.Contains("Resources") ||
//                path.Contains("StreamingAssets")
			)
			{
				sb.Append(AssetDatabase.AssetPathToGUID(path));
				sb.AppendLine(path);
			}
		}

    #if UNITY_IOS
//		DirectoryInfo directory = Directory.GetParent(filePath);
//		if(!directory.Exists)
//		{
//			Directory.CreateDirectory(directory.ToString());
//		}
    #endif
        
		using (StreamWriter outfile = new StreamWriter(filePath, false))
		{
			outfile.Write(sb.ToString());
		}
	}
   


	public static string GUIDToAssetPath(string guid)
	{
		return AssetDatabase.GUIDToAssetPath(guid);
	}

	#else

	const int GUID_LENGTH = 32;
	static Dictionary<string, string> guidMap = new Dictionary<string, string>();


	static GUIDMapper()
	{
		Parse();
	}


	static void Parse()
	{
		string filePath = Application.streamingAssetsPath + "/guids";

		bool isCorrect = false;

		#if UNITY_ANDROID

        filePath = "jar:file://" + Application.dataPath + "!/assets/guids";

		AndroidJavaClass fileManagerClass = new AndroidJavaClass("com.inventain.android.FileManager");
		string fileContext = fileManagerClass.CallStatic<string>("GetTextFromStreamingAssets", filePath);

		isCorrect = ((fileContext != null) && (fileContext != ""));
		
		#else 
		isCorrect = File.Exists(filePath);
	    #endif

	    if (isCorrect)
		{
			float time = Time.realtimeSinceStartup;

			#if UNITY_ANDROID
			string[] paths = JsonConvert.DeserializeObject<List<string>>(fileContext).ToArray();
			
			#else 
			string[] paths = File.ReadAllLines(filePath);
			#endif
			for (int i = 0, pathsLength = paths.Length; i < pathsLength; i++) 
			{
				var line = paths[i];
				string guid = line.Substring(0, GUID_LENGTH);
				string path = line.Substring(GUID_LENGTH, line.Length - GUID_LENGTH);
				guidMap.Add(guid, path);
			}

			time = (Time.realtimeSinceStartup - time) * 1000;
            Debug.Log("GUIDMapper.Parse by : " + time.ToString("f10") + " miliseconds for " + guidMap.Keys.Count);
		}
		else
		{
			Debug.LogError("can't find file : " + filePath);
		}
	}


	public static string GUIDToAssetPath(string guid)
	{
		string path;
		if(!guidMap.TryGetValue(guid, out path))
		{
			Debug.LogError("can't find path for Guid : " + guid);
			path = string.Empty;
		}
		return path;
	}
	#endif
}
