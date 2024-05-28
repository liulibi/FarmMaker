[System.Serializable]
public class GridProperty//网格属性
{
    public GridCoordinate gridCoordinate;//网格坐标
    public GridBoolProperty gridBoolProperty;//用途 
    public bool gridBoolValue = false;

    public GridProperty(GridCoordinate gridCoordinate,GridBoolProperty gridBoolProperty,bool gridBoolValue)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridBoolProperty = gridBoolProperty;
        this.gridBoolValue = gridBoolValue;
    }
}