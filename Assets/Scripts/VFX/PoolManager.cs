using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager :SingletonMonobehaviour<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
    [SerializeField] private Pool[] pool = null;
    [SerializeField] private Transform objectPoolTransform = null;

    [System.Serializable]

    public struct Pool
    {
        public int poolSize;
        public GameObject perfab;
    }

    private void Start()
    {
        //creat object pools on start 
        for (int i=0; i < pool.Length; i++)
        {
            CreatePool(pool[i].perfab, pool[i].poolSize);
        }
    }

    private void CreatePool(GameObject perfab, int poolSize)
    {
        int poolKey = perfab.GetInstanceID();//在场景中的每一个实例化出来的对象（实体GameObject、组件等），都有一个唯一固定且各不不同的instanceID。GetInstanceID便可以得到此ID。
        string perfabName = perfab.name;

        //creat parent gameobject to parent the child object to
        GameObject parentGameObject = new GameObject(perfabName + "Anchor");//Anchor锚

        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());

            for(int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(perfab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false);

                poolDictionary[poolKey].Enqueue(newObject);//Queue.Enqueue()方法用于在Queue的末尾添加一个对象/元素。
            }
        }
    }

    public GameObject ReuseObject(GameObject perfab,Vector3 position,Quaternion rotation)
    {
        int poolKey = perfab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            //get object from pool queue
            GameObject objectToReuse = GetObjectFromPool(poolKey);

            ResetObjcet(position, rotation, objectToReuse, perfab);

            return objectToReuse;
        }
        else
        {
            Debug.Log("No object pool for " + perfab);
            return null;
        }
    }

    private GameObject GetObjectFromPool(int poolKey)
    {
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue();//用于从Queue的开头删除一个对象/元素，并返回该对象/元素。
        poolDictionary[poolKey].Enqueue(objectToReuse);//用于在Queue的末尾添加一个对象/元素。

        if (objectToReuse.activeSelf == true)
        {
            objectToReuse.SetActive(false);
        }

        return objectToReuse;

    }

    private static void ResetObjcet(Vector3 position,Quaternion rotation,GameObject objectToReuse,GameObject perfab)
    {
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;

        objectToReuse.transform.localScale = perfab.transform.localScale;//可以用来对某个GameObject进行放大和缩小
    }
}
