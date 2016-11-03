using System;
#if UNITY_IOS
using System.IO;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
//using UnityEditor.FacebookEditor;

public static class XCodePostProcess
{

	[PostProcessBuild]
	public static void OnPostProcessBuild( BuildTarget target, string path )
	{
		if (target == BuildTarget.iOS)
		{
			// Create a new project object from build target
			UnityEditor.XCodeEditor.XCProject project = new UnityEditor.XCodeEditor.XCProject( path );

			string projModPath = System.IO.Path.Combine(Application.dataPath, "XCodeEditor");
			// Find and run through all projmods files to patch the project
			var files = System.IO.Directory.GetFiles( projModPath, "*.projmods", SearchOption.AllDirectories );
			foreach( var file in files ) {
				project.ApplyMod( file );
			}
			
			// Finally save the xcode project
			project.Save();


			// Update 7plist for Facebook
//			UnityEditor.FacebookEditor.XCodePostProcess.UpdatePlist(path);
		}
	}
}
#endif