using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(AStar))]
public class AStarTest : MonoBehaviour
{
    private AStar aStar;
    [SerializeField] private Vector2Int startPosition;
    [SerializeField] private Vector2Int finishPosition;
    [SerializeField] private Tilemap tilemapToDisplayPathOn = null;
    [SerializeField] private TileBase tileToUseToDisplayPath = null;
    [SerializeField] private bool displayStarAndFinish = false;
    [SerializeField] private bool displayPath = false;

    private Stack<NPCMovementStep> npcMovementSteps;

    private void Awake()
    {
        aStar = GetComponent<AStar>();

        npcMovementSteps = new Stack<NPCMovementStep>();
    }


    private void Update()
    {
        if (startPosition != null && finishPosition != null && tilemapToDisplayPathOn != null && tileToUseToDisplayPath != null)
        {
            //Display start and finish tiles
            if (displayStarAndFinish)
            {
                //Display start tile
                tilemapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), tileToUseToDisplayPath);

                //Display finish tile
                tilemapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0), tileToUseToDisplayPath);
            }
            else
            //Clear start and finish tiles
            {
                //Clear start tile
                tilemapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), null);

                //Clear finish tile
                tilemapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0), null);
            }

            //Display Path
            if (displayPath)
            {
                //Get current scene name
                Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, out SceneName sceneName);

                //Build path
                aStar.BuildPath(sceneName, startPosition, finishPosition, npcMovementSteps);

                //Display path on tilemap
                foreach (NPCMovementStep npcMovementStep in npcMovementSteps)
                {
                    tilemapToDisplayPathOn.SetTile(new Vector3Int(npcMovementStep.girdCoordinate.x, npcMovementStep.girdCoordinate.y, 0), tileToUseToDisplayPath);
                }
            }
            else
            {
                // Clear path
                if (npcMovementSteps.Count > 0)
                {
                    //Clear path on tilemap
                    foreach (NPCMovementStep npcMovementStep in npcMovementSteps)
                    {
                        tilemapToDisplayPathOn.SetTile(new Vector3Int(npcMovementStep.girdCoordinate.x, npcMovementStep.girdCoordinate.y, 0), null);
                    }

                    //Clear movement steps
                    npcMovementSteps.Clear();
                }
            }
        }
    }
}
