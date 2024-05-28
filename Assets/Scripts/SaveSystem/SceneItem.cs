[System.Serializable]

public class SceneItem
{
    public int itemCode;
    public Verctor3Serializable position;
    public string itemName;

    public SceneItem()
    {
        position = new Verctor3Serializable();
    }
}