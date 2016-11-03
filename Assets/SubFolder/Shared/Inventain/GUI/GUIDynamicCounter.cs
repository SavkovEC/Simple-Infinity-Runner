using UnityEngine;
using System;

public class GUIDynamicCounter : MonoBehaviour
{
	#region Variables  

	public event Action OnCounterValueChanged;

	[SerializeField] tk2dTextMesh counterLabel;
    [SerializeField] float filterValue = 0.1f;

    float currentValue;
    float finalValue;

	public tk2dTextMesh CounterLabel
	{
		get
		{
			return counterLabel;
		}
	}

	#endregion

  
	#region Unity Lifecycle   

    void Update()
    {
        if (Mathf.Abs(finalValue - currentValue) > float.Epsilon)  
        {
            int oldValue = Mathf.FloorToInt(currentValue);

			currentValue += (filterValue * Mathf.Abs(finalValue - oldValue));
			currentValue = Mathf.Min(currentValue, finalValue);

            if (oldValue != Mathf.FloorToInt(currentValue))
            {
                counterLabel.text = currentValue.ToString("F0");

				if (OnCounterValueChanged != null)
				{
					OnCounterValueChanged();
				}
            }
        }
    }

	#endregion

 
	#region Public methods   

    public void SetValue(float v, bool immediately = false)
    {
		if (immediately)
        {
            currentValue = finalValue = v;
            counterLabel.text = currentValue.ToString("F0");
        }
        else
        {
            finalValue = v;
        }
    }

	#endregion

}