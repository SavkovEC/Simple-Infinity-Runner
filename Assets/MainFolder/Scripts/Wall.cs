using UnityEngine;
using System.Linq;
using System.Collections;
using System.Threading;

public class Wall : LevelObject
{
    #region Fields

    public static event System.Action<Wall> OnWallDestroy;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        Type = CommonFields.LevelObjectType.Wall;
        GetComponent<tk2dSprite>().color = Color.red;
        GetComponent<TweenPosition>().SetOnFinishedDelegate(EndReached);
    }


    void OnDisable()
    {
        if (gameObject.GetComponent<TweenColor>() != null)
        {
            gameObject.GetComponent<TweenColor>().enabled = false;
        } 
    }
        
    #endregion


    #region Public methods

    public void DestroyWall(bool isDestroyer)
    {
        if (isDestroyer)
        {
            if (OnWallDestroy != null)
            {
                OnWallDestroy(this);
            }
        }
        else
        {
            TweenColor tweenColor = GetComponent<TweenColor>();
            if(tweenColor == null)
            {
                tweenColor = gameObject.AddComponent<TweenColor>();
            }
            tweenColor.ignoreTimeScale = true;
            tweenColor.style = Style.PingPong;
            tweenColor.beginColor = Color.red;
            tweenColor.endColor = Color.yellow;
            tweenColor.duration = 0.3f;
            tweenColor.enabled = true;
        }
    }

    #endregion


    #region Private methods

    void EndReached(ITweener iTweener)
    {
        OnWallDestroy(this);
    }

    #endregion
}