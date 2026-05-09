using UnityEngine;

namespace _01_Script.Pool
{
    [CreateAssetMenu(fileName = "PoolList", menuName = "SO/Pool/PoolList")]
    public class PoolList : ScriptableObject
    {
        public Pool[] pools;

    }
}
