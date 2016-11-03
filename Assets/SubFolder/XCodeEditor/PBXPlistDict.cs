using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections;

namespace UnityEditor.XCodeEditor
{
	public class PBXPlistDict : Dictionary<string,object>
	{
		private const string TYPE_PLIST = "plist";
		private const string TYPE_DICT = "dict";
		private const string TYPE_STRING = "string";
		private const string TYPE_INTEGER = "integer";
		private const string TYPE_REAL = "real";
		private const string TYPE_TRUE = "true";
		private const string TYPE_FALSE = "false";
		private const string TYPE_ARRAY = "array";
		private const string TYPE_NULL = "null";
		private const string TYPE_DATE = "date";
		private const string TYPE_DATA = "data";
		private const string TYPE_KEY = "key";

		private const string PUBLIC_ID = "-//Apple//DTD PLIST 1.0//EN";
		private const string STRING_ID = "http://www.apple.com/DTDs/PropertyList-1.0.dtd";

		public PBXPlistDict()
		{
		}
		
		public PBXPlistDict(string fullpath)
		{
			Load(fullpath);
		}

		public PBXPlistDict(XmlDocument doc)
		{
			Load(doc);
		}
		
		public void Load(string fullpath)
		{
			Clear();
			
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ProhibitDtd = false;
			XmlReader plistReader = XmlReader.Create(fullpath, settings);
			
			
			XDocument doc = XDocument.Load(plistReader);
			XElement plist = doc.Element(TYPE_PLIST);
			XElement dict = plist.Element(TYPE_DICT);
			
			var dictElements = dict.Elements();
			ParseDictForLoad(this, dictElements);

			plistReader.Close();
		}

		public void Load(XmlDocument doc)
		{
			Clear();

			XmlNodeReader nodeReader = new XmlNodeReader(doc);
			nodeReader.MoveToContent();
			
			XDocument dict = XDocument.Load(nodeReader);
			XElement plist = dict.Element(TYPE_PLIST);

			var plisElements = plist.Elements();
			ParseDictForLoad(this, plisElements);

			nodeReader.Close();
		}
		
		private void ParseDictForLoad(PBXPlistDict dict, IEnumerable<XElement> elements)
		{
			for (int i = 0; i < elements.Count(); i += 2)
			{
				XElement key = elements.ElementAt(i);
				XElement val = elements.ElementAt(i + 1);
				dict[key.Value] = ParseValueForLoad(val);
			}
		}
		
		private ArrayList ParseArrayForLoad(IEnumerable<XElement> elements)
		{
			var list = new ArrayList();
			foreach (XElement e in elements)
			{
				object one = ParseValueForLoad(e);
				list.Add(one);
			}
			return list;
		}
		
		private object ParseValueForLoad(XElement val)
		{
			switch (val.Name.ToString())
			{
			case TYPE_STRING:
				return val.Value;
			case TYPE_INTEGER:
				return int.Parse(val.Value);
			case TYPE_REAL:
				return float.Parse(val.Value);
			case TYPE_TRUE:
				return true;
			case TYPE_FALSE:
				return false;
			case TYPE_DICT:
				PBXPlistDict plist = new PBXPlistDict();
				ParseDictForLoad(plist, val.Elements());
				return plist;
			case TYPE_ARRAY:
				return ParseArrayForLoad(val.Elements());
			case TYPE_NULL:
				return null;
			case TYPE_DATE:
				return XmlConvert.ToDateTime(val.Value, XmlDateTimeSerializationMode.Utc);
			case TYPE_DATA:
				return Convert.FromBase64String(val.Value);
				
			default:
				CustomDebug.LogWarning("PBXPlistDict: Format unsupported, Parser update needed");
				return null;
			}
		}
		
		public void Save(string fileName)
		{
			string internalSubset = null;
			XDeclaration declaration = new XDeclaration("1.0", "UTF-8", null);
			XDocumentType docType = new XDocumentType("plist", PUBLIC_ID, STRING_ID, internalSubset);
			
			XmlWriterSettings settings = new XmlWriterSettings 
			{
				Encoding = new System.Text.UTF8Encoding(false),
				ConformanceLevel = ConformanceLevel.Document,
				Indent = true,
				IndentChars = "\t",
				NewLineChars = "\n",
				NewLineHandling = NewLineHandling.None
			};
			
			
			XElement plistNode = new XElement(TYPE_PLIST, ParseDictForSave(this));
			plistNode.SetAttributeValue("version", "1.0");
			XDocument file = new XDocument(declaration, docType);
			file.Add(plistNode);
			
			XmlWriter xmlwriter = XmlWriter.Create(fileName, settings);
			file.Save(xmlwriter);
			xmlwriter.Close();  
		}
		
		private XElement ParseDictForSave(PBXPlistDict dict)
		{
			XElement dictNode = new XElement(TYPE_DICT);
			foreach (string key in dict.Keys)
			{
				dictNode.Add(new XElement(TYPE_KEY, key));
				dictNode.Add(ParseValueForSave(dict[key]));
			}
			return dictNode;
		}
		
		public XElement ParseValueForSave(object node)
		{
			if ((node is string) || (node == null))
			{
				return new XElement(TYPE_STRING, node as string);
			}
			else if (node is int || node is long)
			{
				return new XElement(TYPE_INTEGER, ((int)node).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
			}
			else if (node is PBXPlistDict)
			{
				return ParseDictForSave((PBXPlistDict)node);
			}
			else if (node is ArrayList)
			{
				return ParseArrayForSave(node);
			}
			else if (node is byte[])
			{
				return new XElement(TYPE_DATA, Convert.ToBase64String((Byte[])node));
			}
			else if (node is float || node is double)
			{
				return new XElement(TYPE_REAL, ((double)node).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
			}
			else if (node is DateTime)
			{
				DateTime time = (DateTime)node;
				string stringTime = XmlConvert.ToString(time, XmlDateTimeSerializationMode.Utc);
				
				return new XElement(TYPE_DATE, stringTime);
			}
			else if (node is bool)
			{
				return new XElement(node.ToString().ToLower());
			}
			else
			{
				CustomDebug.LogWarning("PBXPlistDict: Format unsupported, Parser update needed");
			}
			
			return null;
		}
		
		private XElement ParseArrayForSave(object node)
		{
			XElement arrayNode = new XElement(TYPE_ARRAY);
			var array = (ArrayList)node;
			for (int i = 0; i < array.Count; i++)
			{
				arrayNode.Add(ParseValueForSave(array[i]));
			}
			return arrayNode;
		}
	}
}