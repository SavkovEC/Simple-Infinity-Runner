
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TweenAmbientLight))]
public class TweenAmbientLightInspector : TweenerInspector
{
    protected override void CustomInspectorGUI()
    {
        TweenAmbientLight tAmbient = (TweenAmbientLight) tween;

        EditorGUILayout.LabelField("Begin Color");
        GUI.contentColor = defaultContentColor;
        EditorGUILayout.BeginHorizontal();

        EditorGUIUtility.LookLikeControls(15f, 0);
        tAmbient.beginColor = EditorGUILayout.ColorField(tAmbient.beginColor,GUILayout.Width(100f));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("End Color");
        GUI.contentColor = defaultContentColor;
        EditorGUILayout.BeginHorizontal();

        tAmbient.endColor = EditorGUILayout.ColorField(tAmbient.endColor,GUILayout.Width(100f));


        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("Tween target", true, GUILayout.Width(100f));

        EditorGUILayout.EndHorizontal();
    }

}

