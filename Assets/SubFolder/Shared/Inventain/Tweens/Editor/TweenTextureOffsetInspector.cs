using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TweenTextureOffset))]
public class TweenTextureOffsetInspector : TweenerInspector 
{
	
	protected override void CustomInspectorGUI() 
    {
		var tTextureOffset = (TweenTextureOffset) tween;
		
        if (tTextureOffset.IsBeginStateSet) 
        {
			GUI.contentColor = Color.green;
		}
		EditorGUILayout.LabelField("Begin offset");
		GUI.contentColor = defaultContentColor;
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("R", "Reset offset to zero", IsResetOffsetValid(tTextureOffset.beginOffset), 20f)) {
			EditorTools.RegisterUndo("Reset begin offset", tTextureOffset);
			tTextureOffset.beginOffset = Vector2.zero;
		}
		EditorGUIUtility.LookLikeControls(15f, 0);
		tTextureOffset.beginOffset = EditorTools.DrawVector2(tTextureOffset.beginOffset);
		EditorGUIUtility.LookLikeControls();
		if (EditorTools.DrawButton("S", "Set current offset", IsSetOffsetValid(tTextureOffset.beginOffset, tTextureOffset.CurrentOffset), 20f)) 
        {
			EditorTools.RegisterUndo("Set begin offset", tTextureOffset);
			tTextureOffset.beginOffset = tTextureOffset.CurrentOffset;
		}
		EditorGUILayout.EndHorizontal();
        if (tTextureOffset.IsEndStateSet) 
        {
			GUI.contentColor = Color.green;
		}
		EditorGUILayout.LabelField("End offset");
		GUI.contentColor = defaultContentColor;
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("R", "Reset offset to zero", IsResetOffsetValid(tTextureOffset.endOffset), 20f)) 
        {
			EditorTools.RegisterUndo("Reset end offset", tTextureOffset);
			tTextureOffset.endOffset = Vector2.zero;
		}
		EditorGUIUtility.LookLikeControls(15f, 0);
		tTextureOffset.endOffset = EditorTools.DrawVector2(tTextureOffset.endOffset);
		EditorGUIUtility.LookLikeControls();
		if (EditorTools.DrawButton("S", "Set current offset", IsSetOffsetValid(tTextureOffset.endOffset, tTextureOffset.CurrentOffset), 20f)) 
        {
			EditorTools.RegisterUndo("Set end offset", tTextureOffset);
			tTextureOffset.endOffset = tTextureOffset.CurrentOffset;
		}
		EditorGUILayout.EndHorizontal();		

	}

	bool IsResetOffsetValid(Vector2 v) {
		return (v.x != 0f) || (v.y != 0f);
	}
	
	bool IsSetOffsetValid(Vector2 v, Vector2 cv) {
		return (v.x != cv.x) || (v.y != cv.y);
	}

	

}
