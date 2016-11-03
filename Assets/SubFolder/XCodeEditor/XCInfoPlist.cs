using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.XCodeEditor
{
	public class XCInfoPlist
	{
		private static void UpdateArray(ArrayList list, ArrayList config)
		{
			foreach (var item in config)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}


		private static void UpdateDict(PBXPlistDict dict, PBXPlistDict config)
		{
			foreach (var pair in config)
			{
				if (!dict.ContainsKey(pair.Key))
				{
					dict.Add(pair.Key, pair.Value);
				}
				else
				{
					var dictVal = dict[pair.Key];
					var dictConfigVal = pair.Value;
					
					if (dictVal is ArrayList)
					{
						UpdateArray((ArrayList)dictVal, (ArrayList)dictConfigVal);
					}
					else if (dictVal is PBXPlistDict)
					{
						UpdateDict((PBXPlistDict)dictVal, (PBXPlistDict)dictConfigVal);
					}
					else
					{
						dictVal = dictConfigVal;
					}
				}
			}

		}
		
		public static void UpdatePlist(string path, string xmlNodeString)
		{
			const string fileName = "Info.plist";
			string fullPath = Path.Combine(path, fileName);

			PBXPlistDict dict = new PBXPlistDict(fullPath);

			XmlDocument config = new XmlDocument();
			config.LoadXml(xmlNodeString);

			PBXPlistDict dictConfig = new PBXPlistDict(config);

			UpdateDict(dict, dictConfig);

			dict.Save(fullPath);
		}
	}
}