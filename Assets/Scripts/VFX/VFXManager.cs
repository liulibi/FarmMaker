using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : SingletonMonobehaviour<VFXManager>
{
    private WaitForSeconds twoSeconds;
    [SerializeField] private GameObject reapingPerfab = null;
    [SerializeField] private GameObject deciduousLeavesFallingPerfab = null;
    [SerializeField] private GameObject choppingTreeTrunkPrefab = null;
    [SerializeField] private GameObject pineConesFallingPerfab = null;
    [SerializeField] private GameObject breakingStonePerfab = null;


    protected override void Awake()
    {
        base.Awake();

        twoSeconds = new WaitForSeconds(2f);
    }

    private void OnDisable()
    {
        EventHander.HarvestActionEffectEvent -= displayHarvestActionEffect;
    }

    private void OnEnable()
    {
        EventHander.HarvestActionEffectEvent += displayHarvestActionEffect;
    }

    private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject,WaitForSeconds secondsToWait) 
    {
        yield return twoSeconds;
        effectGameObject.SetActive(false);
    }

    private void displayHarvestActionEffect(Vector3 effectPosition,HarvestActionEffect harvestActionEffect)
    {
        if (harvestActionEffect == HarvestActionEffect.reaping)
        {
            GameObject reaping = PoolManager.Instance.ReuseObject(reapingPerfab, effectPosition, Quaternion.identity);
            reaping.SetActive(true);
            StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
        }
        else if (harvestActionEffect == HarvestActionEffect.deciduousLeavesFalling)
        {
            GameObject deciuousLeavesFalling = PoolManager.Instance.ReuseObject(deciduousLeavesFallingPerfab, effectPosition, Quaternion.identity);
            deciuousLeavesFalling.SetActive(true);
            StartCoroutine(DisableHarvestActionEffect(deciuousLeavesFalling, twoSeconds));
        }
        else if (harvestActionEffect == HarvestActionEffect.choppingTreeTrunk)
        {
            GameObject choppingTreeTrunk = PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, effectPosition, Quaternion.identity);
            choppingTreeTrunk.SetActive(true);
            StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, twoSeconds));
        }
        else if (harvestActionEffect == HarvestActionEffect.pineConesFalling)
        {
            GameObject pineConesFaling = PoolManager.Instance.ReuseObject(pineConesFallingPerfab, effectPosition, Quaternion.identity);
            pineConesFaling.SetActive(true);
            StartCoroutine(DisableHarvestActionEffect(pineConesFaling, twoSeconds));
        }
        else if (harvestActionEffect == HarvestActionEffect.breakingStone)
        {
            GameObject breakingStone = PoolManager.Instance.ReuseObject(breakingStonePerfab, effectPosition, Quaternion.identity);
            breakingStone.SetActive(true);
            StartCoroutine(DisableHarvestActionEffect(breakingStone, twoSeconds));
        }
    }

}
