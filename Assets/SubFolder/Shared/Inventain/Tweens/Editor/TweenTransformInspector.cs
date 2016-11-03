using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TweenTransform))]
public class TweenTransformInspector : TweenerInspector {
	
	protected override void CustomInspectorGUI() {
		var tTransform = (TweenTransform) tween;
				
		EditorGUILayout.BeginHorizontal();
        if (tTransform.IsBeginStateSet) 
        {
			GUI.contentColor = Color.green;
		}
		EditorTools.DrawLabel("Begin transform", true, GUILayout.Width(150f));
		GUI.contentColor = defaultContentColor;
		if (EditorTools.DrawButton("R", "Reset begin transform", (tTransform.beginTransform != null), 20f)) {
			EditorTools.RegisterUndo("Reset begin transform", tTransform);
			tTransform.beginTransform = null;
		}
		tTransform.beginTransform = (Transform) EditorGUILayout.ObjectField(tTransform.beginTransform, typeof(Transform), true);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
        if (tTransform.IsEndStateSet) 
        {
			GUI.contentColor = Color.green;
		}
		EditorTools.DrawLabel("End transform", true, GUILayout.Width(150f));
		GUI.contentColor = defaultContentColor;
		if (EditorTools.DrawButton("R", "Reset end transform", (tTransform.endTransform != null), 20f)) {
			EditorTools.RegisterUndo("Reset end transform", tTransform);
			tTransform.endTransform = null;
		}
		tTransform.endTransform = (Transform) EditorGUILayout.ObjectField(tTransform.endTransform, typeof(Transform), true);
		EditorGUILayout.EndHorizontal();
				
	}	
	
}
