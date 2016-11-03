using UnityEngine;
using System;
using System.Collections.Generic;

public class EnumAttribute : PropertyAttribute {
	string[] _list;

	public Type ListSourceClass;
	public string MethodName;


	public string[] List
	{
		get
		{
			if(ListSourceClass != null)
				Update(ListSourceClass.GetMethod(MethodName).Invoke(null, null) as string[]);

			return _list;
		}
		set
		{
			_list = value;
		}
	}


	public EnumAttribute()
	{

	}


	public EnumAttribute(params object[] list)
	{
		if (list.Length > 0)
		{
			this.List = new string[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
				this.List[i] = list[i].ToString();
			}
		}
	}


	public EnumAttribute(string[] list)
	{
		if (list.Length > 0)
		{
			this.List = new string[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
				this.List[i] = list[i];
			}
		}
	}



	public void Update<T>(T[] list)
	{
		if (list.Length > 0)
		{
			this._list = new string[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
				this._list[i] = list[i].ToString();
			}
		}
	}
}
