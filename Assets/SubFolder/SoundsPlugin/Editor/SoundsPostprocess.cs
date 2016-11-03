using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using UnityEditor.Callbacks;


public static class SoundsPostprocess 
{

//	[PostProcessBuild]
	public static void UpdateSounds(BuildTarget target, string path)
	{
		if (target == BuildTarget.Android)
		{
			string filePathAssets = path + "/" + Application.productName + "/assets/"; 
			var allFiles = Directory.GetFiles(filePathAssets, "*.caf", SearchOption.AllDirectories);

			foreach (string file in allFiles)
			{
				string filePath = file;

				int fileExtPos = file.LastIndexOf(".");
				if (fileExtPos >= 0 )
				{
					filePath = file.Substring(0, fileExtPos);
				}

				string sourceFile = filePath + ".caf";
				string destanationFile = filePath + ".ogg";

				string processPath = Application.dataPath + "/SoundsPlugin/ffmpeg";

				string attributes = string.Format("-i {0} -acodec libvorbis {1}", sourceFile.ShellString(), destanationFile.ShellString());
				
				System.Diagnostics.Process p = System.Diagnostics.Process.Start(processPath, attributes);
				p.WaitForExit();
				
				System.IO.File.SetAttributes(sourceFile, FileAttributes.Normal);
				System.IO.File.Replace(destanationFile, sourceFile, null, true);
				System.IO.File.SetAttributes(sourceFile, FileAttributes.ReadOnly);
			}
		}
	}
}
