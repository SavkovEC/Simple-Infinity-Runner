using UnityEngine;
using System.Collections;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region Fields

    public event System.Action<bool> UpdateScore;
    public event System.Action Reset;

    [SerializeField] LevelObjectsCreator levelObjectsCreator;
    [SerializeField] Player player;
    [SerializeField] UIManager uIManager;
    [SerializeField] bool shouldResetBestScore;
    [SerializeField] float scoreMultiplier;

    UIManager uIManagerInstance;
    Player playerInstance;
    LevelObjectsCreator levelObjectsCreatorInstance;
    float timer;
    int scoreInt;

    #endregion


    #region Properties

    public CommonFields.GameManagerState State
    {
        get;
        private set;
    }

    public float GameScore 
    { 
        get; 
        private set; 
    }

    public int BestScore
    {
        get;
        private set;
    }

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        uIManagerInstance = UIManager.Instance;
        levelObjectsCreatorInstance = Instantiate(levelObjectsCreator);
        playerInstance = Instantiate(player);
        playerInstance.OnPlayerDeath += GameManager_OnPlayerDeath;
    }

    void Start () 
    {
        PauseGame();
        State = CommonFields.GameManagerState.GameReady;
        timer = 0;
        scoreInt = 0;
        GameScore = 0;
        if (shouldResetBestScore)
        {
            BestScore = 0;
        }
        else
        {
            BestScore = PlayerPrefs.GetInt(CommonFields.KEY_FOR_BESTSCORE);
        }
	}


    void Update()
    {
        CalculateScore();
        CheckGameOver();
    }


    void OnDisable()
    {
        playerInstance.OnPlayerDeath -= GameManager_OnPlayerDeath;
    }

    #endregion


    #region Events

    public void GameManager_OnPlayerDeath(Player player)
    {
        PauseGame();
        State = CommonFields.GameManagerState.GameOver;
    }

    #endregion


    #region Public methods

    public void LaunchGame()
    {
        State = CommonFields.GameManagerState.GameRunning;
        ResumeGame();
    }


    public void PauseGame()
    {
        Time.timeScale = 0;
        State = CommonFields.GameManagerState.GamePaused;
    }


    public void ResumeGame()
    {
        Time.timeScale = 1;
        State = CommonFields.GameManagerState.GameRunning;
    }


    public void MovePlayer()
    {
        if (State == CommonFields.GameManagerState.GameRunning)
        {
            playerInstance.MoveWithMouse();
            playerInstance.MoveWithTouch();
        }
    }


    public void ResetGame()
    {
        uIManagerInstance.UIState(CommonFields.UIState.GameScreen);
        PauseGame();
        State = CommonFields.GameManagerState.GameReady;
        timer = 0;
        scoreInt = 0;
        GameScore = 0;
        Reset();
    }

    #endregion


    #region Private methods

    void CalculateScore()
    {
        GameScore += Time.deltaTime * scoreMultiplier;
        if ((int)(GameScore) > scoreInt)
        {
            scoreInt = (int)(GameScore);
            if (BestScore < scoreInt)
            {
                UpdateScore(true);
            }
            else
            {
                UpdateScore(false);
            }
        }
    }

    void CheckGameOver()
    {
        if(State == CommonFields.GameManagerState.GameOver)
        {
            timer += Time.unscaledDeltaTime;
            if (timer > 2)
            {
                uIManagerInstance.UIState(CommonFields.UIState.GameOverScreen);
                if ((int)GameScore > BestScore)
                {
                    BestScore = (int)GameScore;
                    State = CommonFields.GameManagerState.GameReady;
                    PlayerPrefs.SetInt(CommonFields.KEY_FOR_BESTSCORE, BestScore);
                    PlayerPrefs.Save();
                }
            }
        }
    }

    #endregion
}