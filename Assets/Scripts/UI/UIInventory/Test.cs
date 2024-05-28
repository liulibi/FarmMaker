using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;




public class Test : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        print("开始拖动");
    }

    public void OnDrag(PointerEventData eventData)
    {
        print("正在拖动");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print("完成拖动");
    }
}
