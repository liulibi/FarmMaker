 using System.Collections;
using UnityEngine;

public class Crop : MonoBehaviour
{
    private int harvestActionCount = 0;

    [Tooltip("This should be populated from child gameobject")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRender = null;

    [Tooltip("This should be populated from child transform gameObject showing harvest effect spawn point")]
    [SerializeField] private Transform harvestActionEffectTranform = null;

    [HideInInspector]
    public Vector2Int cropGridPosition;

    

    public void ProcessToolAction(ItemDetails equippedItemDetails,bool isToolRight,bool isToolLeft,bool isToolDown,bool isToolUp)
    {
        //get grid property details
        GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

        if (gridPropertyDetails == null)
            return;

        //get seed item itemdetails
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null)
            return;

        //get crop details
        CropDetails cropDetails = GridPropertyManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null)
            return;

        //get animator for crop if present
        Animator animator = GetComponentInChildren<Animator>();

        //trigger tool animation
        if (animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }
            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }

        //trigger tool particle effect on crop
        if(cropDetails.isHarvestActionEffect)
        {
            EventHander.CallHarvestActionEffectEvent(harvestActionEffectTranform.position, cropDetails.harvestActionEffect);
        }    
        //增加收获动作数量
        harvestActionCount += 1;

        //得到对应工具收获所需要的动作数量
        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if (requiredHarvestActions == -1)
            return;//this tool cann't be used to harvest this crop 

        //检查是否采取了必要的收获行动
        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(isToolRight, isToolUp, cropDetails, gridPropertyDetails, animator);
        }

    }

    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp, CropDetails cropDetails, GridPropertyDetails gridPropertyDetails,Animator animator)
    {
        //is there a harvested animation
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            //if harvest sprite then add to sprite renderer
            if (cropDetails.harvestedSprite != null)
            {
                if (cropHarvestedSpriteRender != null)
                {
                    cropHarvestedSpriteRender.sprite = cropDetails.harvestedSprite;
                }
            }

            if (isUsingToolRight || isUsingToolUp)
            {
                animator.SetTrigger("harvestright");
            }
            else
            {
                animator.SetTrigger("harvestleft");
            }
        }

        //playy sounds
        if (cropDetails.harvestSound != SoundName.none)
        {
            AudioManager.Instance.PlaySound(cropDetails.harvestSound);
        }


        //delete crop from grid properties
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        //should the crop hidden before the harvested animation
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        //Should box Colliders be disabled before harvest
        if (cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            //disable box collider
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();

            foreach(Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertyManager.Instance.SetGridPorpertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY,gridPropertyDetails);

        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionsAfterAnimatio(cropDetails, gridPropertyDetails, animator));
        }
        else
        {
            HarvestActionEffect(cropDetails, gridPropertyDetails);
        }
    }

    private IEnumerator ProcessHarvestActionsAfterAnimatio(CropDetails cropDetails,GridPropertyDetails gridPropertyDetails,Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))//0指的是 Animator的 动画层layer，base layer
        {
            yield return null;
        }
        HarvestActions(cropDetails, gridPropertyDetails);
    }

    private void HarvestActions(CropDetails cropDetails,GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);

        //does this tranform into another crop
        if (cropDetails.harvestedTransformItemCode > 0)
        {
            CreatHarvestedTranformCrop(cropDetails, gridPropertyDetails);
        }

        Destroy(gameObject);
    }


    private void HarvestActionEffect(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);

        Destroy(gameObject);
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        //收获作物
        for(int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;

            //计算有多少产物
            if(cropDetails.cropProducedMinQuantity[i]==cropDetails.cropProducedMaxQuantity[i]||
                cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i]);
            }

            for(int j = 0; j < cropsToProduce; j++)
            {
                Vector3 spawnPosition;
                if (cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    //add item to the player inventory
                    InventoryManager.Instance.AddItem(InventoryLocation.player, cropDetails.cropProducedItemCode[i]);
                }
                else
                {
                    //random position
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), 0f);
                    SceneItemManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
                }
            }
        }
    }

    private void CreatHarvestedTranformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        //update crop to property
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertyManager.Instance.SetGridPorpertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //display planted crop
        GridPropertyManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }
}
