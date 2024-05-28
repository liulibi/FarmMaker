using System.Collections.Generic;

[System.Serializable]
public class SceneSave 
{   //string key is an identifier name we choose for this list
    public Dictionary<string, int> intDictonary;
    public Dictionary<string, bool> boolDictionary;
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, Verctor3Serializable> vector3Dictionary;
    public Dictionary<string, int[]> intArrayDictionary;
    //字符串键是我们为此列表选择的标识符名称
    public List<SceneItem> listSceneITem;

    //key is gridx+gridy
    public Dictionary<string,GridPropertyDetails> gridPropertyDetailsDictionary;
    public List<InventoryItem>[] listInvItemArray;
}
