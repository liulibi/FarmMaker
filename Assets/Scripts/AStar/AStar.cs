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
    /// Ϊ�������NPC�ƶ�·����·����ӵ�ջ��
    /// ���·�����ڷ���true,���򷵻�false
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

    //����ջ
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

    //Ѱ�����·��
    private bool FindShortestPath()
    {
        // �������ӵ����ż�����
        openNodeList.Add(startNode);

        // ѭ���������ż���ֱ����
        while (openNodeList.Count > 0)
        {
            // ���б���Ԫ������
            openNodeList.Sort();

            // �����ż�����fCost��С�Ľ����Ϊ��ǰ���
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            // ����ǰ�����ӵ���ռ�����
            closeNodeList.Add(currentNode);

            //�����ǰ���ΪĿ���ͽ�������
            if (currentNode == targetNode)
            {
                pathFound = true;
                break;
            }

            // ���㵱ǰ�����Χ����fCost
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

    // ���㵱ǰ�����Χ����fCost
    private void EvaluateCurrentNodeNeighbiours(Node currentNode)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        //ѭ��������ǰ�������з���
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
                    // ������Χ����gCost
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

    // ��ȡ������ľ���
    private int  GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        //�Խ���Ϊ14��ֱ����10
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition)
    {
        //������ڽ�㳬����ͼ��С�ͷ���null
        if (neighbourNodeXPosition >= gridWidth || neighbourNodeXPosition < 0 || neighbourNodeYPosition >= gridHeight || neighbourNodeYPosition < 0)
        {
            return null;
        }

        //������ڽ�����ϰ�������ڷ�ռ����о�����
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

    //�����������ֵ������������
    private bool PopulateGridNodesFromPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        //�õ����������������ֵ�
        SceneSave sceneSave;

        if (GridPropertyManager.Instance.GameObjectSave.sceneDate.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            //�ж��Ƿ������õ���������ϸ���ֵ�
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                //�õ������ͼ�Ŀ�͸�
                if (GridPropertyManager.Instance.GetGirdDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
                {
                    //�������������ֵ����ɽ������
                    gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;
                    orginX = gridOrigin.x;
                    orginY = gridOrigin.y;

                    //�������ż���
                    openNodeList = new List<Node>();

                    //������ռ���
                    closeNodeList = new HashSet<Node>();
                }
                else
                {
                    return false;
                }

                //������
                startNode = gridNodes.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);

                //����յ�
                targetNode = gridNodes.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);

                //����ϰ�����·��
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

