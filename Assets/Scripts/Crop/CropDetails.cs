using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescriptionAttributer]
    public int seedItemCode;//这是对应种子的项目代码
    public int[] growthDays;//每个阶段的生长天数
    public GameObject[] growthPrefab;//prefab to use when instantiating growth stages
    public Sprite[] growthSprite;//生长sprite
    public Season[] seasons;//生长季节
    public Sprite harvestedSprite;//收获动画

    [ItemCodeDescriptionAttributer]
    public int harvestedTransformItemCode;//如果项目在收获时转变为另一个项目，则该项目代码将被填入
    public bool hideCropBeforeHarvestedAnimation;//如果在收割动画之前，作物应该被禁用。
    public bool disableCropCollidersBeforeHarvestedAnimation;//如果作物上的碰撞器应该被禁用，以避免收割动画影响到任何其他游戏对象。
    public bool isHarvestedAnimation;//如果收获动作在生长的最后阶段播放那么这个为真
    public bool isHarvestActionEffect = false;//标志，以确定是否有行动效果
    public bool spawnCropProducedAtPlayerPosition;
    public HarvestActionEffect harvestActionEffect;//收获效果
    public SoundName harvestSound;//the harvest sound for the crop

    [ItemCodeDescriptionAttributer]
    public int[] harvestToolItemCode;//可以收割的工具的项目代码，如果不需要工具，则为0 arry元素。
    public int[] requiredHarvestActions;//在收获工具项目代码阵列中，对应工具所需的收获行动的数量。

    [ItemCodeDescriptionAttributer]
    public int[] cropProducedItemCode;//为收割的作物生产的物品代码数组
    public int[] cropProducedMinQuantity;//收获作物最少数量
    public int[] cropProducedMaxQuantity;//收获作物最多数量
    public int daysToRegrow;//重新生长的天数

    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for(int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if(harvestToolItemCode[i]==toolItemCode)
            return requiredHarvestActions[i];
        }
        return -1;
    }
}
