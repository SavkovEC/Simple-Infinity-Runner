using UnityEngine;
using System.Collections;

public class GUICellColliderSetter : MonoBehaviour , ILayoutCellHandler
{
	#region Variables
	
	[SerializeField] GUILayoutCell rePositionPlace;
	[SerializeField] BoxCollider settedCollider;
	[SerializeField] float sizeMultiplier = 1;

	BoxCollider toReposition;
	BoxCollider ToReposition
	{
		get
		{
			if(toReposition == null)
			{
				if(settedCollider != null)
				{
					toReposition = settedCollider;
				}
				else
				{
					toReposition = GetComponent<BoxCollider>();
				}
			}
			return toReposition;
		}
	}

	#endregion

	#region ILayoutCellHandler implementation

	public void RepositionForCell (LayoutCellInfo info)
	{
		float y = (rePositionPlace.transform.position.y - transform.position.y) / (Vector3.one / ((float)ScreenDimentions.Height / (float)Screen.height)).y;

		ToReposition.center = Vector3.up * y;
		ToReposition.size = new Vector3 (rePositionPlace.RecievedRect.size.x * sizeMultiplier, rePositionPlace.RecievedRect.size.y * sizeMultiplier, 1);
	}

	#endregion


}
