using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;


[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR
    private Tilemap tilemap;
    [SerializeField] private SO_Gridproperties gridproperties = null;//
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable;//

    private void Update()//在没有启用的时候发出警告
    {
        //only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            Debug.Log("启用属性磁贴图");
        }
    }

    private void OnEnable()//开始使用的时候清空SO_Gridproperties gridproperties的信息
    {
        //only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();

            if (gridproperties != null)
            {
                gridproperties.gridPropertyList.Clear();//列表清空
            }
        }
    }

    private void OnDisable()//关闭使用的时候记录下当前tilemap的信息
    {
        //only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();

            if (gridproperties != null)
            {
               EditorUtility.SetDirty(gridproperties);
            }
        }
    }

    private void UpdateGridProperties()//更新SO_Gridproperties gridproperties的信息
    {
        tilemap.CompressBounds();//压缩边界

        if (!Application.IsPlaying(gameObject))
        {
            if (gridproperties != null)
            {
                Vector3Int startCell = tilemap.cellBounds.min;
                Vector3Int endCell = tilemap.cellBounds.max;

                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for(int y = startCell.y; y < endCell.y; y++)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0)); 
                        if (tile != null)
                        {
                            gridproperties.gridPropertyList.Add(new GridProperty(new GridCoordinate(x,y),gridBoolProperty,true));//参数：网格坐标，用途，真值
                        }
                    }
                }
            }
        }
    }
#endif
}
