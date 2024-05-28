 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertyManager : SingletonMonobehaviour<GridPropertyManager>, ISaveable
{
    public Grid grid;
    private Transform cropsParnentTransform;
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    private bool isFirstTimeSceneLoaded = true;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;//key 是网格坐标
    [SerializeField] private SO_CropDetailsList so_cropDetailsLists = null;
    [SerializeField] private SO_Gridproperties[] so_gridProperiesArray = null;
    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] wateredGround = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }


    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }
    private void Start()
    {
        IntitialiseGridProperties();//初始化网格信息
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHander.AfterSceneLoadEvent -= AfterSceneLoad;
        EventHander.AdvanceGameDayEvent -= AdvanceDay;

    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHander.AfterSceneLoadEvent += AfterSceneLoad;
        EventHander.AdvanceGameDayEvent += AdvanceDay;
    }

    //在tilemap中
    private void ClearDisplayGroundDecorations()
    {
        //清除地面装饰
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        //destroy all crop in scene
        Crop[] cropArray;
        cropArray = FindObjectsOfType<Crop>();


        foreach(Crop crop in cropArray)
        {
            Destroy(crop.gameObject);
        }
    }

    //还原地面在gridproperty中
    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();

        ClearDisplayAllPlantedCrops();
    }



    //dug ground
    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }
    //选择dug瓦片
    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        //根据周围瓦片挖掘情况选择瓷砖基础

        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        //set 4 Tiles if dug surrounding current tile - up,down,left,right now that this central tile has been dug 
        GridPropertyDetails adjaccentGridPropertyDetails;

        adjaccentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);// adjaccent 毗连的
        if (adjaccentGridPropertyDetails != null && adjaccentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        adjaccentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjaccentGridPropertyDetails != null && adjaccentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1), dugTile2);
        }


        adjaccentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjaccentGridPropertyDetails != null && adjaccentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY), dugTile3);
        }


        adjaccentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjaccentGridPropertyDetails != null && adjaccentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY), dugTile4);//根据给定的瓦片地图中某个单元格的 XYZ 坐标，设置瓦片。
        }

        
    }
    //判断dug瓦片
    private Tile SetDugTile(int xGrid, int yGrid)
    {
        //get whether surrounding tiles (up, down ,left ,and right)are dug or not 
        bool upDug = isGridSquareDug(xGrid, yGrid + 1);
        bool downDug = isGridSquareDug(xGrid, yGrid - 1);
        bool leftDug = isGridSquareDug(xGrid - 1, yGrid);
        bool rightDug = isGridSquareDug(xGrid + 1, yGrid);

        #region 根据周围的瓦片是否被挖开，设置适当的瓦片
        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8];
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }

        return null;
        #endregion 根据周围的瓦片是否被挖开，设置适当的瓦片 
    }
    //dug 判断
    private bool isGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }




    //water ground
    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }
    //选择water瓦片
    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        //根据周围瓦片挖掘情况选择瓷砖基础

        Tile wateredTile0 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), wateredTile0);

        //set 4 Tiles if dug surrounding current tile - up,down,left,right now that this central tile has been dug 
        GridPropertyDetails adjaccentGridPropertyDetails;

        adjaccentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);// adjaccent 毗连的
        if (adjaccentGridPropertyDetails != null && adjaccentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile1 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), wateredTile1);
        }

        adjaccentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjaccentGridPropertyDetails != null && adjaccentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile2 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1), wateredTile2);
        }


        adjaccentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjaccentGridPropertyDetails != null && adjaccentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile3 = SetWateredTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY), wateredTile3);
        }


        adjaccentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjaccentGridPropertyDetails != null && adjaccentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile4 = SetWateredTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY), wateredTile4);//根据给定的瓦片地图中某个单元格的 XYZ 坐标，设置瓦片。
        }
    }
    //通过周围瓦片情况设置water瓦片
    private Tile SetWateredTile(int xGrid, int yGrid)
    {
        //get whether surrounding tiles (up, down ,left ,and right)are dug or not 
        bool upWatered = isGridSquareWatered(xGrid, yGrid + 1);
        bool downWatered = isGridSquareWatered(xGrid, yGrid - 1);
        bool leftWatered = isGridSquareWatered(xGrid - 1, yGrid);
        bool rightWatered = isGridSquareWatered(xGrid + 1, yGrid);

        #region 根据周围的瓦片是否被浇灌，设置适当的瓦片
        if (!upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[0];
        }
        else if (!upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[1];
        }
        else if (!upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[2];
        }
        else if (!upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[3];
        }
        else if (!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[4];
        }
        else if (upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[5];
        }
        else if (upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[6];
        }
        else if (upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[7];
        }
        else if (upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[8];
        }
        else if (upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[9];
        }
        else if (upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[10];
        }
        else if (upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[11];
        }
        else if (upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[12];
        }
        else if (!upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[13];
        }
        else if (!upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[14];
        }
        else if (!upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[15];
        }

        return null;
        #endregion 根据周围的瓦片是否被挖开，设置适当的瓦片 
    }
    //判断瓦片water情况
    private bool isGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    //plant crop
    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.seedItemCode > -1)
        {
            //get crop details
            CropDetails cropDetails = so_cropDetailsLists.GetCropDetails(gridPropertyDetails.seedItemCode);

           
            //prefab to use
            GameObject cropPrefab;

            //instantiate crop prefab at grid location
            int growthStages = cropDetails.growthDays.Length;

            int currentGrowthStage = 0;

            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (gridPropertyDetails.growthDays >= cropDetails.growthDays[i])
                {
                    currentGrowthStage = i;
                    
                    break;
                }
            }

            cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

            

            Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

            Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

            worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y, worldPosition.z);


            GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);


            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
            cropInstance.transform.SetParent(cropsParnentTransform);
            cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        }
    }


    //根据瓦片信息显示相应的改变
    private void DisplayGridPropertyDetails()
    {
        //loop through all grid item 
        foreach (KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);

            DisplayWateredGround(gridPropertyDetails);

            DisplayPlantedCrop(gridPropertyDetails);

        }
    }



    /// <summary>
    /// 使用SO_GridProperties中的assets 去初始化grid property dictionary 并且存储这些信息for each scene in GameObjectScave sceneData
    /// </summary>
    private void IntitialiseGridProperties()
    {
        //遍历所有的元素在SO_GridProperties中的
        foreach (SO_Gridproperties so_GridProperties in so_gridProperiesArray)
        {//创建网格信息的字典
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            //填充网格信息字典——循环访问“SO 网格属性”列表中的所有网格属性
            foreach (GridProperty gridProperty in so_GridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;

                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);

                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }
                if (gridProperty.gridBoolProperty == GridBoolProperty.diggable)
                {
                    gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                }
                if (gridProperty.gridBoolProperty == GridBoolProperty.canDropItem)
                {
                    gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                }
                if (gridProperty.gridBoolProperty == GridBoolProperty.canPlaceFurniture)
                {
                    gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                }
                if (gridProperty.gridBoolProperty == GridBoolProperty.isPath)
                {
                    gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                }
                if (gridProperty.gridBoolProperty == GridBoolProperty.isNPCObstacle)
                {
                    gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                }

                SetGridPorpertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }

            //create scene save for the gameObject
            SceneSave sceneSave = new SceneSave();

            //add grid property dictionary to scene save data
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            //if starting scene set the gridPropertyDictionary 
            if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            //add bool dictionary and set first time scene loaded to true
            sceneSave.boolDictionary = new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", true);

            //Add scene save to game object scene data
            GameObjectSave.sceneDate.Add(so_GridProperties.sceneName.ToString(), sceneSave);
        }
    }
    public void SetGridPorpertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPorpertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    public void SetGridPorpertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDitionary)
    {
        //construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        //set value 
        gridPropertyDitionary[key] = gridPropertyDetails;
    }


    /// <summary>
    /// 在提供的字典的网格位置返回 gridpropertydetails，如果该位置不存在任何属性，则返回 null
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <param name="gridPropertyDictionary"></param>
    /// <returns></returns>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        //以坐标构建key
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        //检查坐标和检索的网格属性详细信息是否存在
        if (!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            //if not found
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    /// <summary>
    /// 在提供的字典的网格位置返回 gridpropertydetails，如果该位置不存在任何属性，则返回 null
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns></returns>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    /// <summary>
    /// returns the crop object at the gridx,gridy position or null if no crop was found
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <returns></returns>
    public Crop GetCropObjectAtGridLoaction(GridPropertyDetails gridPropertyDetails)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);

        //loop through collider to get crop game object
        Crop crop = null;

        for(int i = 0; i < collider2DArray.Length; i++)
        {
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
            crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
        }
        return crop;
    }

    /// <summary>
    /// returns crop details for the provided seeditemcode
    /// </summary>
    /// <param name="ssedItemCode"></param>
    /// <returns></returns>
    public CropDetails GetCropDetails(int ssedItemCode)
    {
        return so_cropDetailsLists.GetCropDetails(ssedItemCode);
    }


    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        //清除所有的地图装饰信息，如dug water
        ClearDisplayGroundDecorations();
        ClearDisplayAllPlantedCrops();

        //遍历所有的地图信息
        foreach (SO_Gridproperties so_GridProperties in so_gridProperiesArray)
        {
            if (GameObjectSave.sceneDate.TryGetValue(so_GridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
                if (sceneSave.gridPropertyDetailsDictionary != null)
                {
                    for (int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;

                        #region 更新所有的grid property 通过时间的映射
                        //if a crop is planted
                        if (gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays += 1;
                        }

                        //if ground is watered, the clear water
                        if (gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        //set propertydetails
                        SetGridPorpertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);

                        #endregion 更新
                    }
                }
            }
        }
        DisplayGridPropertyDetails();
    }

    private void AfterSceneLoad()
    {
        if (GameObject.FindGameObjectWithTag(Tags.CropsParentTransform) != null)
        {
            cropsParnentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
        }
        else
        {
            cropsParnentTransform = null;
        }

        //Get Grid            
        grid = Object.FindObjectOfType<Grid>();

        //get tilemap
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectlist.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectlist.Remove(this);
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectDate.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            //重新储存当前场景信息
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //得到场景信息
        if (GameObjectSave.sceneDate.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }

            if(sceneSave.boolDictionary!=null&&sceneSave.boolDictionary.TryGetValue("isFirstTimeSceneLoaded",out bool storeIsFirstTimeSceneLoaded))
            {
                isFirstTimeSceneLoaded = storeIsFirstTimeSceneLoaded;
            }

            if (isFirstTimeSceneLoaded)
            {
                EventHander.CallInstantiateCropPerfabsEvent();
            }

            if (gridPropertyDictionary.Count > 0)
            {
                ClearDisplayGridPropertyDetails();

                DisplayGridPropertyDetails();
            }

            if (isFirstTimeSceneLoaded == true)
            {
                isFirstTimeSceneLoaded = false;
            }
        }
    }

    public GameObjectSave ISaveableSave()
    {
        //储存当前场景信息
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //remove sceneSave for scene
        GameObjectSave.sceneDate.Remove(sceneName);

        //Create scenceSave for scene
        SceneSave sceneSave = new SceneSave();

        //创建和添加字典属性详细信息字典
        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", isFirstTimeSceneLoaded);

        //add scene save to game object scene data
        GameObjectSave.sceneDate.Add(sceneName, sceneSave);
    }

    /// <summary>
    /// for sceneName this method returns a Vector2Int with the gird dimensions for that scene, or Vector2Int.zero if scene not found
    /// </summary>
    //AStar
    public bool GetGirdDimensions(SceneName sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)
    {
        gridDimensions = Vector2Int.zero;
        gridOrigin = Vector2Int.zero;

        //loop through scenes
        foreach (SO_Gridproperties so_Gridproperties in so_gridProperiesArray)
        {
            if (so_Gridproperties.sceneName == sceneName)
            {
                gridDimensions.x = so_Gridproperties.gridWidth;
                gridDimensions.y = so_Gridproperties.gridHeight;

                gridOrigin.x = so_Gridproperties.originx;
                gridOrigin.y = so_Gridproperties.originy;

                return true;
            }
        }

        return false;
    }
}