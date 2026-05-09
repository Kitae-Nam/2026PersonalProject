using UnityEngine;

namespace _01_Script.Pool
{
    [CreateAssetMenu(fileName = "Pool", menuName = "SO/Pool/Pool")]
    public class Pool : ScriptableObject
    {
        public GameObject prefab;
        public int count;
    }
}
