using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods 
{
    /// <summary>
    /// 获取框中具有中心点、大小和角度的 T 类型的组件。如果列表中至少返回一个找到和找到的组件，则返回 true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listComponentsAtBoxPosition"></param>
    /// <param name="point"></param>
    /// <param name="size"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition,Vector2 point,Vector2 size,float angle)//angle角度
    {
        bool found = false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2DArry = Physics2D.OverlapBoxAll(point, size, angle);

        //遍历collider2DArry的到类型为T的物品
        for(int i = 0; i < collider2DArry.Length; i++)
        {
            T tComponent = collider2DArry[i].gameObject.GetComponentInParent<T>();

            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArry[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }
        listComponentsAtBoxPosition = componentList;

        return found;
    }


    /// <summary>
    /// 在positionTocheck处获取T类型的组件，如果至少找到一个，则返回true，找到的组件将在componentAtpositionlist中返回。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="componentsAtPositionList"></param>
    /// <param name="positionToCheck"></param>
    /// <returns></returns>
    public static bool GetComponentsAtCursorLocation<T>(out List<T> componentsAtPositionList,Vector3 positionToCheck)
    {
        bool found = false;

        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck);//获取与空间中的某个点重叠的所有碰撞体的列表。 该函数类似于 OverlapPoint，不同之处在于其返回与该点重叠的所有碰撞体。返回的数组中的碰撞体按 Z 坐标增大的顺序排序。如果该点上没有任何碰撞体，则返回一个空数组。

        //遍历所有的collider得到T类型

        T tComponent = default(T);

        for(int i = 0; i < collider2DArray.Length; i++)
        {
            tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent=collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        componentsAtPositionList = componentList;

        return found;
    }

    /// <summary>
    /// 返回中心点、大小和角度的T型箱体组件数组。 测试的碰撞数被作为一个参数传递。被发现的组件在数组中返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="numberOfCollidersToTest"></param>
    /// <param name="point"></param>
    /// <param name="size"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static T[] GetComponentAtBoxLocationNonAlloc<T>(int numberOfCollidersToTest,Vector2 point,Vector2 size,float angle)
    {
        Collider2D[] collider2DArray = new Collider2D[numberOfCollidersToTest];

        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArray);//获取位于指定盒形区域内的所有碰撞体的列表。此函数类似于 OverlapBoxAll，不同之处在于其结果会返回到提供的数组中。整数返回值是位于该盒体内的对象数（可能为零），但如果结果数组中没有足够的元素来报告所有这些结果，则不会调整该数组的大小。其意义在于不为这些结果分配内存，因此，如果经常进行检查，可以提高垃圾回收性能。

        T tComponent = default(T);

        T[] componentArray = new T[collider2DArray.Length];
       
        for(int i = collider2DArray.Length - 1; i >= 0; i--)
        {
            if (collider2DArray[i] != null)
            {
                tComponent = collider2DArray[i].gameObject.GetComponent<T>();
            }
            if (tComponent != null)
            {
                componentArray[i] = tComponent;
            }
        }
        return componentArray;
    }
    
}
