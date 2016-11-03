using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GUILayouterType
{
    Horizontal,
    Vertical
}

public class GUILayouter : MonoBehaviour, ILayoutCellHandler 
{
    public static float DEFAULT_OFFSET
    {
        get 
        {
            int platformMultiplier = (tk2dSystem.IsRetina ? 2 : 1);

            return 20 * platformMultiplier;
        }
    }
     

	#region Variables

    [SerializeField] GUILayouterType type = GUILayouterType.Horizontal;
    public GUILayouterType Type
    {
        get
        {
            return type;
        }

		set 
		{
			type = value;
		}
    }
    GUILayoutCell[] cells;   

    [SerializeField] bool isRootLayouter = true;

    public bool IsRootLayouter
    {
        get
        {
            return isRootLayouter;
        }
        set
        {
            isRootLayouter = value;
        }
    }


    [SerializeField] bool isInversed;

    Rect? occupiedPixels = null;
     

    Transform cachedTransform;

    public Transform CachedTransform
    {
        get
        {
            if (cachedTransform == null)
            {
                cachedTransform = transform;
            }

            return transform;
        }
    }

    public Rect? OccupiedPixels
    {
        get
        {
            return occupiedPixels;
        }
        set
        {
            occupiedPixels = value;

            if (cells == null || cells.Length == 0)
            {
                cells = GetComponentsInChildren<GUILayoutCell>(true);

                List<GUILayoutCell> myCells = new List<GUILayoutCell>();

                foreach (var cell in cells)
                {
                    if (cell.transform.parent == CachedTransform)
                    {
                        myCells.Add(cell);
                    }
                }

                cells = myCells.ToArray();
            }

            UpdateLayout();
        }
    }




	#endregion

	#region Unity Lifecycle

    void Awake()
    {
		ResetLayouter ();
    }

    void Start()
    {
		Initialize();
    }

	#endregion

    #region ILayoutCellHandler implementation

    public void RepositionForCell(LayoutCellInfo info)
    {        
        if (info.type == GUILayouterType.Horizontal)
        {
            OccupiedPixels = info.cellRect;
        }
        else if (info.type == GUILayouterType.Vertical)
        {
            OccupiedPixels = info.cellRect;
        }

        if (info.anchor == AnchorType.Left)
        {
            CachedTransform.localPosition = new Vector3(-info.cellRect.width * 0.5f, 0, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Right)
        {
            CachedTransform.localPosition = new Vector3(info.cellRect.width * 0.5f, 0, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Top)
        {
            CachedTransform.localPosition = new Vector3(0, info.cellRect.height * 0.5f, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Bottom)
        {
            CachedTransform.localPosition = new Vector3(0, -info.cellRect.height * 0.5f, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Center)
        {
            CachedTransform.localPosition = new Vector3(0, 0, CachedTransform.localPosition.z);;
        }
    }

    #endregion

	#region Public methods

	public void ResetLayouter()
	{
		occupiedPixels = null;
		cells = null;
	}

	public void Initialize()
	{
		if (occupiedPixels == null)
		{
            OccupiedPixels = new Rect?(new Rect(0, 0, ScreenDimentions.Width, ScreenDimentions.Height));
		}
	}

    #if UNITY_EDITOR
    public void UpdateLayoutDebug(Vector3 debugScreenSize)
    {
        occupiedPixels = new Rect?(new Rect(0, 0, debugScreenSize.x, debugScreenSize.y));

        cells = GetComponentsInChildren<GUILayoutCell>(true);

        List<GUILayoutCell> myCells = new List<GUILayoutCell>();

        foreach (var cell in cells)
        {
            if (cell.transform.parent == CachedTransform)
            {
                myCells.Add(cell);
            }
        }

        cells = myCells.ToArray();

        UpdateLayout();
    }
    #endif

    public void UpdateLayout()
    {
        if(cells == null || cells.Length == 0 || cells[0] == null)
		{
			return;
		}

        int occupiedFixedSize = 0;
        float flexibleAreasTotalWeight = 0;

        bool hasFlexibleAreas = false;

        for (int i = 0; i < cells.Length; i++)
        {
            GUILayoutCell cell = cells[i];
            if (cell.Type == GUILayoutCellType.FixedSize)
            {
                occupiedFixedSize += (int)cell.SizeValue;
            }
            else if (cell.Type == GUILayoutCellType.RelativeFixedSize)
            {
                occupiedFixedSize += (int)(cell.SizeValue * (type == GUILayouterType.Horizontal ? occupiedPixels.GetValueOrDefault().width : occupiedPixels.GetValueOrDefault().height));
            }
            else if (cell.Type == GUILayoutCellType.Flexible)
            {
                hasFlexibleAreas = true;
                flexibleAreasTotalWeight += cell.SizeValue;
            }
        }

        int availiblePixels = (int)(type == GUILayouterType.Horizontal ? occupiedPixels.GetValueOrDefault().width : occupiedPixels.GetValueOrDefault().height);

        int unoccupiedArea = availiblePixels - occupiedFixedSize;

        if (unoccupiedArea < 0)
        {
//            CustomDebug.LogWarning(gameObject.name + ": Can't fit " + occupiedFixedSize + " pixels in " + occupiedPixels + " pixels");
        }

        if (unoccupiedArea > 0 && flexibleAreasTotalWeight < float.Epsilon && hasFlexibleAreas)
        {
            CustomDebug.LogError(gameObject.name + ": Weights for flexible cells is 0!");
        }


        //setting cells sizes
        int currentPosition = isInversed ? availiblePixels : 0;

        for (int i = 0; i < cells.Length; i++)
        {
            GUILayoutCell cell = cells[i];
            Rect cellRect = new Rect();
            if (type == GUILayouterType.Horizontal)
            {
                if (cell.Type == GUILayoutCellType.FixedSize)
                {
                    cellRect = new Rect(0, 0, cell.SizeValue, occupiedPixels.GetValueOrDefault().height);
                }
                else if (cell.Type == GUILayoutCellType.RelativeFixedSize)
                {
                    cellRect = new Rect(0, 0, cell.SizeValue * (type == GUILayouterType.Horizontal ? ScreenDimentions.Width : ScreenDimentions.Height), occupiedPixels.GetValueOrDefault().height);
                }
                else if (cell.Type == GUILayoutCellType.Flexible)
                {
                    cellRect = new Rect(0, 0, cell.SizeValue * (float)unoccupiedArea / (float)flexibleAreasTotalWeight, occupiedPixels.GetValueOrDefault().height);
                }
                if (isInversed)
                {
                    cellRect.center = new Vector3(currentPosition - cellRect.width * 0.5f, 0, 0);
                    currentPosition -= (int)cellRect.width;
                }
                else
                {
                    cellRect.center = new Vector3(currentPosition + cellRect.width * 0.5f, 0, 0);
                    currentPosition += (int)cellRect.width;
                }
            }
            else if (type == GUILayouterType.Vertical)
            {
                if (cell.Type == GUILayoutCellType.FixedSize)
                {
                    cellRect = new Rect(0, cell.SizeValue * 0.5f, occupiedPixels.GetValueOrDefault().width, cell.SizeValue);
                }
                else if (cell.Type == GUILayoutCellType.RelativeFixedSize)
                {
                    cellRect = new Rect(0, 0, occupiedPixels.GetValueOrDefault().width, cell.SizeValue * (type == GUILayouterType.Horizontal ? ScreenDimentions.Width : ScreenDimentions.Height));
                }
                else if (cell.Type == GUILayoutCellType.Flexible)
                {
                    cellRect = new Rect(0, 0, occupiedPixels.GetValueOrDefault().width, cell.SizeValue * (float)unoccupiedArea / (float)flexibleAreasTotalWeight);
                }
                if (isInversed)
                {
                    cellRect.center = new Vector3(0, currentPosition - cellRect.height * 0.5f, 0);
                    currentPosition -= (int)cellRect.height;
                }
                else
                {
                    cellRect.center = new Vector3(0, currentPosition + cellRect.height * 0.5f, 0);
                    currentPosition += (int)cellRect.height;
                }
            }
            cell.Reposition(type, cellRect);
        }
    }


	#endregion

}
