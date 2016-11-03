using UnityEngine;
using System.Collections;

public class PropertyValueAttribute : PropertyAttribute 
{
    protected string propertyName;


    public string PropertyName
    {
        get
        {
            return propertyName;
        }
    }


    public PropertyValueAttribute(string property = null)
	{
		this.propertyName = property;
	}
}
