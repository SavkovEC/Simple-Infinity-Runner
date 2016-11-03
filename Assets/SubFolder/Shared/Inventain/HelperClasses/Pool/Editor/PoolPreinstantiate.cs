using UnityEngine;
using UnityEditor;
using System.Collections;
[CanEditMultipleObjects]
[CustomEditor(typeof(ObjectPool))]
public class PoolPreinstantiate : Editor {
	
		public override void OnInspectorGUI() {
			EditorGUIUtility.LookLikeControls(0, 0);
			base.OnInspectorGUI();


			GUILayout.BeginHorizontal("Box");
			{
				if(GUILayout.Button("PreInstantiate"))
				Preinstantiate();
			}
			GUILayout.EndHorizontal();
		}

	void Preinstantiate ()
	{
		foreach(ObjectPool p in targets)
		{
			for (int i = 0; i < p.preInstantiateCount; i++) {
				GameObject go = Instantiate(p.prefab) as GameObject;
				go.transform.parent = p.transform;
			}
		}
	}
}
