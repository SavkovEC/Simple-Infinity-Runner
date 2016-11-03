using UnityEngine;
using System.Collections;

public class GameOverScreen : MonoBehaviour 
{
    #region Fields

    [SerializeField] tk2dUIItem tryAgain;
    [SerializeField] tk2dTextMesh scoreTextCell;
    [SerializeField] tk2dTextMesh scoreValueCell;
    [SerializeField] tk2dTextMesh bestScoreTextCell;
    [SerializeField] tk2dTextMesh bestScoreValuCell;


    UIManager uIManagerInstance;
    GameManager gameManagerInstance;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        uIManagerInstance = UIManager.Instance;
        gameManagerInstance = GameManager.Instance;
        tryAgain.OnClick += TryAgainButton_OnClick;

        int score = (int)gameManagerInstance.GameScore;
        int bestScore = gameManagerInstance.BestScore;
        if (score < bestScore)
        {
            scoreTextCell.text = "Score:";
            scoreValueCell.text = score.ToString();
            bestScoreTextCell.text = "Best Score:";
            bestScoreValuCell.text = bestScore.ToString();
        }
        else
        {
            scoreTextCell.text = "YOU SET";
            scoreValueCell.text = "NEW";
            bestScoreTextCell.text = "BEST SCORE!!!";
            bestScoreValuCell.text = score.ToString();
        }
	}


    void OnDisable()
    {
        tryAgain.OnClick -= TryAgainButton_OnClick;
    }

    #endregion


    #region Events

    private void TryAgainButton_OnClick()
    {
        uIManagerInstance.TryAgain();
    }

    #endregion
}
