using System.Collections.Generic;

[System.Serializable]

public class GameObjectSave
{
    //string key= scene name
    public Dictionary<string, SceneSave> sceneDate;

    public GameObjectSave()
    {
        sceneDate = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneDate)//string is sceneName
    {
        this.sceneDate = sceneDate;
    }
}