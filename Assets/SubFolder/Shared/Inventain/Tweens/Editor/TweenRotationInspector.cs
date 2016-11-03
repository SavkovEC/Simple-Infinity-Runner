using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TweenRotation))]
public class TweenRotationInspector : TweenerInspector
{
    protected override void CustomInspectorGUI()
    {
        TweenRotation tRotation = (TweenRotation) tween;

        EditorGUILayout.LabelField("Begin rotation");
        GUI.contentColor = defaultContentColor;
        EditorGUILayout.BeginHorizontal();

        EditorGUIUtility.LookLikeControls(15f, 0);
        tRotation.BeginRotation = EditorTools.DrawVector3(tRotation.BeginRotation);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("End rotation");
        GUI.contentColor = defaultContentColor;
        EditorGUILayout.BeginHorizontal();

        tRotation.EndRotation = EditorTools.DrawVector3(tRotation.EndRotation);


        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("Tween target", true, GUILayout.Width(100f));

        EditorGUILayout.EndHorizontal();
    }

}