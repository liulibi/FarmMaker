using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class Player : SingletonMonobehaviour<Player>,ISaveable
{
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds afterPickAnimationPause;

    private AnimationOverrides animationOverrides;

    private GridCursor gridCursor;
    private Cursor cursor;


    //PlayerMovement 参数
    private float xInput;
    private float yInput;
    private bool isCarrying = false;
    private bool isIdle;
    private bool isLiftingToolDown;
    private bool isLiftingToolLeft;
    private bool isLiftingToolRight;
    private bool isLiftingToolUp;
    private bool isRunning;
    private bool isUsingToolDown;
    private bool isUsingToolLeft;
    private bool isUsingToolRight;
    private bool isUsingToolUp;
    private bool isSwingingToolDown;
    private bool isSwingingToolRight;
    private bool isSwingingToolLeft;
    private bool isSwingingToolUp;
    private bool isWalking;
    private bool isPickingUp;
    private bool isPickingDown;
    private bool isPickingRight;
    private bool isPickingLeft;

    private Camera mainCamera;

    private bool playerToolUseDisabled = false;

    private ToolEffect toolEffect = ToolEffect.none;

    private Rigidbody2D rigidbody2D;

    private WaitForSeconds useToolAniamtionPause;//使用缩放时间将协程执行暂停指定的秒数。
    private WaitForSeconds liftToolAnimationPause;
    private WaitForSeconds pickAnimationPause;

    private Direction playerDirection;

    private List<CharacterAttribute> characterAttributecCustomisationList;

    [Tooltip("Should be populated in the prefab with the equipped item sprite render")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRender = null;

    private CharacterAttribute armsCharacterAttribute;

    private CharacterAttribute toolCharacterAttribute;

    private float movementSpeed;

    private bool _playerInputDisabled = false;

    public bool PlayerInputIsDisabled { get => _playerInputDisabled; set => _playerInputDisabled = value; }

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    protected override void Awake()
    {
        base.Awake();

        rigidbody2D = GetComponent<Rigidbody2D>();

        animationOverrides = GetComponentInChildren<AnimationOverrides>();

        //获得角色的GUID并创建数据保存对象
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        GameObjectSave = new GameObjectSave();

        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColour.none, PartVariantType.none);
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.tool, PartVariantColour.none, PartVariantType.hoe);

        characterAttributecCustomisationList = new List<CharacterAttribute>();

        //获取主摄像机的引用
        mainCamera = Camera.main;
    }
    

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();

        useToolAniamtionPause = new WaitForSeconds(Settings.useToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        pickAnimationPause = new WaitForSeconds(Settings.pickAniamtionPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        afterPickAnimationPause = new WaitForSeconds(Settings.afterPickAnimationPsuse);
    }

    private void Update()
    {
        #region 玩家input
        if (!PlayerInputIsDisabled)
        {
            ResetAnimationTriggers();

            PlayerMovementInput();

            PlayerWalkInput();

            PlayerTestInput();

            PlayerClickInput();

            EventHander.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying,
                toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);
        }
        #endregion

    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHander.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndRestMovement;
        EventHander.AfterSceneLoadFadeInEvent += EnablePlayerInput;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHander.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndRestMovement;
        EventHander.AfterSceneLoadFadeInEvent -= EnablePlayerInput;
    }

    private void PlayerMovement()
    {
        Vector2 move = new Vector2(xInput * movementSpeed * Time.deltaTime, yInput * movementSpeed * Time.deltaTime);
        rigidbody2D.MovePosition(rigidbody2D.position + move);
    }

    private void ResetAnimationTriggers()
    {
        isPickingDown = false;
        isPickingLeft = false;
        isPickingRight = false;
        isPickingUp = false;
        isUsingToolRight = false;
        isUsingToolUp = false;
        isUsingToolLeft = false;
        isUsingToolDown = false;
        isLiftingToolDown = false;
        isLiftingToolLeft = false;
        isLiftingToolRight = false;
        isLiftingToolUp = false;
        isSwingingToolDown = false;
        isSwingingToolLeft = false;
        isSwingingToolRight = false;
        isSwingingToolUp = false;
        toolEffect = ToolEffect.none;
    }

    private void PlayerMovementInput()
    {
        yInput = Input.GetAxisRaw("Vertical");
        xInput = Input.GetAxisRaw("Horizontal");
        if (yInput != 0 && xInput != 0)//x，y同时按按照对角移动
        {
            xInput = xInput * 0.71f;
            yInput = yInput * 0.71f;
        }
        if (xInput != 0 || yInput != 0)
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;

            if (xInput < 0)
            {
                playerDirection = Direction.left;
            }
            if (xInput > 0)
            {
                playerDirection = Direction.right;
            }
            if (yInput > 0)
            {
                playerDirection = Direction.up;
            }
            if (yInput < 0)
            {
                playerDirection = Direction.down;
            }
        }
        else if (xInput == 0 && yInput == 0)
        {
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }
    }

    private void PlayerWalkInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movementSpeed = Settings.walkingSpeed;
        }
        else
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;
        }
    }


    private void PlayerClickInput()
    {
        if (!playerToolUseDisabled)
        {
            if (Input.GetMouseButtonDown(0))//得到鼠标左键点击
            {

                if (gridCursor.CursorIsEnable||cursor.CursorIsEnabled)
                {
                  //Get cursor grid position
                  Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();

                  //Get player grid position
                  Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();

                  ProcessPlayerClickInput(cursorGridPosition,playerGridPosition);
                }
            }
        }
        
    }


    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        ResetMovement();

        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);

        //在光标位置获取网格属性详细信息（Gridcursor 验证例程确保网格属性详细信息不为空
        GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        //Get selected item details
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails != null)
        {
            if (itemDetails.itemType == ItemType.Seed)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ProcessPlayerClickInputSeed(gridPropertyDetails,itemDetails);
                }
            }
            else if (itemDetails.itemType == ItemType.Commodity)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ProcessPlayerClickInputCommodity(itemDetails);
                }
            }
            else if (itemDetails.itemType == ItemType.Hoeing_tool
                ||itemDetails.itemType==ItemType.Watering_tool
                ||itemDetails.itemType==ItemType.Reaping_tool
                ||itemDetails.itemType==ItemType.Collecting_tool
                || itemDetails.itemType==ItemType.Chopping_tool
                ||itemDetails.itemType==ItemType.Breaking_tool)
            {
                PorcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
            }
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        else if (cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        else if (cursorGridPosition.y < playerGridPosition.y)
        {
            return Vector3Int.down;
        }
        else
        {
            return Vector3Int.right;
        }
    }

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition,Vector3 playerPosition)
    {
        if (
            cursorPosition.x > playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)
            )
        {
            return Vector3Int.right;
        }
        else if(
            cursorPosition.x<playerPosition.x
            &&
            cursorPosition.y<(playerPosition.y+cursor.ItemUseRadius/2f)
            &&
            cursorPosition.y>(playerPosition.y-cursor.ItemUseRadius/2f)
            )
        {
            return Vector3Int.left;
        }else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }else
        {
            return Vector3Int.down;
        }
    }

    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails,ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsVaild && gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }
        else if (itemDetails.canBeDropped && gridCursor.CursorPositionIsVaild)
        {
            EventHander.CallDrapSelectedItemEvent();
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails,ItemDetails itemDetails)
    {
        //update grid properties with seed details
        gridPropertyDetails.seedItemCode = itemDetails.itemCode;
        gridPropertyDetails.growthDays = 0;

        //play sound
        AudioManager.Instance.PlaySound(SoundName.effectPlantingSound);

        //display planted crop at grid property details
        GridPropertyManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

        //remove item from inventory
        EventHander.CallRemoveSelectedItemFromInventoryEvent();
    }


    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsVaild)
        {
            EventHander.CallDrapSelectedItemEvent();
        }
    }

    private void PorcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails,ItemDetails itemDetails,Vector3Int playerDirection)
    {
        //Switch on tool
        if (itemDetails.itemType == ItemType.Hoeing_tool)
        {
            if (gridCursor.CursorPositionIsVaild)
            {
                HoeGroundAtCursor(gridPropertyDetails, playerDirection);
            }
        }
        else if (itemDetails.itemType == ItemType.Watering_tool)
        {
            if (gridCursor.CursorPositionIsVaild)
            {
                WaterGroundAtCursor(gridPropertyDetails, playerDirection);
            }
        }
        else if (itemDetails.itemType == ItemType.Reaping_tool)
        {
            if (cursor.CursorPositionIsValid)
            {
                playerDirection = GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetPlayerCentrePosition());
                ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
            }
        }
        else if (itemDetails.itemType == ItemType.Collecting_tool)
        {
            if (gridCursor.CursorPositionIsVaild)
            {
                CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
            }
        }
        else if (itemDetails.itemType == ItemType.Chopping_tool)
        {
            if (gridCursor.CursorPositionIsVaild)
            {
                ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
            }
        }
        else if (itemDetails.itemType == ItemType.Breaking_tool)
        {
            if (gridCursor.CursorPositionIsVaild)
            {
                BreakInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
            }
        }
    }




    //hoe
    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails,Vector3Int playerDirection)
    {
        //play sound 
        AudioManager.Instance.PlaySound(SoundName.effectHoe);

        //Trigger animation
        StartCoroutine(HoeGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }
    private IEnumerator HoeGroundAtCursorRoutine(Vector3Int playerDirection,GridPropertyDetails gridPropertyDetails)
    {
        playerToolUseDisabled = true;
        PlayerInputIsDisabled = true;

        //设置playeranimation为hoe 在override animation内
        toolCharacterAttribute.partVariantType = PartVariantType.hoe;
        characterAttributecCustomisationList.Clear();
        characterAttributecCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributecCustomisationList);

        if (playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }else if (playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }else if (playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        yield return useToolAniamtionPause;

        //set grid property details for dug ground
        if (gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        //set grid property to dug
        GridPropertyManager.Instance.SetGridPorpertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY,gridPropertyDetails);

        //diaplay dug grid tiles
        GridPropertyManager.Instance.DisplayDugGround(gridPropertyDetails);

        //after animation pause
        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    //water
    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails,Vector3Int playerDirection)
    {
        //play sound 
        AudioManager.Instance.PlaySound(SoundName.effectWateringCan);

        //Trigger animation
        StartCoroutine(WaterGroundAtCursorRoutine(playerDirection,gridPropertyDetails));
    }
    private IEnumerator WaterGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set tool animation to watering  can in override animation
        toolCharacterAttribute.partVariantType = PartVariantType.wateringCan;
        characterAttributecCustomisationList.Clear();
        characterAttributecCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributecCustomisationList);

        //todo:if there is water in the watering can 
        if (playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }
        yield return liftToolAnimationPause;

        //set grid property details for dug ground
        if (gridPropertyDetails.daysSinceWatered== -1)
        {
            gridPropertyDetails.daysSinceWatered= 0;
        }

        //set grid property to dug
        GridPropertyManager.Instance.SetGridPorpertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //diaplay dug grid tiles
        GridPropertyManager.Instance.DisplayWateredGround(gridPropertyDetails);

        //after animation pause
        yield return afterLiftToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    //collect
    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails,ItemDetails itemDetails,Vector3Int playerDirection)
    {
        //play sound 
        AudioManager.Instance.PlaySound(SoundName.effectBasket);

        StartCoroutine(CollectInPlayerDirectionRoutine(gridPropertyDetails, itemDetails, playerDirection));
    }
    private IEnumerator CollectInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails,ItemDetails equippeditemDetails,Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippeditemDetails, gridPropertyDetails);

        yield return pickAnimationPause;
        
        //after animation pause
        yield return afterPickAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    //chop
    private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails,ItemDetails equippedItemDetails,Vector3Int playerDirection)
    {
        //play sound 
        AudioManager.Instance.PlaySound(SoundName.effectAxe);

        StartCoroutine(ChopInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }
    private IEnumerator ChopInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails,ItemDetails equippedItemDetails,Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set tool animation to axe in override animation
        toolCharacterAttribute.partVariantType = PartVariantType.axe;
        characterAttributecCustomisationList.Clear();
        characterAttributecCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributecCustomisationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAniamtionPause;

        yield return afterUseToolAnimationPause;

        playerToolUseDisabled = false;
        PlayerInputIsDisabled = false;
    }


    //break
    private void BreakInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        //play sound 
        AudioManager.Instance.PlaySound(SoundName.effectPickaxe);

        StartCoroutine(BreakInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }
    private IEnumerator BreakInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set tool animation to axe in override animation
        toolCharacterAttribute.partVariantType = PartVariantType.pickaxe;
        characterAttributecCustomisationList.Clear();
        characterAttributecCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributecCustomisationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAniamtionPause;

        yield return afterUseToolAnimationPause;

        playerToolUseDisabled = false;
        PlayerInputIsDisabled = false;
    }


    /// <summary>
    /// method processes crop with equipped item in player direction
    /// </summary>
    /// <param name="playerDirection"></param>
    /// <param name="equippedItemDetails"></param>
    /// <param name="gridPropertyDetails"></param>
    private void ProcessCropWithEquippedItemInPlayerDirection(Vector3Int playerDirection,ItemDetails equippedItemDetails,GridPropertyDetails gridPropertyDetails)
    {
        if (equippedItemDetails.itemType == ItemType.Chopping_tool
            ||equippedItemDetails.itemType==ItemType.Breaking_tool)
        {
            if (playerDirection == Vector3Int.right)
            {
                isUsingToolRight = true;
            }
            else if (playerDirection == Vector3Int.left)
            {
                isUsingToolLeft = true;
            }
            else if (playerDirection == Vector3Int.up)
            {
                isUsingToolUp = true;
            }
            else if (playerDirection == Vector3Int.down)
            {
                isUsingToolDown = true;
            }
        }

        if (equippedItemDetails.itemType == ItemType.Collecting_tool)
        {
            if (playerDirection == Vector3Int.right)
            {
                isPickingRight = true;
            }
            else if (playerDirection == Vector3Int.left)
            {
                isPickingLeft = true;
            }
            else if (playerDirection == Vector3Int.up)
            {
                isPickingUp = true;
            }
            else if (playerDirection == Vector3Int.down)
            {
                isPickingDown = true;
            }
        }

        //get crop at cursor grid location
        Crop crop = GridPropertyManager.Instance.GetCropObjectAtGridLoaction(gridPropertyDetails);

        //对作物执行加工工具动作
        if (crop != null)
        {
            if (equippedItemDetails.itemType == ItemType.Collecting_tool)
            {
                crop.ProcessToolAction(equippedItemDetails,isPickingRight,isPickingLeft,isLiftingToolDown,isLiftingToolUp);
            }else if (equippedItemDetails.itemType == ItemType.Chopping_tool)
            {
                crop.ProcessToolAction(equippedItemDetails, isUsingToolRight, isUsingToolLeft, isUsingToolDown, isUsingToolUp);
            }
            else if (equippedItemDetails.itemType == ItemType.Breaking_tool)
            {
                crop.ProcessToolAction(equippedItemDetails, isUsingToolRight, isUsingToolLeft, isUsingToolDown, isUsingToolUp);
            }
        }

    }


    //reap
    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }
    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails,Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set tool animation to scythe in overriade animation
        toolCharacterAttribute.partVariantType = PartVariantType.scythe;
        characterAttributecCustomisationList.Clear();
        characterAttributecCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributecCustomisationList);

        //reaping in player direction
        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return useToolAniamtionPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }



    private void UseToolInPlayerDirection(ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        if (Input.GetMouseButton(0))
        {
            if (equippedItemDetails.itemType == ItemType.Reaping_tool)
            {
                if (playerDirection == Vector3Int.right)
                {
                    isSwingingToolRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isSwingingToolLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isSwingingToolUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isSwingingToolDown = true;
                }
            }

            //define centre point  of square which will be used for collision testing
            Vector2 point = new Vector2(GetPlayerCentrePosition().x + (playerDirection.x * (equippedItemDetails.itemUseRadius / 2f)), GetPlayerCentrePosition().y + playerDirection.y * (equippedItemDetails.itemUseRadius / 2f));

            //define size of the square which will be used for collision testing 
            Vector2 size = new Vector2(equippedItemDetails.itemUseRadius, equippedItemDetails.itemUseRadius);

            //get item components with 2D collider located in the square at the centra point define (2d collider tested limited to maxColliderTotestPerReapSwing)
            Item[] itemArray = HelperMethods.GetComponentAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPerreapSwing, point, size, 0f);
            int reapableItemCount = 0;
            for (int i = itemArray.Length - 1; i >= 0; i--)
            {
                if (itemArray[i] != null)
                {
                    //destroy item game object if reapable
                    if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.Reapable_scenary)
                    {
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Settings.gridCellSize / 2f, itemArray[i].transform.position.z);

                        //Trigger reaping effect
                        EventHander.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);

                        //play sound 
                        AudioManager.Instance.PlaySound(SoundName.effectScythe);

                        Destroy(itemArray[i].gameObject);

                        reapableItemCount++;
                        if (reapableItemCount > Settings.maxCollidersToTestPerreapSwing)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }


    private void PlayerTestInput()
    {
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }

    }

    public void ResetMovement()
    {//reset movement
        xInput = 0;
        yInput = 0;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }

    public void DisablePlayerInputAndRestMovement()
    {
        DisablePlayerInput();
        ResetMovement();
        EventHander.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying,
               toolEffect,
               isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
               isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
               isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
               isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
               false, false, false, false); 

    }

    public void DisablePlayerInput()
    {
        PlayerInputIsDisabled = true;
    }

    public void EnablePlayerInput()
    {
        PlayerInputIsDisabled = false;
    }

    public void ClearCarriedItem()
    {
        equippedItemSpriteRender.sprite = null;
        equippedItemSpriteRender.color = new Color(1f, 1f, 1f, 0f);

        armsCharacterAttribute.partVariantType = PartVariantType.none;
        armsCharacterAttribute.partVariantColour = PartVariantColour.none;
        characterAttributecCustomisationList.Clear();
        characterAttributecCustomisationList.Add(armsCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributecCustomisationList);

        isCarrying = false;

    }

    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if (itemDetails != null)
        {
            equippedItemSpriteRender.sprite = itemDetails.itemSprite;
            equippedItemSpriteRender.color = new Color(1f, 1f, 1f,1f);

            armsCharacterAttribute.partVariantType = PartVariantType.carry;
            characterAttributecCustomisationList.Clear();
            characterAttributecCustomisationList.Add(armsCharacterAttribute);
            animationOverrides.ApplyCharacterCustomisationParameters(characterAttributecCustomisationList);

            isCarrying = true;
        }
    }

    public Vector3 GetPlayerViewportPosition()
    {//将位置转为世界坐标
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    public Vector3 GetPlayerCentrePosition()
    {
        //不改变其轴点，只是输出时将轴点换在player的中心
        return new Vector3(transform.position.x, transform.position.y+Settings.playerCentraYOffset, transform.position.z);
    }


    private void SetPlayerDirection(Direction playerDirection)
    {
        switch (playerDirection) 
        {
            case Direction.up:
                //set idle up triggger
                EventHander.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false);
                break;
            case Direction.down:
                EventHander.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false);
                break;
            case Direction.left:
                EventHander.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false);
                break;
            case Direction.right:
                EventHander.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true);
                break;
            default:
                //set idle down trigger
                EventHander.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false);
                break;

        }

    }




    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectlist.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectlist.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        //delete saveScene for game object if it already exists
        GameObjectSave.sceneDate.Remove(Settings.PersistentScene);

        //Creat saveScene for game object
        SceneSave sceneSave = new SceneSave();

        //creat Verctor3 Dictionary 
        sceneSave.vector3Dictionary = new Dictionary<string, Verctor3Serializable>();

        //Creat String Dictionary 
        sceneSave.stringDictionary = new Dictionary<string, string>();

        //add player position to verctor3 dictionary
        Verctor3Serializable verctor3Serializable = new Verctor3Serializable(transform.position.x, transform.position.y, transform.position.z);
        sceneSave.vector3Dictionary.Add("playerPosition", verctor3Serializable);

        //add current Scene Name to String dictionary
        sceneSave.stringDictionary.Add("currentScene", SceneManager.GetActiveScene().name);

        //add player direction to string dictionary
        sceneSave.stringDictionary.Add("playerDirection", playerDirection.ToString());

        //add sceneSave data for player game object
        GameObjectSave.sceneDate.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectDate.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            //get save data dictionary for scene
            if(gameObjectSave.sceneDate.TryGetValue(Settings.PersistentScene,out SceneSave sceneSave))
            {
                //get player position
                if (sceneSave.vector3Dictionary != null && sceneSave.vector3Dictionary.TryGetValue("playerPosition", out Verctor3Serializable playerPosition))
                {
                    transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                }

                //get string dictionary 
                if (sceneSave.stringDictionary != null)
                {
                    //get player scene
                    if(sceneSave.stringDictionary.TryGetValue("currentScene",out string currentScene))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                    }

                    //get player direction
                    if(sceneSave.stringDictionary.TryGetValue("playerDirection",out string playerDir))
                    {
                        bool playerDirFound = Enum.TryParse<Direction>(playerDir, true, out Direction direction);

                        if (playerDirFound)
                        {
                            playerDirection = direction;
                            SetPlayerDirection(playerDirection);
                        }
                    }
                }
            }
        }
    }

   
    public void ISaveableStoreScene(string sceneName)
    {
        //Nothing required here since the player is on a persistent scene
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //Nothing required here since the player is on a persistent scene
    }
}
