                                
using UnityEngine;

public static class Settings 
{
    // 场景
    public const string PersistentScene = "PersistentScene";

    [Header("库存参数")]
    public static int playerInitialInventoryCapacity = 24; //玩家初始库存容量
    public static int playerMaximumInventoryCapacity = 48;//玩家最大库存容量


    //玩家与物品发生遮盖时，物品褪色的参数
    public const float fadeInSeconds = 0.25f;
    public const float fadeOutSeconds = 0.35f;
    public const float targetAlpha = 0.5f;//褪色度

    //Tilemap
    public const float gridCellSize = 1f;//Unity中的网格单元大小
    public const float gridCellDiagonalSize = 1.41f;//diaggonal distance between unity cell centres
    public static Vector2 cursorSize = Vector2.one;


    //player
    public static float playerCentraYOffset = 0.875f;

    [Header("运动参数")]
    public const float runningSpeed=6.333f;
    public const float walkingSpeed=2.666f;
    public static float useToolAnimationPause = 0.25f;//人物开始使用工具动作停顿时间
    public static float liftToolAnimationPause = 0.4f;
    public static float pickAniamtionPause = 1f;
    public static float afterUseToolAnimationPause = 0.3f;//人物使用工具动作完成后的停顿时间
    public static float afterLiftToolAnimationPause = 0.6f;
    public static float afterPickAnimationPsuse = 0.2f;

    //NPC Movement
    public static float pixelSize = 0.0625f;

    //NPC Animation Parameters
    public static int walkUp;
    public static int walkDown;
    public static int walkLeft;
    public static int walkRight;
    public static int eventAnimation;

    //Player动画参数编号
    [Header("移动参数")]
    public static int xInput;
    public static int yInput;
    public static int isWalking;
    public static int isRunning;



    [Header("使用工具参数")]
    public static int toolEffect;
    public static int isUsingToolRight;
    public static int isUsingToolLeft;
    public static int isUsingToolUp;
    public static int isUsingToolDown;



    [Header("工具举起动作参数")]
    public static int isLiftingToolRight;
    public static int isLiftingToolLeft;
    public static int isLiftingToolUp;
    public static int isLiftingToolDown;



    [Header("挥舞工具参数")]
    public static int isSwingingToolRight;
    public static int isSwingingToolLeft;
    public static int isSwingingToolUp;
    public static int isSwingingToolDown;



    [Header("拾取物品参数")]
    public static int isPickingRight;
    public static int isPickingLeft;
    public static int isPickingUp;
    public static int isPickingDown;



    [Header("玩家参数")]
    public static int idleUp;
    public static int idleDown;
    public static int idleLeft;
    public static int idleRight;

    [Header("工具")]
    public const string HoeingTool = "Hoe";//锄头
    public const string ChopingTool = "Axe";
    public const string BreakingTool = "Pickaxe";
    public const string ReapingTool = "Scythe";//镰刀
    public const string WateringTool = "Watering Can";
    public const string CollectingTool = "Basket";

    [Header("reaping")]
    public const int maxCollidersToTestPerreapSwing = 15;
    public const int maxTargetComponentsToDestroyPerReapSwing = 2;

    public const float secondsPerGameSecond= 0.012f;

    //静态构造函数
    static Settings()
    {
        //NPC Animation parameters
        walkUp = Animator.StringToHash("walkUp");
        walkDown = Animator.StringToHash("walkDown");
        walkLeft = Animator.StringToHash("walkLeft");
        walkRight = Animator.StringToHash("walkRight");
        eventAnimation = Animator.StringToHash("eventAnimation");


        xInput = Animator.StringToHash("xInput");
        yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        toolEffect = Animator.StringToHash("toolEffect");
        isUsingToolRight = Animator.StringToHash("isUsingToolRight");
        isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        isUsingToolDown = Animator.StringToHash("isUsingToolDown");
        isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");
        isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");
        isSwingingToolRight = Animator.StringToHash("isSwingingToolRight");
        isSwingingToolLeft = Animator.StringToHash("isSwingingToolLeft");
        isSwingingToolUp = Animator.StringToHash("isSwingingToolUp");
        isSwingingToolDown = Animator.StringToHash("isSwingingToolDown");
        isPickingRight = Animator.StringToHash("isPickingRight");
        isPickingLeft = Animator.StringToHash("isPickingLeft");
        isPickingUp = Animator.StringToHash("isPickingUp");
        isPickingDown = Animator.StringToHash("isPickingDown");

        //Player参数
        idleRight = Animator.StringToHash("idleRight");
        idleLeft = Animator.StringToHash("idleLeft");
        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
    }


}
