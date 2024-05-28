using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundShape : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        EventHander.AfterSceneLoadEvent += SwitchBoundingShape;
    }

    private void OnDisable()
    {
        EventHander.AfterSceneLoadEvent -= SwitchBoundingShape;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 选择碰撞体去确定当前视野边界
    /// </summary>
    private void SwitchBoundingShape()
    {
        //首先要获取到要的多边形碰撞体
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();

        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        //边界被改变了，所以要清除一下内存
        cinemachineConfiner.InvalidatePathCache();
    }
}
