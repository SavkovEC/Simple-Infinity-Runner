using UnityEngine;
using System.Collections;
using System;

public abstract class BundlePrefixAbstract<T> where T : BundlePrefixAbstract<T>, new()
{
	static T instance = new T();

	internal abstract string AppID { get; }
	internal abstract string CompanyID { get; }
    internal abstract bool isHDVersion { get;}
	
	private const string IAP_SUFFIX = "iap.";
	private const string LEADERBOARD_SUFFIX = "gc.lb.";
	
	public static string BundleIDPrefix()
	{
		return (AppIdentifier() + ".");
	}
	
	public static string LeaderboardIDPrefix()
	{
		return ( BundleIDPrefix() + LEADERBOARD_SUFFIX );
	}
	
	public static string InAppPurchaseIDPrefix()
	{
		return ( BundleIDPrefix() + IAP_SUFFIX );
	}
	
	public static string AppIdentifier()
	{
		return ( instance.CompanyID + "." + instance.AppID);
	}
	
	public static string AppName()
	{
		return instance.AppID;
	}

    public static bool IsHDVersion()
    {
        return instance.isHDVersion;
    }
}
