using System.Collections.Generic;
using _01_Script.Managers;
using UnityEngine;

namespace _01_Script.Pool
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        [SerializeField] private PoolList poolList;

        private Dictionary<string, Queue<GameObject>> _poolDictionary = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<GameObject, string> _activeObjects = new Dictionary<GameObject, string>();

        protected override void Awake()
        {
            base.Awake();
            InitializePools();
        }

        private void InitializePools()
        {
            if (poolList == null || poolList.pools == null) return;

            foreach (var pool in poolList.pools)
            {
                if (pool.prefab == null) continue;

                if (string.IsNullOrEmpty(pool.poolName))
                    pool.poolName = pool.prefab.name;

                _poolDictionary[pool.poolName] = new Queue<GameObject>();

                GameObject poolParent = new GameObject($"Pool_{pool.poolName}");
                poolParent.transform.SetParent(this.transform);

                for (int i = 0; i < pool.initialCount; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, poolParent.transform);
                    obj.SetActive(false);
                    _poolDictionary[pool.poolName].Enqueue(obj);
                }
            }
        }

        public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(poolName))
            {
                Debug.LogWarning($"[PoolManager] {poolName} 풀이 리스트에 존재하지 않습니다.");
                return null;
            }

            GameObject obj;

            if (_poolDictionary[poolName].Count > 0)
            {
                obj = _poolDictionary[poolName].Dequeue();
            }
            else
            {
                // 리스트에서 매칭되는 원본 Pool 데이터를 찾아 동적 확장
                PoolItem targetPool = poolList.pools.Find(p => p.poolName == poolName);
                obj = Instantiate(targetPool.prefab);
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnPop();
            }

            _activeObjects[obj] = poolName;
            return obj;
        }

        public void Despawn(GameObject obj)
        {
            if (obj == null) return;

            if (!_activeObjects.ContainsKey(obj))
            {
                Debug.LogWarning($"[PoolManager] 풀을 통해 생성되지 않은 오브젝트({obj.name})입니다. Destroy 합니다.");
                Destroy(obj);
                return;
            }

            string poolName = _activeObjects[obj];
            _activeObjects.Remove(obj);

            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnPush();
            }

            obj.SetActive(false);

            Transform parentFolder = transform.Find($"Pool_{poolName}");
            if (parentFolder != null) obj.transform.SetParent(parentFolder);

            _poolDictionary[poolName].Enqueue(obj);
        }
    }
}
