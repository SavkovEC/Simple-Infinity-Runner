using System;

public struct CommonFields
{
    #region Fields

    public const string KEY_FOR_BESTSCORE = "BestScore";
    public enum LevelObjectType
    {
        Wall = 0,
        Reducer = 1,
        Destroyer = 2
    };
    public enum UIState
    {
        GameScreen = 0,
        MainMenuScreen = 1,
        GameOverScreen = 2
    };
    public enum GameManagerState
    {
        GameOver = 0,
        GameReady = 1,
        GamePaused = 2,
        GameRunning = 3
    };

    #endregion
}