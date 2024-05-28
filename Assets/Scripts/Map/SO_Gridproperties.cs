using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//1.ScriptableObject 数据存储在 asset 资源文件中，类似 unity 材质或纹理资源，如果在运行时改变了它的值则就是真的改变了
//2.ScriptableObject 资源在实例化时是被引用，而非像 Prefab 或其他 GameObject 一样是复制，即实际场景中会存在多个 GameObject，所有 ScriptableObject 可以节省 memory
//3.传统保存数据并调用可使用 playerprefs、json、xml，而 ScriptableObject 提供了一种新的更便捷与友好的方法，通过 inspector 可视化的编辑需要存储的数据

[CreateAssetMenu(fileName ="so_GridProperties",menuName ="Scriptable Objects/Grid Properties")]
public class SO_Gridproperties : ScriptableObject
{
    public SceneName sceneName;//Field,Farm,Cabin
    public int gridWidth;//网格宽
    public int gridHeight;
    public int originx;//原始x坐标
    public int originy;//原始y坐标

    [SerializeField]//强制unity去序列化一个私有域。这是一个内部的unity序列化功能，有时候我们需要Serialize一个private或者protected的属性， 这个时候可以使用[SerializeField] 这个Attribute:之后就可以在面板看到该变量。
    public List<GridProperty> gridPropertyList;
}
