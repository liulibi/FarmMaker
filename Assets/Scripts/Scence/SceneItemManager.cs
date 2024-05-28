using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GenerateGUID))]
public class SceneItemManager : SingletonMonobehaviour<SceneItemManager>,ISaveable
{
    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;

    private string _iSaveableUniqueID;

    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; }}

    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTranform).transform;
    }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void DestroySceneItems()
    {
        //get all item in the scene
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        //loop through all scene items and destroy them
        for(int i = itemsInScene.Length - 1; i > -1; i--)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHander.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHander.AfterSceneLoadEvent += AfterSceneLoad;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectlist.Add(this);
    }

    public GameObjectSave ISaveableSave()
    {
        //储存当前场景信息
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectlist.Remove(this);
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectDate.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            //重新储存当前场景信息
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //Remove old scene save for gameObject if exists
        GameObjectSave.sceneDate.Remove(sceneName);

        //Get all items in the scene
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemInScene = FindObjectsOfType<Item>();

        //loop through all scene items
        foreach (Item item in itemInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Verctor3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;
            sceneItemList.Add(sceneItem);
        }

        //在场景保存中创建列表场景项并设置为场景项列表
        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneITem=sceneItemList;

        GameObjectSave.sceneDate.Add(sceneName, sceneSave);
    }


    public void ISaveableRestoreScene(string sceneName)
    {
        if(GameObjectSave.sceneDate.TryGetValue(sceneName,out SceneSave sceneSave))
        {
            if(sceneSave.listSceneITem!=null)
            {
                DestroySceneItems();

                InstantiateSceneItems(sceneSave.listSceneITem);
            }
        }
    }

    public void InstantiateSceneItem(int itemCode,Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }


    public void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;

        foreach(SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z),Quaternion.identity,parentItem);

            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
            item.name = sceneItem.itemName;
        }
    }
}
