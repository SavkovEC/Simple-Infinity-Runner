using UnityEngine;
using System.Collections;

public class MenuScreen : MonoBehaviour 
{
    #region Fields

    [SerializeField] tk2dUIItem keepGoingButton;
    [SerializeField] tk2dUIItem tryAgainButton;


    UIManager uIManagerInstance;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        uIManagerInstance = UIManager.Instance;
        keepGoingButton.OnClick += KeepGoingButton_OnClick;
        tryAgainButton.OnClick += TryAgainButton_OnClick;
    }


    void OnDisable()
    {
        keepGoingButton.OnClick -= KeepGoingButton_OnClick;
        tryAgainButton.OnClick -= TryAgainButton_OnClick;
    }

    #endregion


    #region Events

    private void KeepGoingButton_OnClick()
    {
        uIManagerInstance.UIState(CommonFields.UIState.GameScreen);
    }

    private void TryAgainButton_OnClick()
    {
        uIManagerInstance.TryAgain();
    }

    #endregion
}
