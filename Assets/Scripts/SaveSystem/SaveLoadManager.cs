using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadManager :SingletonMonobehaviour<SaveLoadManager>
{
    public GameSave gameSave;
    public List<ISaveable> iSaveableObjectlist;//可以保存的gameobject，其都存在ISaveable接口。列如：TimeManager,NPCManager,PlayerManager,SceneItemsManager

    protected override void Awake()
    {
        base.Awake();

        iSaveableObjectlist = new List<ISaveable>();
    }

    private void OnEnable()
    {
    }

    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            gameSave = new GameSave();

            FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);

            gameSave = (GameSave)bf.Deserialize(file);

            //循环遍历所有ISaveable对象并应用保存数据
            for (int i = iSaveableObjectlist.Count - 1; i > -1; i--)
            {
                if (gameSave.gameObjectDate.ContainsKey(iSaveableObjectlist[i].ISaveableUniqueID))
                {
                    iSaveableObjectlist[i].ISaveableLoad(gameSave);
                }
                //如果iSaveableObject唯一ID不在游戏对象数据中，则销毁对象
                else
                {
                    Component component = (Component)iSaveableObjectlist[i];
                    Destroy(component.gameObject);
                }
            }
            file.Close();
        }

        UIManager.Instance.DisablePauseMenu();
    }

    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        //循环遍历所有ISaveable对象并生成保存数据
        foreach (ISaveable iSaveableObject in iSaveableObjectlist)
        {
            gameSave.gameObjectDate.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());

        }

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);

        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            bf.Serialize(file, gameSave);
        }

        

        file.Close();

        UIManager.Instance.DisablePauseMenu();
    }

    public void StoreCurrentSceneData() 
    {
        //循环遍历所有 ISaveable 对象并触发每个对象的储存场景数据
        foreach (ISaveable iSaveableObject in iSaveableObjectlist)
        {
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        //循环遍历所有 ISaveable 对象并触发每个对象的还原场景数据
        foreach (ISaveable iSaveableObjct in iSaveableObjectlist)
        {
            iSaveableObjct.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
