using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    #region Fields

    public event System.Action<Player> OnPlayerDeath;


    [SerializeField] float bonusDuration;
    [SerializeField] float reducePower;
    [SerializeField] float yStartPositionFromTheBottom;
    [SerializeField] tk2dSprite playerSprite;


    GameManager gameManagerInstance;
    bool isReduced;
    bool isDestroyer;
    float bonusTime;
    float playerYPosition;
    Vector3 playerScale;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        gameManagerInstance = GameManager.Instance;
        gameManagerInstance.Reset += Player_OnReset;
    }


    void Start()
    {
        playerYPosition = -ScreenDimentions.Height / 2 + yStartPositionFromTheBottom;
        transform.position = new Vector3(0f, playerYPosition, -0.5f);
        playerScale = transform.localScale;
        bonusTime = 0;
    }


	void Update() 
	{
        if (isDestroyer)
        {
            if(bonusTime <= 0)
            {
                isDestroyer = false;
                GetComponent<TweenColor>().enabled = false;
                GetComponent<tk2dSprite>().color = Color.blue;
            }
            bonusTime -= Time.deltaTime;
        }

		if (isReduced) 
		{
            if (bonusTime <= 0)
			{
				isReduced = false;
                Vector3 playerSize = new Vector3(playerScale.x, playerScale.y, playerScale.z);
                playerSprite.transform.localScale = playerSize;
			}
            bonusTime -= Time.deltaTime;
		}
	}


    void OnTriggerEnter(Collider collider)
    {
        switch(collider.gameObject.GetComponent<LevelObject>().Type)
        {
            case CommonFields.LevelObjectType.Wall:
                collider.gameObject.GetComponent<Wall>().DestroyWall(isDestroyer);
                if(!isDestroyer)
                {
                    OnPlayerDeath(this);
                }
                break;
            case CommonFields.LevelObjectType.Destroyer:
                collider.gameObject.GetComponent<Bonus>().DestroyBonus();
                isDestroyer = true;
                Flick();
                bonusTime = bonusDuration;
                break;
            case CommonFields.LevelObjectType.Reducer:
                collider.gameObject.GetComponent<Bonus>().DestroyBonus();
                isReduced = true;
                Vector3 playerSize = new Vector3 (playerScale.x / reducePower, playerScale.y / reducePower, playerScale.z / reducePower);
                playerSprite.transform.localScale = playerSize;
                bonusTime = bonusDuration;
                break;
        }
    }


    void OnDisable()
    {
        gameManagerInstance.Reset -= Player_OnReset;
    }

    #endregion


    #region Events

    void Player_OnReset()
    {
        if (bonusTime != 0)
        {
            GetComponent<TweenColor>().enabled = false;
            GetComponent<tk2dSprite>().color = Color.blue;
            Vector3 playerSize = new Vector3(playerScale.x, playerScale.y, playerScale.z / reducePower);
            playerSprite.transform.localScale = playerSize;
        }
        Start();
    }

    #endregion


    #region Public methods

    public void MoveWithMouse()
    {
        float newX = Input.mousePosition.x / Screen.width * ScreenDimentions.Width - ScreenDimentions.Width / 2;
        transform.position = new Vector3(newX, playerYPosition, gameObject.transform.position.z);
    }


    public void MoveWithTouch()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                float newX = touch.position.x / Screen.width * ScreenDimentions.Width - ScreenDimentions.Width / 2;
                transform.position = new Vector3(newX, playerYPosition, gameObject.transform.position.z);
            }
        }
    }

    #endregion


    #region Private methods

    void Flick()
    {
        TweenColor tweenColor = GetComponent<TweenColor>();
        tweenColor.beginColor = Color.blue;
        tweenColor.endColor = Color.magenta;
        tweenColor.duration = 0.02f;
        tweenColor.enabled = true;
    }

    #endregion
}