using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField] private PoolList _poolList;
    private Dictionary<string, Queue<GameObject>> _poolDictionary;

    private void Start()
    {
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();
        for (int i = 0; i < _poolList.pools.Length; i++)
        {
            Pool pool = _poolList.pools[i];
            Queue<GameObject> objectQueue = new Queue<GameObject>();
            for (int j = 0; j < pool.count; j++)
            {
                GameObject poolObj = Instantiate(pool.prefab);
                objectQueue.Enqueue(poolObj);
                poolObj.name = pool.prefab.name;
                poolObj.SetActive(false);
            }
            _poolDictionary.Add(pool.prefab.name, objectQueue);
        }
    }

    public GameObject Pop(string name)
    {
        if (!_poolDictionary.ContainsKey(name))
        {
            Debug.Assert(false, $"PoolManager: Pop - {name} is not exists.");
            return null;
        }

        foreach (var pool in _poolDictionary)
        {
            if (pool.Key == name)
            {
                if (pool.Value.Count > 0)
                {
                    GameObject obj = pool.Value.Dequeue();
                    obj.SetActive(true);
                    return obj;
                }
                else
                {
                    Debug.Assert(false, $"PoolManager: Pop - {name} is empty.");
                    return null;
                }
            }
        }
        return null;
    }
    public void Push(GameObject obj)
    {
        if (!_poolDictionary.ContainsKey(obj.name))
        {
            Debug.Assert(false, $"PoolManager: Push - {obj.name} is not exists.");
            return;
        }
        _poolDictionary[obj.name].Enqueue(obj);
        obj.SetActive(false);
    }
}
