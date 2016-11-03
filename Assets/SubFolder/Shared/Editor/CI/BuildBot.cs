using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
//using UnityEditor.FacebookEditor;


public static class LoaderImageInfoExtention
{
	public static string GetImageInfoFileName(this Dictionary<string, object> info)
	{
		return info["filename"] as string;
	}


	public static string GetImageInfoMinOS(this Dictionary<string, object> info)
	{
		object curObjVersion;
		if (!info.TryGetValue("minimum-system-version", out curObjVersion))
		{
			curObjVersion = "";
		}

		return curObjVersion as string;
	}
}



public class BuildBot : MonoBehaviour
{
	class BuildConfig
	{
		public string[] scenes;
		public string fullPath;
	}


	static BuildConfig lastConfig;



    public static void ModifyDefineSymbols(BuildTargetGroup target, bool addFlag, params string[] pDefines)
	{
		string defineString = PlayerSettings.GetScriptingDefineSymbolsForGroup (target);

		string[] currentDefines = defineString.Split (';');
		HashSet<string> defineSet = new HashSet<string> (currentDefines);

		foreach (var define in pDefines) 
		{
            if (addFlag)
            {
                defineSet.Add(define);
            }
            else
            {
                defineSet.Remove(define);
            }
		}

        StringBuilder sb = new StringBuilder();
        foreach (var define in defineSet)
        {
            sb.Append(define);
            sb.Append(';');
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup (target, sb.ToString());
	}


	static void ActualPerformBuild(bool useDebug)
	{
		print("BuildBot Started");
		BuildTargetGroup target = GetBuildTargetGroup ();
		EditorUserBuildSettings.selectedBuildTargetGroup = target;
        ModifyDefineSymbols(target, useDebug, "DEBUG");
		
		string[] scenes = FindEnabledEditorScenes ();
		
		string targetDir = "IOSBuild";
		string fullPath = Application.dataPath.Replace("Assets", targetDir);
		
		System.IO.Directory.CreateDirectory(fullPath);
		CustomDebug.Log("Creating path : " + fullPath);

#if !UNITY_5
		EditorUserBuildSettings.appendProject = false;
#endif

		print("ABT:" + EditorUserBuildSettings.activeBuildTarget);
		
		
		BuildConfig config = new BuildConfig();
		config.scenes = scenes;
		config.fullPath = fullPath;
		
		if (EditorUserBuildSettings.activeBuildTarget != GetBuildTarget())
		{
			lastConfig = config;
			
			CustomDebug.Log("Switching BuildTarget");
			EditorUserBuildSettings.activeBuildTargetChanged += startBuildingPlayer;
			EditorUserBuildSettings.SwitchActiveBuildTarget (GetBuildTarget());			
		}
		else
		{
			Build(config);
		}
	}


	[MenuItem ("File/CI/Console build")]		
	static void PerformConsoleBuild ()
	{
		string cmd = EditorApplication.applicationPath + "/Contents/MacOS/Unity";
		System.Diagnostics.Process.Start(cmd, "-quit -batchmode -executeMethod BuildBot.PerformIOSBuild -quit");
		EditorApplication.Exit(0);
	}


	[MenuItem ("File/CI/Build IOS Player")]		
	static void PerformIOSBuild ()
	{
		ActualPerformBuild(false);
	}


	[MenuItem("File/CI/Build IOS Player Profiled")]
	static void PerformIOSBuildDebug ()
	{
		CustomDebug.Log("Enabling Debug");
		ActualPerformBuild(true);
	}

    static void PerformIOSBuildWithParameters()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        List<string> buildParameters = new List<string>();

        for (int i = 0; i < args.Length; i++)
        {
            var a = args[i];
            if (a.Equals("-executeMethod"))
            {
                for (int j = i + 1; j < args.Length; j++) 
                {
                    if (args[j].Contains("-"))
                    {
                        break;
                    }    

                    buildParameters.Add(args[j]);
                }
            }
        }

        bool needDebug = false;
        iOSTargetDevice targetDevice = iOSTargetDevice.iPhoneAndiPad;

        foreach (var p in buildParameters)
        {
            if (p.Equals("Debug"))
            {
                needDebug = true;
            }
            else if (p.Equals("NoDebug"))
            {
                needDebug = false;
            }
            else if (p.Equals("Universal"))
            {
                targetDevice = iOSTargetDevice.iPhoneAndiPad;
            }
            else if (p.Equals("iPhone"))
            {
                targetDevice = iOSTargetDevice.iPhoneOnly;
            }
            else if (p.Equals("iPad"))
            {
                targetDevice = iOSTargetDevice.iPadOnly;
            }

            PlayerSettings.iOS.targetDevice = targetDevice;

            PlayerSettings.bundleIdentifier = PlayerSettings.bundleIdentifier.Replace(".hd", "");

            if (targetDevice == iOSTargetDevice.iPadOnly)
            {
                PlayerSettings.bundleIdentifier = PlayerSettings.bundleIdentifier + ".hd";
            }
        }

        BundlePrefixChecker.VerifyBundle();

        PluginIDCreator.Instance.isDebug = needDebug;
        PluginIDCreator.Instance.isIPadOnly = targetDevice == iOSTargetDevice.iPadOnly;
        PluginIDCreator.Instance.Generate();

        ActualPerformBuild(needDebug);
    }


	[MenuItem("File/CI/Build IOS Player with preaction")]
	static void PerformIOSBuildWithPreBuildAction()
	{
		PerformIOSBuildDebug();
	}


//	[MenuItem ("File/CI/Add Crittercism")]		
//	static void AddCrittercism ()
//	{
//		PluginImporter imp = AssetImporter.GetAtPath("Assets/Plugins/iOS/libCrittercismPlugin_v5_2_2.a") as PluginImporter;
//		imp.SetCompatibleWithPlatform(BuildTarget.iOS, true);
//
//		EditorApplication.OpenScene("Assets/Scenes/Splash.unity");
//		GameObject g = new GameObject("Crittercism");
//		g.AddComponent<CrittercismInit>();
//		EditorApplication.SaveScene("Assets/Scenes/Splash.unity");
//	}
	


	[UnityEditor.Callbacks.PostProcessBuild(6000)]
	public static void FixBuild(BuildTarget target, string path)
	{
        
        #if UNITY_STANDALONE_OSX || UNITY_ANDROID
		return;
		#endif

        const string fileName = "Info.plist";
        string fullPath = System.IO.Path.Combine(path, fileName);

        //Fabric(Crashlytics)
        string fabricAPIKey = null;
        string fabricSecret = null;

        string fabricKeysPath = System.IO.Path.Combine(Application.dataPath, "StreamingAssets/FabricKeys.csv");
        if(System.IO.File.Exists(fabricKeysPath))
        {
            FileInfo fabricFileInfo = new FileInfo(fabricKeysPath);
            StreamReader fabricReader = fabricFileInfo.OpenText();
            string[] fabricContents = fabricReader.ReadToEnd().Split(',');
            fabricReader.Close();

            if(fabricContents.Length == 2)
            {
                fabricAPIKey = fabricContents[0];
                fabricSecret = fabricContents[1];
            }
        }

//        if(!string.IsNullOrEmpty(fabricAPIKey) && !string.IsNullOrEmpty(fabricSecret))
//        {
//            // For Runner!!!! Fix if needed!!!
//            //const string fabricAPIKey = "e439e7ac0edea04d8d1dc270b31f57d4f3826253";
//            //const string fabricSecret = "dd61dcda946b8e8eac3b72397be2cc55131c6cf66010d33a139f95664fab41ee";
//
//            // Crashlytics info.plist
//            FBPListParser crashLytycsParser = new FBPListParser(fullPath);
//            crashLytycsParser.xmlDict.Remove("Fabric");
//
//            PListDict fabricDict = new PListDict();
//            fabricDict.Add("APIKey", fabricAPIKey);
//
//            var kitsArray = new List<object>();
//
//            PListDict kit = new PListDict();
//            kit.Add("KitName", "Crashlytics");
//            kit.Add("KitInfo", new PListDict());
//
//            kitsArray.Add(kit);
//            fabricDict.Add("Kits", kitsArray);
//
//            crashLytycsParser.xmlDict.Add("Fabric", fabricDict);
//            crashLytycsParser.WriteToFile();
//
//
//            // Crashlytics script
//            string projectFileFullPath = System.IO.Path.Combine(path, "Unity-iPhone.xcodeproj/project.pbxproj");
//
//
//            // Jenkins
//            if (path.Contains("Jenkins"))
//            {
//                FileInfo projectInfo = new FileInfo(projectFileFullPath);
//                StreamReader streamReader = projectInfo.OpenText();
//                string projectFileContent = streamReader.ReadToEnd();
//                streamReader.Close();
//
//                const string scriptReference = "380AC1661B3AB4780088D012";
//
//                if(!projectFileContent.Contains(scriptReference))
//                {
//                    const string buildActionMask = "buildActionMask";
//                    const string fabricRunPath = "./Frameworks/Plugins/iOS/GeneralPlugins/Crashlytics/Fabric.framework/run";
//
//                    // define action mask
//                    int indexBuildActionMask = projectFileContent.IndexOf(buildActionMask);
//                    string builmask = projectFileContent.Substring(indexBuildActionMask, buildActionMask.Length  + 12);
//                    if(!builmask.Contains(";"))
//                    {
//                        builmask = projectFileContent.Substring(indexBuildActionMask, buildActionMask.Length  + 14);
//                    }
//
//                    // add script reference to build phase list
//                    int indexStartBP = projectFileContent.IndexOf("buildPhases");
//                    int indexFinishBP = projectFileContent.IndexOf(")", indexStartBP);
//
//                    projectFileContent = projectFileContent.Insert(indexFinishBP, scriptReference);
//
//                    // add script object
//                    string scriptObject = scriptReference + " = {isa = PBXShellScriptBuildPhase;" + builmask + "files = ();inputPaths = ();outputPaths = ();runOnlyForDeploymentPostprocessing = 0;shellPath = /bin/sh;shellScript = \"" + fabricRunPath + " " + fabricAPIKey + " " + fabricSecret + "\";};";
//
//                    int indexStartSO = projectFileContent.IndexOf("PBXShellScriptBuildPhase");
//                    int indexFinishSO = projectFileContent.IndexOf("};", indexStartSO);
//                    projectFileContent = projectFileContent.Insert(indexFinishSO + 2, scriptObject);
//                }
//
//                StreamWriter streamWriter = new StreamWriter(projectFileFullPath);
//                streamWriter.Write(projectFileContent);
//                streamWriter.Close();
//            }
//        }               	
//
	}


	[MenuItem("File/CI/Post Build Actions", false, 10000)]
	public static void PostBuildActions() 
	{
		PostProcess(BuildTarget.iOS, "/Users/newmango/Documents/Projects/Runner_projects/BuildLaunchImage/");
	}


	static void PostProcess(BuildTarget target, string pathToBuiltProject)
	{
        #if UNITY_IOS
//		XCodePostProcess.OnPostProcessBuild(target, pathToBuiltProject); // 100 

//		FixBuild(target, pathToBuiltProject);
        #endif
	}


	static void Build(BuildConfig config)
	{
		tmMenu.RebuildIndex();
		tmMenu.StaticPrebuild(); 

		print("Start building player!");
		EditorUserBuildSettings.symlinkLibraries = false;
		EditorUserBuildSettings.allowDebugging = false;
		EditorUserBuildSettings.development = false;
		EditorUserBuildSettings.connectProfiler = false;

		BuildOptions options = new BuildOptions();
		string res = BuildPipeline.BuildPlayer (config.scenes, config.fullPath, GetBuildTarget(), options);

		if (res.Length > 0) 
		{
			throw new System.Exception ("BuildPlayer failed:" + res);
		}	
	}


	static void Build()
	{
		if (lastConfig != null)
		{
			Build(lastConfig);
			lastConfig = null;
		}
	}


	static string[] FindEnabledEditorScenes ()
	{
		var EditorScenes = new List<string> ();
		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if (!scene.enabled)
				continue;
			EditorScenes.Add (scene.path);
		}
		return EditorScenes.ToArray ();
	}


	static void SwitchPlatform()
	{
		CustomDebug.Log("Switching BuildTarget");
		EditorUserBuildSettings.activeBuildTargetChanged += startBuildingPlayer;
		EditorUserBuildSettings.SwitchActiveBuildTarget (GetBuildTarget());	
	}


	static void startBuildingPlayer ()
	{
		EditorUserBuildSettings.activeBuildTargetChanged-=startBuildingPlayer;	
		Build();
	}

	static BuildTargetGroup GetBuildTargetGroup()
	{
#if UNITY_5
		return BuildTargetGroup.iOS;
#else
		return BuildTargetGroup.iPhone;
#endif
	}


	static BuildTarget GetBuildTarget()
	{
#if UNITY_5
		return BuildTarget.iOS;
#else
		return BuildTarget.iPhone;
#endif
	}
}
