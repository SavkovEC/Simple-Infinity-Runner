using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CurvesData 
{
	
	public string id;
	public AnimationCurve curve;
	public Keyframe begin;
	public Keyframe end;
	
	public CurvesData() 
    {
		this.id = "<enter id>";
		this.begin = new Keyframe(0f, 0f, 0f, 1f);
		this.end = new Keyframe(1f, 1f, 1f, 0f);
		this.curve = new AnimationCurve(begin, end);
	}
	
	public CurvesData(string id, AnimationCurve newCurve) 
    {
		this.id = id;
		this.begin = newCurve.keys[0];
		this.end = newCurve.keys[newCurve.keys.Length - 1];
		this.curve = new AnimationCurve(newCurve.keys);
	}
}

public class CurvesForTweener : SingletonMonoBehaviour<CurvesForTweener> 
{
	
	[SerializeField]
	List<CurvesData> curves = new List<CurvesData>();
	
//	void Start() {
//		foreach (CurvesData curve in curves) {
//			float beginValue = curve.curve.keys[0].value;
//			float endValue = 10f / 12f;
//			int count = curve.curve.keys.Length;
//			Keyframe begin = new Keyframe(0f, beginValue, curve.curve.keys[0].inTangent, curve.curve.keys[0].outTangent);
//			Keyframe end = new Keyframe(1f, endValue, curve.curve.keys[count - 1].inTangent, curve.curve.keys[count - 1].outTangent);
//			AnimationCurve c = new AnimationCurve(begin, end);
//			for (int i = 1; i < (count - 2); i++) c.AddKey(curve.curve.keys[i]);
//			AddCurve((curve.id + " normalize"), c);
//		}
//	}
	
	bool IsCurveExists(string id) 
    {
		return curves.Exists(data => data.id.Equals(id));
	}
	
	CurvesData FindCurve(string id)
    {
		return curves.Find(data => data.id.Equals(id));
	}
	
	public void AddCurve(string id, AnimationCurve newCurve) 
    {
		while (IsCurveExists(id) || id.Equals("default")) 
        {
			id += "_new";
		}
		curves.Add(new CurvesData(id, newCurve));
	}
	
	public AnimationCurve GetCurve(string id) 
    {
		if (IsCurveExists(id)) 
        {
			return new AnimationCurve(FindCurve(id).curve.keys);
		}
		if (id.Equals("default")) 
        {
			return new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
		}
		return null;
	}
	
	public List<string> IdList 
    {
		get 
        {
			var list = new List<string>();
			list.Add("default");

			foreach (CurvesData curve in curves) 
            {
				list.Add(curve.id);
			}
			return list;
		}
	}
}
