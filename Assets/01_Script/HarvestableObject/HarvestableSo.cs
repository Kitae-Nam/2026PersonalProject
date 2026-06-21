using UnityEngine;

namespace _01_Script.HarvestableObject
{
    [CreateAssetMenu(fileName = "HarvestableSO", menuName = "SO/Harvestable/HarvestableSO")]
    public class HarvestableSo : ScriptableObject
    {
        public int harvestCount = 3;
        public int hitCount = 1;
        public string harvestingName;
    }
}
