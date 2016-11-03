using UnityEngine;
using System.Collections;

public static class PathUtils 
{
	#region Constants

	public const string TableExtention = "csv";
	public const string PNGExtention = "png";
	public const string CAFExtention = "caf";

	#endregion



	#region Root Folders

	public static string GetStreamingAssetsPath()
	{
		return Application.streamingAssetsPath;
	}


	public static string GetDataRoot()
	{
		return 
		#if UNITY_EDITOR
			GetStreamingAssetsPath().RemoveLastPathComponent();
		#elif UNITY_ANDROID
            GetStreamingAssetsPath();
		#elif UNITY_IOS
			GetStreamingAssetsPath().RemoveLastPathComponent().RemoveLastPathComponent();
		#else 
			GetStreamingAssetsPath().RemoveLastPathComponent();
		#endif
	}

	#region	



	#region Streaming Assets

	public static string GetStreamingAssetsFilePath(string fileName, string extention, string directory = "")
	{
		string result = GetStreamingAssetsPath();

		if (directory != "")
		{
			result = result.AppendPathComponent(directory);
		}

		result = result.AppendPathComponent(fileName);

		if (extention != "")
		{
			result = result.AppendPathExtention(extention);
		}

		return result;
	}


	public static string GetStreamingAssetsTable(string fileName, string directory = "")
	{
		return GetStreamingAssetsFilePath(fileName, TableExtention, directory);
	}


	public static string GetStreamingAssetsPNG(string fileName, string directory = "")
	{
		return GetStreamingAssetsFilePath(fileName, PNGExtention, directory);
	}


	public static string GetStreamingAssetsCAF(string fileName, string directory = "")
	{
		return GetStreamingAssetsFilePath(fileName, CAFExtention, directory);
	}

	#endregion



	#endregion String Utils

	public static string AppendPathComponent(this string str, string pathComponent)
	{
		return string.Format("{0}/{1}", str, pathComponent);
	}


	public static string AppendPathExtention(this string str, string pathExtention)
	{
		return string.Format("{0}.{1}", str, pathExtention);
	}


	public static string RemoveLastPathComponent(this string str)
	{
		if (str.Length >= 2)
		{
			int idx = str.LastIndexOf('/', str.Length - 2);
			if (idx > 0)
			{
				return str.Remove(idx);
			}
		}

		return "";
	}


	public static string ShellString(this string str)
	{
		return "\"" + str + "\"";
	}

	#endregion
}
