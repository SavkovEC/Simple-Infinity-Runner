using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
using System.Collections;
using System.Reflection;

[InitializeOnLoad]
public class BundlePrefixChecker 
{
	private const string PATH_BUNDLE_PREFIX_RESOURCES = "BundlePrefixResources/BundlePrefixResources.cs";
	private const string ASSET_FOLDER = "Assets";
	private const string CLASS_NAME = "BundlePrefixResources";
	private const string METHOD_NAME = "AppIdentifier";
	private const string ASSEMBLY_NAME = "Assembly-CSharp";

	static BundlePrefixChecker()
	{
		VerifyBundle();
	}

    [MenuItem("Inventain/BundlePrefix/VerifyBundle")]
	public static void VerifyBundle()
	{
		if (!Application.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
		{
			string appIdentifierSettings = PlayerSettings.bundleIdentifier;
			
			bool rewrite = false;
			
			if (!string.IsNullOrEmpty(appIdentifierSettings))
			{
				Assembly assembly = Assembly.Load(ASSEMBLY_NAME);

				if (assembly != null)
				{
					Type classBundlePrefixResources = assembly.GetType(CLASS_NAME);

					if (classBundlePrefixResources != null)
					{
						MethodInfo methodIdentifier = classBundlePrefixResources.BaseType.GetMethod(METHOD_NAME);
						
						if (methodIdentifier != null)
						{
							string result = (string)methodIdentifier.Invoke(null, null);
							
							if (!appIdentifierSettings.Equals(result))
							{
								Debug.Log("BundlePrefixChecker: Identifiers not equals");


								rewrite = true;
							}
						}
						else
						{
							Debug.LogWarning("BundlePrefixChecker: Method not found!");
						}
					}
					else
					{
						Debug.Log("BundlePrefixChecker: BundlePrefixResources not found");

						rewrite = true;
					}
				}
				else
				{
					Debug.LogWarning("BundlePrefixChecker: Assembly not found!");
				}
			}
			
			if (rewrite)
			{
				string pathToFile = System.IO.Path.Combine(Application.dataPath, PATH_BUNDLE_PREFIX_RESOURCES);
				
				Debug.Log("BundlePrefixChecker: Generate new Bundle prefix resources = " + pathToFile);
				
				bool success = false;

				CheckOrCreateDirectory(pathToFile.RemoveLastPathComponent());


				using (StreamWriter newBundle = new StreamWriter(pathToFile, false))
				{
					try 
					{
						newBundle.WriteLine(BundlePrefixResourcesGenerateCode(appIdentifierSettings));
						success = true;
					} 
					catch (System.Exception ex) 
					{
						string msg = " \n" + ex.ToString ();
						Debug.LogError (msg);
					}
				}
				
				if (success)
				{
					string assetPath = System.IO.Path.Combine(ASSET_FOLDER, PATH_BUNDLE_PREFIX_RESOURCES);
					AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
				}
			}
		}
	}

	private static void CheckOrCreateDirectory(string dir) 
	{
		if (File.Exists (dir)) 
		{
			Debug.LogWarning (dir + " is a file instead of a directory !");
			return;
		}
		else if (!Directory.Exists (dir)) 
		{
			try 
			{
				Directory.CreateDirectory (dir);
			}
			catch (System.Exception ex) 
			{
				Debug.LogWarning (ex.Message);
				throw ex;
			}
		}
	}
	
	private static string BundlePrefixResourcesGenerateCode(string identifier)
	{
        string[] identifierParts = identifier.Split('.');

        string companyID = "";
        string appName = "";
        bool isHD = false;;

        for (int i = 0; i < identifierParts.Length; i++)
        {
            if (i > (identifierParts.Length - 2))
            {
                appName += identifierParts[i];
            }
            else if (i == (identifierParts.Length - 2))
            {
                if (identifierParts[i + 1].Equals("hd"))
                {
                    appName += identifierParts[i]+ ".";
                    isHD = true;
                }
                else
                {
                    companyID += identifierParts[i] + ".";
                }
            }
            else
            {
                companyID += identifierParts[i] + ".";
            }
        }		

        if (companyID.EndsWith("."))
        {
            companyID = companyID.Remove(companyID.Length - 1);
        }

		string code = "";

		code += Line (0, "//WARNING: Auto generated file", 2);

		code += Line (0, "using System;", 2);
		code += Line (0, "public class BundlePrefixResources : BundlePrefixAbstract<BundlePrefixResources>");
		code += Line (0, "{");

		code += Line (1, "internal override string AppID { get { return \"" + appName + "\"; } }");
		code += Line (1, "internal override string CompanyID { get { return \"" + companyID + "\"; } }");
        code += Line (1, "internal override bool isHDVersion { get { return " + (isHD ? "true" : "false") + "; } }");

		code += Line (0, "}");

		return code;
	}

	private static string Line(int tabs, string code, int noOfReturns = 1) 
	{
		string indent = "";
		for (int i = 0; i < tabs; i++) 
		{
			indent += "\t";
		}

		string CRs = "";
		for (int i = 0; i < noOfReturns; i++) 
		{
			CRs += "\n";
		}

		return indent + code + CRs;
	}
}