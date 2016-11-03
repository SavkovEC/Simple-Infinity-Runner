using UnityEngine;
using System.Collections;

public class Bonus : LevelObject
{
    #region Fields

    public static event System.Action<Bonus> OnBonusDestroy;

    [SerializeField] float chanceToCreateDestoyer;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        DetermineBonusType();
        GetComponent<TweenPosition>().SetOnFinishedDelegate(EndReached);
    }

    #endregion


    #region Public methods

    public void DestroyBonus()
    {
        if (OnBonusDestroy != null)
        {
            OnBonusDestroy(this);
        }
    }


    #endregion


    #region Private methods

    void DetermineBonusType()
    {
        if (Random.value < chanceToCreateDestoyer)
        {
            Type = CommonFields.LevelObjectType.Destroyer;
        }
        else
        {
            Type = CommonFields.LevelObjectType.Reducer;
        }
    }


    void EndReached(ITweener iTweener)
	{
        OnBonusDestroy(this);
    }

    #endregion
}