using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumAttribute))]
public class EnumAttributeDrawer : PropertyDrawer {
	
	EnumAttribute enumAttribute { get { return ((EnumAttribute)attribute); } }
	int index;


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (enumAttribute.List != null)
		{
			EditorGUI.BeginChangeCheck();

			index = -1;

			for (int i = 0; i < enumAttribute.List.Length; i++)
			{
				if (GetValue(property) == enumAttribute.List[i])
				{
					index = i;
				}
			}

			index = EditorGUI.Popup(position, label.text, index, enumAttribute.List);
			if (EditorGUI.EndChangeCheck() && index != -1)
			{
				SetValue(property, enumAttribute.List[index]);
			}
		}
	}


	public static string GetValue(SerializedProperty property)
	{
		if(property.type == "int")
		{
			return property.intValue.ToString();
		}
		else if(property.type == "float")
		{
			return property.floatValue.ToString();
		}
		return property.stringValue;
	}


	public static void SetValue(SerializedProperty property, string value)
	{
		if(property.type == "int")
		{
			property.intValue = int.Parse(value);
		}
		else if(property.type == "float")
		{
			property.floatValue = float.Parse(value);
		}
		else if(property.type == "string")
		{
			property.stringValue = value;
		}

	}
}
