using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class CustomDebug : SingletonMonoBehaviour<CustomDebug>
{
	#region Variables

	static bool isInitialized;
	static int logMask;


	const string FILE_NAME = "DebugGroups";
	const string MESSAGE_FORMAT = "[{1}]       {0}";

	#endregion


	#region Unity lifecycle
	
	protected override void Awake()
	{	
		base.Awake();
		
		if (!isInitialized)
		{
			isInitialized = true;
			logMask = (int)DebugGroup.All;

			#if DEBUG
			
			TryToReadFileFromStreamingAssets();
			
			#endif
		}
	}
	
	#endregion


	#region Public methods

	public static void Log(object message, DebugGroup group = DebugGroup.All)
	{
		#if DEBUG

		if ((logMask & (int)group) > 0)
		{
			Debug.LogFormat(MESSAGE_FORMAT, message, group);
		}

		#endif
	}


	public static void Log(object message, UnityEngine.Object context, DebugGroup group = DebugGroup.All)
	{
		#if DEBUG
		
		if ((logMask & (int)group) > 0)
		{
            Debug.LogFormat(context.ToString(), MESSAGE_FORMAT, message, group);
		}
		
		#endif
	}


	public static void LogWarning(object message)
	{
		#if DEBUG

		Debug.LogWarning(message);
		
		#endif
	}


	public static void LogWarning(object message, UnityEngine.Object context)
	{
		#if DEBUG

		Debug.LogWarning(message, context);
		
		#endif
	}


	public static void LogError(object message)
	{
		#if DEBUG

		Debug.LogError(message);
		
		#endif
	}
	
	
	public static void LogError(object message, UnityEngine.Object context)
	{
		#if DEBUG

		Debug.LogError(message, context);
		
		#endif
	}

	#endregion


	#region Private methods

	void TryToReadFileFromStreamingAssets()
	{
		string tableName = PathUtils.GetStreamingAssetsTable(FILE_NAME);
		if (File.Exists(tableName))
		{
            string text = File.ReadAllText(tableName);
            ReadText(text);
		}
	}
	
	
	void ReadText(string text)
	{
		logMask = 0;

		string[,] settings = CSVReader.SplitCsvGrid(text);
		
		for (int row = 0; row < settings.GetUpperBound(1); row++) 
		{	
			string key = settings[0, row];
			string value = settings[1, row];

			if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
			{
				if (int.Parse(value) > 0)
				{
					logMask |= (int)Enum.Parse(typeof(DebugGroup), key);
				}
			}
		}
	}
	
	
	[ButtonAttribute] public string CreateFileWithDebugGroupsBtn = "CreateFileWithDebugGroups";
	void CreateFileWithDebugGroups()
	{
		string[] names = Enum.GetNames(typeof(DebugGroup));
		for (int i = 0; i < names.Length; i++)
		{
			names[i] += ",1";
		}

		SaveFile(names, PathUtils.GetStreamingAssetsTable(FILE_NAME));
	}


	static void SaveFile(string[] names, string fileName)
	{
        using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
		{
			fs.Flush();
			
			using (StreamWriter sw = new StreamWriter(fs))
			{
				foreach(string row in names)
				{
					sw.WriteLine(row);
				}
			}
		}
	}

	#endregion
}