using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class LevelObjectsCreator : MonoBehaviour
{
    #region Fields

    const int ROWS_TOTAL_COUNT = 9;
    const int LEFT_BORDER = 0;
    const int THRESHOLD_TO_CREATE_DIRECT = 40;
    const int THRESHOLD_TO_CREATE_RIGHT = 70;
    const int MIN_PATH_WIDTH = 1;
    const float INITIAL_BOTTOM_LEVEL = 0f;


    [SerializeField] GameObject bonusPrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] int columnsTotalCount;
    [SerializeField] int maxBonusCount;
    [SerializeField] float levelObjectsSpeed;
    [SerializeField] float bonusSpawnChanseIncrease;


    GameManager gameManagerInstance;
    float totalChanseToCreateBonus;
    ObjectPool walls;
    ObjectPool bonuses;
    GameObject lastWall;
    Vector2 sizeOfWall;
    Vector2 wallScale;
    int[] rows = new int[ROWS_TOTAL_COUNT];
    int pathColumnCurrent;
    int currentRow = 0;
    float leftScreenBorder;
    float bottomEnd;
    int rightColumn;
    int[] path;
    List<GameObject> allWalls;
    List<GameObject> allBonuses;

    #endregion


    #region Properties

    public bool ShouldSpawnBonus
    {
        get 
        {
            if (Random.value <= totalChanseToCreateBonus)
            {
                totalChanseToCreateBonus = 0f;
                return true;
            }
            totalChanseToCreateBonus += bonusSpawnChanseIncrease;
            return false;
        }
    }

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        gameManagerInstance = GameManager.Instance;
        gameManagerInstance.Reset += LevelObjectsCreator_OnReset;
        Wall.OnWallDestroy += Wall_OnWallDestroy;
        Bonus.OnBonusDestroy += Bonus_OnBonusDestroy;
    }


    void Start()
    {
        allWalls = new List<GameObject>();
        allBonuses = new List<GameObject>();
        rightColumn = columnsTotalCount - 1;
        PoolManager poolManagerInstance = PoolManager.Instance;
        poolManagerInstance.PoolObjectsRoot.transform.parent = this.transform;
        poolManagerInstance.name = "PoolManager";
        poolManagerInstance.transform.parent = this.transform;
        walls = poolManagerInstance.PoolForObject(wallPrefab);
        walls.transform.parent = this.transform;
        bonuses = poolManagerInstance.PoolForObject(bonusPrefab);
        bonuses.transform.parent = this.transform;
        leftScreenBorder = -ScreenDimentions.Width / 2;
        bottomEnd = -ScreenDimentions.Height / 2;
        pathColumnCurrent = 3;
        path = new int[1];
        path[0] = pathColumnCurrent;
        DefineWallSizeParametrs();
        CreateStartWalls();
    }


    void OnDisable()
    {
        gameManagerInstance.Reset -= LevelObjectsCreator_OnReset;
        Wall.OnWallDestroy -= Wall_OnWallDestroy;
        Bonus.OnBonusDestroy -= Bonus_OnBonusDestroy;
    }

    #endregion


    #region Events

    void Wall_OnWallDestroy(Wall wall)
    {
        allWalls.Remove(wall.gameObject);
        walls.Push(wall.gameObject);
        rows[currentRow]--;
        if (rows[currentRow] == 0)
        {
            GenerateNewRow();
            CreateNewRow(false);
        }
    }


    void Bonus_OnBonusDestroy(Bonus bonus)
    {
        allBonuses.Remove(bonus.gameObject);
        bonuses.Push(bonus.gameObject);
    }


    void LevelObjectsCreator_OnReset()
    {
        for (int i = 0; i < allWalls.Count; i++)
        {
            walls.Push(allWalls[i]);
        }
        for (int i = 0; i < allBonuses.Count; i++)
        {
            bonuses.Push(allBonuses[i]);
        }
        allWalls.Clear();
        allBonuses.Clear();
        CreateStartWalls();
        pathColumnCurrent = 3;
        path = new int[1];
        path[0] = pathColumnCurrent;
    }

    #endregion
        

    #region Private methods

    void DefineWallSizeParametrs()
    {
        tk2dSprite wallSprite = wallPrefab.GetComponent<tk2dSprite>();
        sizeOfWall = new Vector2((float)(ScreenDimentions.Width) / columnsTotalCount, ScreenDimentions.Height / (ROWS_TOTAL_COUNT - 2));
        float wallScaleX = sizeOfWall.x / wallSprite.GetBounds().size.x;
        float wallScaleY = sizeOfWall.y / wallSprite.GetBounds().size.y;
        wallScale = new Vector2(wallScaleX, wallScaleY);
    }


    void CreateStartWalls()
    {
        lastWall = walls.Pop();
        lastWall.transform.position = new Vector2(leftScreenBorder + 0.5f * sizeOfWall.x, INITIAL_BOTTOM_LEVEL);
        lastWall.transform.localScale = new Vector3(wallScale.x, wallScale.y, 1);
        walls.Push(lastWall);
        for (int i = 0; i < ROWS_TOTAL_COUNT; i++)
        {
            for (int j = 0; j < columnsTotalCount; j++)
            {
                switch (i)
                {
                    case 0:
                    case 1:
                        path = new int[] { 1, 2, 3, 4, 5 };
                        break;
                    case 2:
                    case 3:
                        path = new int[] { 2, 3, 4 };
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        path = new int[] { 3 };
                        break;
                }
            }
            CreateNewRow(true);
        }
    }
        

    void CreateNewRow(bool isStartRow)
    {
        GameObject[] wallsToCreate = new GameObject[columnsTotalCount];
        rows[currentRow] = wallsToCreate.Length - path.Length;
        float delta = Time.deltaTime * levelObjectsSpeed;
        float bottomPosition = lastWall.transform.position.y + sizeOfWall.y - delta;
        float duration = (bottomPosition - bottomEnd + sizeOfWall.y) / levelObjectsSpeed;
        for (int i = 0; i < wallsToCreate.Length; i++)
        {
            if (!path.Contains(i))
            {
                wallsToCreate[i] = walls.Pop();
                wallsToCreate[i].SetActive(false);
                Vector2 newWallPosition = new Vector2(leftScreenBorder + (i + 0.5f) * sizeOfWall.x, bottomPosition);
                wallsToCreate[i].transform.SetParent(walls.transform);
                wallsToCreate[i].transform.position = newWallPosition;
                wallsToCreate[i].transform.localScale = new Vector3(wallScale.x, wallScale.y, 1);
                TweenPosition.SetPosition(wallsToCreate[i], new Vector3(wallsToCreate[i].transform.position.x, bottomEnd - sizeOfWall.y, 0), duration);
                wallsToCreate[i].SetActive(true);
                allWalls.Add(wallsToCreate[i]);
            }
        }

        if (!isStartRow && allBonuses.Count < maxBonusCount)
        {
            LocateBonus();
        }

        for (int i = 0; i < wallsToCreate.Length; i++)
        {
            if (!path.Contains(i))
            {
                lastWall = wallsToCreate[i];
                break;
            }
        }

        SwitchRow();
    }
       

    void LocateBonus()
    {
        if (ShouldSpawnBonus) 
        {
            GameObject bonus = bonuses.Pop();
            bonus.transform.parent = bonuses.transform;
            allBonuses.Add(bonus.gameObject);
            Vector2 spawnPosition = new Vector2(leftScreenBorder + (path[0] + 0.5f) * sizeOfWall.x, lastWall.transform.position.y + sizeOfWall.y);
            bonus.transform.position = spawnPosition;
            float duration = (bonus.transform.position.y - bottomEnd + sizeOfWall.y) / levelObjectsSpeed;
            Vector3 pos = new Vector3(bonus.gameObject.transform.position.x, bottomEnd - sizeOfWall.y, 0);
            TweenPosition.SetPosition(bonus.gameObject, pos, duration);
        }
    }


    void SwitchRow()
    {
        if (currentRow == ROWS_TOTAL_COUNT - 1)
        {
            currentRow = 0;
        }
        else
        {
            currentRow++;
        }
    }


    void GenerateNewRow()
    {
        int determiner = Random.Range(0, 101);
        if (determiner <= THRESHOLD_TO_CREATE_DIRECT || path.Length != MIN_PATH_WIDTH)
        {
            GenerateDirectPath();
        }
        else
        {
            if (determiner < THRESHOLD_TO_CREATE_RIGHT)
            {
                GeneratePathToTheRight();
            }
            else
            {
                GeneratePathToTheLeft();
            }
        }
    }


    void GenerateDirectPath()
    {
        path = new int[MIN_PATH_WIDTH];
        path[0] = pathColumnCurrent;
    }


    void GeneratePathToTheRight()
    {
        int determiner = Random.Range(MIN_PATH_WIDTH, rightColumn - pathColumnCurrent + 1);
        path = new int[determiner];
        for (int i = 0; i < path.Length; i++)
        {
            path[i] = pathColumnCurrent++;
        }
        pathColumnCurrent--;
    }


    void GeneratePathToTheLeft()
    {
        int determiner = Random.Range(MIN_PATH_WIDTH, pathColumnCurrent - LEFT_BORDER + 1);
        path = new int[determiner];
        for (int i = 0; i < path.Length; i++)
        {
            path[i] = pathColumnCurrent--;
        }
        pathColumnCurrent++;
    }

    #endregion
}