using UnityEngine;
using System.Collections;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    #region Fields

    [SerializeField] GameObject gameScreenPrefab;
    [SerializeField] GameObject gameOverScreenPrefab;
    [SerializeField] GameObject menuScreenPrefab;


    GameManager gameManagerInstance;
    GameObject gameScreen;
    GameObject gameOverScreen;
    GameObject menuScreen;
    bool isSwipeDown;

    #endregion


    #region Unity lifecycle

    void OnEnable() 
    {
        gameManagerInstance = GameManager.Instance;
        gameScreen = Instantiate(gameScreenPrefab);
        gameScreen.transform.parent = this.transform;
        gameOverScreen = Instantiate(gameOverScreenPrefab);
        gameOverScreen.transform.parent = this.transform;
        menuScreen = Instantiate(menuScreenPrefab);
        menuScreen.transform.parent = this.transform;
        UIMode(true, false, false);
        isSwipeDown = false;
	}


    void Update()
    {
        if (isSwipeDown)
        {
            gameManagerInstance.MovePlayer();
        }
    }
        
    #endregion


    #region Public methods

    public void UIState(CommonFields.UIState state)
    {
        switch (state)
        {
            case CommonFields.UIState.GameScreen:
                UIMode(true, false, false);
                gameManagerInstance.ResumeGame();
                break;
            case CommonFields.UIState.MainMenuScreen:
                if(gameManagerInstance.State != CommonFields.GameManagerState.GamePaused &&
                    gameManagerInstance.State != CommonFields.GameManagerState.GameOver)
                {
                    UIMode(false, true, false);
                    gameManagerInstance.PauseGame();
                }
                break;
            case CommonFields.UIState.GameOverScreen:
                UIMode(false, false, true);
                break;
        }
    }


    public void Swipe()
    {
        if(gameManagerInstance.State == CommonFields.GameManagerState.GameReady)
        {
            gameManagerInstance.LaunchGame();
        }
        if(gameManagerInstance.State == CommonFields.GameManagerState.GameRunning)
        {
            gameManagerInstance.MovePlayer();
            isSwipeDown = true;
        }
    }


    public void SwipeUp()
    {
        isSwipeDown = false;
    }


    public void TryAgain()
    {
        gameManagerInstance.ResetGame();
    }

    #endregion


    #region Private methods

    void UIMode(bool isGameScreen, bool isMainMenu, bool isGameOver)
    {
        gameScreen.SetActive(isGameScreen);
        menuScreen.SetActive(isMainMenu);
        gameOverScreen.SetActive(isGameOver);
    }

    #endregion
}