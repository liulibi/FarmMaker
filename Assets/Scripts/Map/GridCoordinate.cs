using UnityEngine;

[System.Serializable]//序列化是指将对象实例的状态存储到存储媒体的过程。在此过程中，先将对象的公共字段和私有字段以及类的名称（包括类所在的程序集）转换为字节流，然后再把字节流写入数据流。在随后对对象进行反序列化时，将创建出与原对象完全相同的副本。


public class GridCoordinate//网格坐标
{
    public int x;
    public int y;
    
    public GridCoordinate(int p1,int p2)
    {
        x = p1;
        y = p2;
    }
    //explicit它的作用是表明该构造函数是显示的, 而非隐式的, 跟它相对应的另一个关键字是implicit, 意思是隐藏的,类构造函数默认情况下即声明为implicit(隐式).
    public static explicit operator Vector2(GridCoordinate gridCoordinate)//返回2向量float
    {
        return new Vector2((float)gridCoordinate.x, (float)gridCoordinate.y);
    }

    public static explicit operator Vector2Int(GridCoordinate gridCoordinate)//返回2向量int
    {
        return new Vector2Int(gridCoordinate.x, gridCoordinate.y);
    }

    public static explicit operator Vector3(GridCoordinate gridCoordinate)//返回3向量float
    {
        return new Vector3((float)gridCoordinate.x, (float)gridCoordinate.y, 0f);
    }

    public static explicit operator Vector3Int(GridCoordinate gridCoordinate)//返回3向量int
    {
        return new Vector3Int(gridCoordinate.x, gridCoordinate.y, 0);
    }
}