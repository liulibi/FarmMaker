public enum AnimationName//animation名称
{
    idleDown,
    idleUp,
    idleRight,
    idleLeft,
    walkUp,
    walkDown,
    walkRight,
    walkLeft,
    runUp,
    runDown,
    runRight,
    runLeft,
    useToolUp,
    useToolDown,
    useToolRight,
    useToolLeft,
    swingToolUp,
    swingToolDown,
    swingToolRight,
    swingToolLeft,
    liftToolUp,
    liftToolDown,
    liftToolRight,
    liftToolLeft,
    holdToolUp,
    holdToolDown,
    holdToolRight,
    holdToolLeft,
    pickDown,
    pickUp,
    pickRight,
    pickLeft,
    count
}

public enum CharacterPartAnimator//动画部位
{
    body,
    arms,
    hair,
    tool,
    hat,
    count
}

public enum PartVariantColour//颜色
{
    none,
    count
}


public enum PartVariantType//部位的动作
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    count
}

public enum GridBoolProperty//地图网格信息
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,//寻路功能
    isNPCObstacle
}

public enum InventoryLocation//库存位置
{
    player,
    chest,//背包
    count
}

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter,
    none,
    count
}

public enum ToolEffect
{
    none,
    watering
}

public enum HarvestActionEffect 
{
    deciduousLeavesFalling,// 落叶飘零、
    pineConesFalling,// 松子落下、
    choppingTreeTrunk,// 砍树树干
    breakingStone,
    reaping,
    none
}

public enum Weather
{
    dry,
    raining,
    snowing,
    none,
    count
}


public enum Direction
{
    up,
    down,
    left,
    right,
    none
}

public enum ItemType//工具
{
    Seed,//种子
    Commodity,//商品
    Watering_tool,
    Hoeing_tool,//锄头
    Chopping_tool,
    Breaking_tool,//斧头
    Reaping_tool,//镰刀
    Collecting_tool,//收获
    Reapable_scenary,
    furniture,
    none,
    count
}

public enum SceneName
{
    Farm,
    Field,
    Cabin,
    TransitionScene,
    City,
    CityRoad
}

public enum Facing
{
    none,
    front,
    back,
    right
}

public enum SoundName 
{
    none=0,
    effectFootstepSoftGround=10,
    effectFootstepHardGround=20,
    effectAxe=30,
    effectPickaxe=40,
    effectScythe=50,
    effectHoe=60,
    effectWateringCan=70,
    effectBasket=80,
    effectPickupSound=90,
    effectRustle=100,
    effectTreeFalling=110,
    effectPlantingSound=120,
    effectPluck=130,
    effectStoneShatter=140,
    effectWoodSpllinters=150,
    ambientCountrySide1=1000,
    ambientCountrySide2=1010,
    ambientIndoors1=1020,
    musicCalm3=2000,
    musicCalm1=2010

}
