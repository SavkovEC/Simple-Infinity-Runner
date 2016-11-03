using UnityEngine;
using System.Collections;

public class GameScreen : MonoBehaviour 
{
    #region Fields

    [SerializeField] tk2dUIItem mainMenu;
    [SerializeField] tk2dUIItem swipe;
    [SerializeField] tk2dTextMesh swipeText;
    [SerializeField] tk2dTextMesh scoreValue;
    [SerializeField] tk2dSlicedSprite scoreSprite;
    [SerializeField] Color bestScoreColor;


    Color defaultScoreSpriteColor;
    UIManager uIManagerInstance;
    GameManager gameManagerInstance;
    bool isPushedOnce;
    bool isBestScoreSet;
    #endregion


    #region Unity lifecycle


    void OnEnable()
    {
        uIManagerInstance = UIManager.Instance;
        gameManagerInstance = GameManager.Instance;
        mainMenu.OnClick += MainMenuButton_OnClick;
        swipe.OnDown += Swipe_OnDown;
        swipe.OnUp += Swipe_OnUp;
        gameManagerInstance.UpdateScore += GameScreen_OnUpdateScore;
        gameManagerInstance.Reset += GameScreen_OnReset;
    }


    void Start()
    {
        defaultScoreSpriteColor = scoreSprite.color;
        isPushedOnce = false;
        scoreValue.text = "0";
        isBestScoreSet = false;
    }


    void OnDisable()
    {
        mainMenu.OnClick -= MainMenuButton_OnClick;
        swipe.OnDown -= Swipe_OnDown;
        swipe.OnUp -= Swipe_OnUp;
        gameManagerInstance.UpdateScore -= GameScreen_OnUpdateScore;
        gameManagerInstance.Reset -= GameScreen_OnReset;
    }

    #endregion


    #region Events

    void MainMenuButton_OnClick()
    {
        uIManagerInstance.UIState(CommonFields.UIState.MainMenuScreen);
    }


    void Swipe_OnDown()
    {
        if (!isPushedOnce)
        {
            TweenColor.SetColor(swipeText.gameObject, new Color(0, 0, 0, 0), 2);
            isPushedOnce = true;
        }
        uIManagerInstance.Swipe();
    }


    void Swipe_OnUp()
    {
        uIManagerInstance.SwipeUp();
    }


    void GameScreen_OnUpdateScore(bool isNewBestScore)
    {
        scoreValue.text = ((int)GameManager.Instance.GameScore).ToString();
        if (isNewBestScore)
        {
            if (!isBestScoreSet)
            {
                TweenColor.SetColor(scoreSprite.gameObject, bestScoreColor, 0.5f);
                isBestScoreSet = true;
            }
        }
    }

    void GameScreen_OnReset()
    {
        scoreSprite.color = defaultScoreSpriteColor;
        isPushedOnce = false;
        scoreValue.text = "0";
        isBestScoreSet = false;
    }

    #endregion
}