using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attach to a crop perfab to set the values in the grid property dictionary
/// </summary>
public class CopInstantiater :MonoBehaviour
{
    private Grid grid;
    [SerializeField] private int daySinceDug = -1;
    [SerializeField] private int daySinceWatered = -1;

    [ItemCodeDescriptionAttributer]
    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growthDays = 0;

    private void OnEnable()
    {
        EventHander.InstantiateCropPerfabsEvent += InstantiateCropPrefabs;
    }
    private void OnDisable()
    {
        EventHander.InstantiateCropPerfabsEvent -= InstantiateCropPrefabs;
    }

    private void InstantiateCropPrefabs()
    {
        //get grid gameObject
        grid = GameObject.FindObjectOfType<Grid>();

        //get grid position for crop
        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        SetCropGridProperties(cropGridPosition);

        Destroy(gameObject);
    }

    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {
        if (seedItemCode > 0)
        {
            GridPropertyDetails gridPropertyDetails;

            gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

            if (gridPropertyDetails == null)
            {
                gridPropertyDetails = new GridPropertyDetails();
            }

            gridPropertyDetails.daysSinceDug = daySinceDug;
            gridPropertyDetails.daysSinceWatered = daySinceWatered;
            gridPropertyDetails.seedItemCode = seedItemCode;
            gridPropertyDetails.growthDays = growthDays;


            GridPropertyManager.Instance.SetGridPorpertyDetails(cropGridPosition.x, cropGridPosition.y, gridPropertyDetails);
        }
    }
}
