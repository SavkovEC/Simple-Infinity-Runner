using UnityEngine;
using System.Collections.Generic;

public class LocalisationManager : SingletonMonoBehaviour<LocalisationManager> 
{
	#region Variables

    public bool IsDebugTextEnabled;

	[SerializeField] TextAsset keysFile;

	Dictionary<string, string> internal_allTexts = null;
	Dictionary<string, string> AllTexts
	{
		get
		{
			if (internal_allTexts == null)
			{
				internal_allTexts = new Dictionary<string, string>();

				string[,] loadedText = CSVReader.SplitCsvGrid(keysFile.text);
				for (int y = 0; y < loadedText.GetUpperBound(1); y++) 
				{	
					if(!string.IsNullOrEmpty(loadedText[0, y]))
					{
						if(internal_allTexts.ContainsKey(loadedText[0,y]))
						{
							CustomDebug.LogError("KEY ALLREADY EXISTS = " + loadedText[0,y]);
						}
						else
						{
							string value = loadedText[1,y];
							
							if (!string.IsNullOrEmpty(value))
							{
								value = value.Replace(Constants.LocalizationTags.LINE, "\n");
                                value = value.Replace(Constants.LocalizationTags.COMMA, ",");
								
								internal_allTexts.Add(loadedText[0,y], value);
							}
							else
							{
								Debug.LogWarning("Null ref in key : " + loadedText[0,y]);
							}
						}
					}
				}
			}

			return internal_allTexts;
		}
	}

	#endregion

	
	#region Public

	public string GetTextByKey(string key)
	{
		string result = key;

		if (AllTexts.ContainsKey(key))
		{
			if (IsDebugTextEnabled)
			{
				result = "???";
			}

			result = AllTexts[key];
		}

		return result;
	}

	public static string LocalizedStringOrSource(string source)
	{
		string result = source;

		if (InstanceIfExist)
		{
			string loadedText = Instance.GetTextByKey(result);
			if (loadedText != null)
			{
				result = loadedText;
			}
		}

		return result;
	}


	#endregion
}