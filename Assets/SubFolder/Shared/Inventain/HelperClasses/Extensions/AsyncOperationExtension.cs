using UnityEngine;
using System;
using System.Collections;


public static class AsyncOperationExtension 
{
	static MonoBehaviour behaviour = null;


	static void Initialize()
	{
		if(behaviour == null)
		{
			GameObject g = new GameObject();
			g.name = "AsyncOperationExtension_Couroutine";
            g.hideFlags = HideFlags.HideAndDontSave;

			behaviour = g.AddComponent<MonoBehaviour>();
		}
	}


	public static Coroutine StartCoroutine(this IEnumerator iterator)
	{
		Initialize();
		return behaviour.StartCoroutine(iterator);
	}


	public static void StopCoroutine(this IEnumerator iterator)
	{
		if(iterator != null && behaviour)
		{
			behaviour.StopCoroutine(iterator);
		}
	}


	public static Coroutine StartCoroutine(this AsyncOperation task, Action finished = null)
	{
		Initialize();
		return behaviour.StartCoroutine(RunTaskInner(task, finished));
	}


	public static void StopCoroutine(this Coroutine coroutine)
	{
		if(coroutine != null && behaviour)
		{
			behaviour.StopCoroutine(coroutine);
		}
	}


	static IEnumerator RunTaskInner(AsyncOperation task, Action finished = null) 
	{
		while (!task.isDone)
		{
			yield return null;
		}

		if(finished != null)
		{
			finished();
		}
	}
}