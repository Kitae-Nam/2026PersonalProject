using Unity.VisualScripting;
using UnityEngine;

namespace _01_Script.Pool
{
    [CreateAssetMenu(fileName = "Pool", menuName = "SO/Pool/PoolItem")]
    public class PoolItem : ScriptableObject
    {
        public string poolName;
        public GameObject prefab;
        public int initialCount;
    }
}
