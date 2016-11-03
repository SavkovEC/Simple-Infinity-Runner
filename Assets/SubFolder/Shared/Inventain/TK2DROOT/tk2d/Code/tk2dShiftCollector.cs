using UnityEngine;
using System.Collections;

public static class TK2D_RoundShiftExtentions
{
	public static Vector2 FloorVector(this Vector2 vector)
	{
		Vector2 result;
		result.x = Mathf.Floor(vector.x);
		result.y = Mathf.Floor(vector.y);
		return result;
	}


	public static Vector3 FloorVector(this Vector3 vector)
	{
		Vector3 result;
		result.x = Mathf.Floor(vector.x);
		result.y = Mathf.Floor(vector.y);
		result.z = Mathf.Floor(vector.z);
		return result;
	}


	public static Vector3 RoundVector(this Vector3 vector)
	{
		Vector3 result;
		result.x = Mathf.Round(vector.x);
		result.y = Mathf.Round(vector.y);
		result.z = Mathf.Round(vector.z);
		return result;
	}


    public static Vector2 RoundShift(this Transform transform, bool useSelfShift = true)
	{
		Vector2 totalShift = new Vector2();
			
        for (Transform curTransform = useSelfShift ? transform : transform.parent; curTransform != null; curTransform = curTransform.parent)
		{
			Vector2 curShift = curTransform.localPosition - curTransform.localPosition.FloorVector();
			totalShift += curShift;
		}

		return totalShift;
	}	
}
