using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ObjectPool 
{
    private GameObject Parent;
    private PoolableObject prefab;
    private int size;
    private List<PoolableObject> availableObjectsPool;

    private ObjectPool(PoolableObject Prefab, int Size)
    {
        this.prefab = Prefab;
        this.size = Size;
        availableObjectsPool = new List<PoolableObject>(Size);
    }

    public static ObjectPool CreateInstance(PoolableObject Prefab, int Size)
    {
        ObjectPool pool = new ObjectPool(Prefab, Size);

        pool.Parent = new GameObject(Prefab + " Pool");
        pool.CreateObjects();

        return pool;
    }

    private void CreateObjects()
    {
        for (int i = 0; i < size; i++)
        {
            CreateObject();
        }
    }
    private void CreateObject()
    {
        PoolableObject poolableObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, Parent.transform);
        poolableObject.Parent = this;
        poolableObject.gameObject.SetActive(false); // PoolableObject handles re-adding the object to the AvailableObjects
    }

    public PoolableObject GetObject()
    {
        if (availableObjectsPool.Count == 0) 
        {
            CreateObject();
        }
        PoolableObject instance = availableObjectsPool[0];

        availableObjectsPool.RemoveAt(0);

        instance.gameObject.SetActive(true);

        return instance;
    }

    public void ReturnObjectToPool(PoolableObject Object)
    {
        availableObjectsPool.Add(Object);
    }
}
