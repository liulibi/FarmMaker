using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSave 
{
    public Dictionary<string, GameObjectSave> gameObjectDate;

    public GameSave()
    {
        gameObjectDate = new Dictionary<string, GameObjectSave>();
    }

}
