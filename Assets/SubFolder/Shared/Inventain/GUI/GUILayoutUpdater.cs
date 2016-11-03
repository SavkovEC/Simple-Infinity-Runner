using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(GUILayouter))]
public class GUILayoutUpdater : MonoBehaviour 
{
    #if UNITY_EDITOR
    GUILayouter targetLayouter;

    public GUILayouter TargetLayouter
    {
        get
        {
            if (targetLayouter == null)
            {
                targetLayouter = GetComponent<GUILayouter>();
            }

            return targetLayouter;
        }
    }    


    void Update()
    {
        if (!TargetLayouter.IsRootLayouter)
        {
            return;
        }

        float width;
        float height;
        float aspect;

        tk2dCamera.Editor__GetGameViewSize(out width, out height, out aspect);

        TargetLayouter.UpdateLayoutDebug(new Vector2(width, height));
        ResetLayouters();
    }

    void ResetLayouters()
    {
        if (TargetLayouter.IsRootLayouter)
        {
            GUILayouter[] childLayouters = GetComponentsInChildren<GUILayouter>();

            foreach (var l in childLayouters)
            {
                l.ResetLayouter();
            }

            TargetLayouter.ResetLayouter();
        }
    }

    #endif
}
