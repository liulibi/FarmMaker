using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("Tiles & TileMap Regerences")]
    [Header("Options")]
    [SerializeField] private bool observeMovementPenalties = true;

    [Range(0, 20)]
    [SerializeField] private int pathMovementPenalty = 0;
    [Range(0, 20)]
    [SerializeField] private int defaultMovementPenalty = 0;

    private GridNodes gridNodes;
    private Node startNode;
    private Node targetNode;
    private int gridWidth;
    private int gridHeight;
    private int orginX;
    private int orginY;

    private List<Node> openNodeList;
    private HashSet<Node> closeNodeList;

    private bool pathFound = false;

    ///<summary>
    /// 为场景添加NPC移动路径并路径添加到栈中
    /// 如果路径存在返回true,否则返回false
    ///</summary>
    public bool BuildPath(SceneName sceneName, Vector2Int startGeiPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        pathFound = false;

        if (PopulateGridNodesFromPropertiesDictionary(sceneName, startGeiPosition, endGridPosition))
        {
            if (FindShortestPath())
            {
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);

                return true;
            }
        }
        return false;
    }

    //更新栈
    private void UpdatePathOnNPCMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStepStack)
    {
        Node nextNode = targetNode;

        while (nextNode != null)
        {
            NPCMovementStep npcMovementStep = new NPCMovementStep();

            npcMovementStep.sceneName = sceneName;
            npcMovementStep.girdCoordinate = new Vector2Int(nextNode.gridPosition.x + orginX, nextNode.gridPosition.y + orginY);

            npcMovementStepStack.Push(npcMovementStep);

            nextNode = nextNode.parentNode;
        }
    }

    //寻找最短路径
    private bool FindShortestPath()
    {
        // 将起点添加到开放集合中
        openNodeList.Add(startNode);

        // 循环遍历开放集合直到空
        while (openNodeList.Count > 0)
        {
            // 将列表中元素排序
            openNodeList.Sort();

            // 将开放集合中fCost最小的结点设为当前结点
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            // 将当前结点添加到封闭集合中
            closeNodeList.Add(currentNode);

            //如果当前结点为目标点就结束遍历
            if (currentNode == targetNode)
            {
                pathFound = true;
                break;
            }

            // 计算当前结点周围结点的fCost
            EvaluateCurrentNodeNeighbiours(currentNode);
        }

        if (pathFound)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 计算当前结点周围结点的fCost
    private void EvaluateCurrentNodeNeighbiours(Node currentNode)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        //循化遍历当前结点的所有方向
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j);

                if (validNeighbourNode != null)
                {
                    // 更新周围结点的gCost
                    int newCostToNeighbour;

                    if (observeMovementPenalties)
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + validNeighbourNode.movementPenalty;
                    }
                    else
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                    }

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);

                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    // 获取两结点间的距离
    private int  GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        //对角线为14，直线是10
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition)
    {
        //如果相邻结点超过地图大小就返回null
        if (neighbourNodeXPosition >= gridWidth || neighbourNodeXPosition < 0 || neighbourNodeYPosition >= gridHeight || neighbourNodeYPosition < 0)
        {
            return null;
        }

        //如果相邻结点是障碍物或者在封闭集合中就跳过
        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        if (neighbourNode.isObstacle || closeNodeList.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }

    //从网格属性字典中填充网格结点
    private bool PopulateGridNodesFromPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        //得到场景的网格属性字典
        SceneSave sceneSave;

        if (GridPropertyManager.Instance.GameObjectSave.sceneDate.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            //判断是否有良好的网格属性细节字典
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                //得到网格地图的宽和高
                if (GridPropertyManager.Instance.GetGirdDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
                {
                    //基于网格属性字典生成结点网格
                    gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;
                    orginX = gridOrigin.x;
                    orginY = gridOrigin.y;

                    //创建开放集合
                    openNodeList = new List<Node>();

                    //创建封闭集合
                    closeNodeList = new HashSet<Node>();
                }
                else
                {
                    return false;
                }

                //填充起点
                startNode = gridNodes.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);

                //填充终点
                targetNode = gridNodes.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);

                //填充障碍和主路径
                for (int x = 0; x < gridDimensions.x; x++)
                {
                    for (int y = 0; y < gridDimensions.y; y++)
                    {
                        GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y);

                        if (gridPropertyDetails != null)
                        {
                            if (gridPropertyDetails.isNPCObstacle == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.isObstacle = true;
                            }
                            else if (gridPropertyDetails.isPath == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = pathMovementPenalty;
                            }
                            else
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = defaultMovementPenalty;
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}

