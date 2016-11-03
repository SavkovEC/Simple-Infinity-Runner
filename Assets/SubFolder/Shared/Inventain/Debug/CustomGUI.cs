using System;
using System.Collections.Generic;

public class CustomGUI : SingletonMonoBehaviour<CustomGUI> 
{
	#region Variables

	List<Action> drawCallbacks = new List<Action>();

	#endregion


	#region Unity lifecycle

	#if DEBUG
	
	void OnGUI()
	{
		foreach (Action callback in drawCallbacks)
		{
			callback();
		}
	}
	
	#endif

	#endregion


	#region Public methods

	public void AddCallback(Action callback)
	{
		#if DEBUG

		if (!drawCallbacks.Contains(callback))
		{
			drawCallbacks.Add(callback);
		}
		else
		{
			CustomDebug.Log("CustomGUI -> you try to add the action that is alredy added to a collection", DebugGroup.UI);
		}

		#endif
	}


	public void RemoveCallback(Action callback)
	{
		#if DEBUG

		if (drawCallbacks.Contains(callback))
		{
			drawCallbacks.Remove(callback);
		}
		else
		{
			CustomDebug.Log("CustomGUI -> you try to remove the action that doesn't add to a collection", DebugGroup.UI);
		}

		#endif
	}

	#endregion
}