using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


public class PreprocessBuild : UnityEditor.AssetModificationProcessor
{
    const string STREAMING_FOLDER = "/StreamingAssets/";
    const string TEMP_FOLDER = "/Temp/StreamingAssets/";
    static HashSet<string> PLATFORM_FOLDERS = new HashSet<string>(){"iOS", "Android"};

    static bool executeBuildProcess = false;


    #region Test
    [MenuItem("Assets/Preprocess")]
    public static void Test()
    {
        AttributeHelper.ExecutePreprocess(new object[]{EditorUserBuildSettings.activeBuildTarget});
        PreprocessStreamingAssets();
        ProstprocessStreamingAssets();
    }


    [PreProcessBuildAttribute(2)]
    public static void OnPreBuild(BuildTarget buildTarget)
    {
        Debug.Log("OnPreBuild : " + buildTarget);
    }
    #endregion


	public static void OnWillSaveAssets(string[] assets)
	{
        if (BuildPipeline.isBuildingPlayer && !executeBuildProcess)
		{
            executeBuildProcess = true;
            try
            {
                AttributeHelper.ExecutePreprocess(new object[]{EditorUserBuildSettings.activeBuildTarget}); //called before PreprocessStreamingAssets, fixed broken sounds references
                PreprocessStreamingAssets();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                executeBuildProcess = false;
            }

            AssetDatabase.Refresh();
		}
	}


    [PostProcessBuild(-1)]
    public static void OnPostprocessBuild (BuildTarget buildTarget, string path)
    {
        ProstprocessStreamingAssets();
    }


    static void PreprocessStreamingAssets()
    {
        BuildTarget target =  EditorUserBuildSettings.activeBuildTarget;
        string rootDirectory = Application.dataPath + STREAMING_FOLDER;
        string tempFolder = Path.GetDirectoryName(Application.dataPath) + TEMP_FOLDER;

        string currentFolder = string.Empty;
        if(target == BuildTarget.iOS)
        {
            currentFolder = "iOS";
        }
        else if(target == BuildTarget.Android)
        {
            currentFolder = "Android";
        }

        HashSet<string> duplicates = new HashSet<string>();
        foreach (var platformFolder in PLATFORM_FOLDERS) 
        {
            if(platformFolder != currentFolder)
            {
                string[] directories = Directory.GetDirectories(rootDirectory, platformFolder, SearchOption.AllDirectories);
                foreach(string directory in directories)
                {
                    int index = directory.IndexOf(platformFolder);
                    string rootFolder = directory.Substring(0, index + platformFolder.Length); //iOS/anyPath/iOS  -> iOS/
                    if(duplicates.Add(rootFolder)) //check circle references from folder, iOS/Folder/iOS e.g.
                    {
                        string relativePath = rootFolder.Substring(rootDirectory.Length);
                        string tempPath = tempFolder + relativePath;
                        string tempDirectory = Path.GetDirectoryName(tempPath);
                        Directory.CreateDirectory(tempDirectory);

                        Directory.Move(rootFolder, tempPath);
                    }
                }
            }
        }
    }


    static void ProstprocessStreamingAssets()
    {
        string rootDirectory = Application.dataPath + STREAMING_FOLDER;
        string tempFolder = Path.GetDirectoryName(Application.dataPath) + TEMP_FOLDER;

        if (Directory.Exists(tempFolder))
        {
            string[] directories = Directory.GetDirectories(tempFolder, "*", SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                string dir = Path.GetFileName(directory);
                if (PLATFORM_FOLDERS.Contains(dir))
                {
                    int index = directory.IndexOf(dir);
                    string rootFolder = directory.Substring(0, index + dir.Length); //iOS/anyPath/iOS  -> iOS/

                    if (Directory.Exists(rootFolder))
                    {
                        string relativePath = rootFolder.Substring(tempFolder.Length);
                        string originalPath = rootDirectory + relativePath;

                        Directory.Move(rootFolder, originalPath);
                    }
                }
            }

            AssetDatabase.Refresh();
            Directory.Delete(tempFolder, true);
        }
    }
}
