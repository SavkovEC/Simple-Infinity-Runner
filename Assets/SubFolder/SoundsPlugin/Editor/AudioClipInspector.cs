using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;


[CanEditMultipleObjects, CustomEditor(typeof(AudioImporter))]
public class AudioClipInspector : Editor 
{
    static System.Type internalType = GetType("AudioImporterInspector");
    Editor baseEditor = null;

    static System.Type GetType(string typeName)
    {
        foreach(Assembly a in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            System.Type[] types = a.GetTypes();

            foreach(System.Type t in types)
            {
                if(t.Name.Equals(typeName))
                {
                    return t;
                }
            }
        }

        return null;
    }


    void OnEnable()
    {
        baseEditor = Editor.CreateEditor(targets, internalType);
    }


    void OnDisable()
    {
        DestroyImmediate(baseEditor);
    }


    int bitrate = 96;
    int channels = 0;

    public override void OnInspectorGUI() 
    {
        baseEditor.OnInspectorGUI();
       
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        bitrate = EditorGUILayout.IntPopup("Bitrate", bitrate, new string[] {"32", "48", "64", "80", "96", "112", "128"}, new int[]{32, 48, 64, 80, 96, 112, 128});
        channels = EditorGUILayout.IntPopup("Channels", channels, new string[] {"Auto", "mono", "stereo"}, new int[]{0,1,2});

        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("convert to caf"))
            {
                foreach(var audio in targets)
                {
                    string assetPath = AssetDatabase.GetAssetPath(audio);
                    string extension = System.IO.Path.GetExtension(assetPath);
                    string sourcePath = Application.dataPath + assetPath.Substring(6);
                    string destanationPath = sourcePath.Replace(extension, ".caf");

                    string ch = "";
                    if (channels > 0)
                    {
                        ch = " -c " + channels;
                    }

                    string attributes = string.Format("-f 'caff' -d aac -s 2 -b {0}000 {3} {1} {2}", bitrate, sourcePath, destanationPath, ch);

                    System.Diagnostics.Process p = System.Diagnostics.Process.Start("afconvert", attributes);
                    p.WaitForExit();
                }
            }
        }
        GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();
			
			if (GUILayout.Button("convert to ogg"))
			{
				foreach(var audio in targets)
				{
					string assetPath = AssetDatabase.GetAssetPath(audio);
					string extension = System.IO.Path.GetExtension(assetPath);
					string sourcePath = Application.dataPath + assetPath.Substring(6);
					string destanationPath = sourcePath.Replace(extension, ".ogg");

					string attributes = string.Format("-i {0} -acodec libvorbis {1}", sourcePath.ShellString(), destanationPath.ShellString());

					string processPath = Application.dataPath + "/SoundsPlugin/ffmpeg";
					System.Diagnostics.Process p = System.Diagnostics.Process.Start(processPath, attributes);
					p.WaitForExit();
				}
			}
		}
		GUILayout.EndHorizontal();

    }
}
