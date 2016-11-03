using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;
using System.Reflection;



public static class AttributeHelper
{
    const string USER_ASSETMBLY = "Library/ScriptAssemblies";


    public class AttributeItem
    {
        public Attribute attribute;
        public MethodInfo methodInfo;
    }


    public static List<AttributeHelper.AttributeItem> ExtractMethods<T>(BindingFlags flags, object[] signature)
    {
        List<AttributeItem> list = new List<AttributeItem>();
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach(var assembly in assemblies)
        {
            if (assembly.Location.Contains(USER_ASSETMBLY))
            {
                foreach (Type type in assembly.GetTypes())
                {
                    MethodInfo[] methods = type.GetMethods (flags);
                    for (int i = 0; i < methods.GetLength (0); i++) 
                    {
                        MethodInfo methodInfo = methods [i];

                        if (Attribute.IsDefined (methodInfo, typeof(T)) && IsValidSignature(methodInfo, signature)) 
                        {
                            object[] customAttributes = methodInfo.GetCustomAttributes (typeof(T), false);
                            if (customAttributes != null && customAttributes.Length > 0)
                            {
                                Attribute item = ((Attribute)customAttributes [0]);

                                AttributeItem info = new AttributeItem();
                                info.attribute = item;
                                info.methodInfo = methodInfo;

                                list.Add(info);
                            }
                        }
                    }
                }
            }
        }

        return list;
	}



    public static bool IsValidSignature(MethodInfo methodInfo, object[] signature)
    {
        ParameterInfo[] param = methodInfo.GetParameters();

        if (param.Length != signature.Length)
        {
            return false;
        }

        for (int i = 0; i < signature.Length; i++)
        {
            if (!param[i].ParameterType.IsAssignableFrom(signature[i].GetType()))
            {
                return false;
            }
        }

        return true;
    }


    public static void ExecutePreprocess(object[] parameters)
    {
        List<AttributeHelper.AttributeItem> invokeAttributes = ExtractMethods<PreProcessBuildAttribute>(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, parameters);
        invokeAttributes.Sort(
            (a, b) =>
            {
                return ((PreProcessBuildAttribute)a.attribute).callbackOrder.CompareTo(((PreProcessBuildAttribute)b.attribute).callbackOrder);
            }
        );

        foreach (var item in invokeAttributes)
        {
            item.methodInfo.Invoke(null, parameters);
        }
    }
}
