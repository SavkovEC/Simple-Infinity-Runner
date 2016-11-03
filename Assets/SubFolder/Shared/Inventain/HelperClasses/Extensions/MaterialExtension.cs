using UnityEngine;
using System.Collections;

public static class MaterialExtension 
{
	public static readonly string LIGHTMAP_KEY = "_Lightmap";


	public static Texture GetLightmapTexture(this Material mat)
	{
		if(mat.HasProperty(LIGHTMAP_KEY))
		{
			return mat.GetTexture(LIGHTMAP_KEY);
		}

		return null;
	}


	public static void SetLightmapTexture(this Material mat, Texture texture)
	{
		if(mat.HasProperty(LIGHTMAP_KEY))
		{
			mat.SetTexture(LIGHTMAP_KEY, texture);
		}
	}
}
