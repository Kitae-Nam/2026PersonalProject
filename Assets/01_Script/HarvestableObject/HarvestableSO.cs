using UnityEngine;

[CreateAssetMenu(fileName = "HarvestableSO", menuName = "SO/Harvestable/HarvestableSO")]
public class HarvestableSO : ScriptableObject
{
    public int harvestCount = 3;
    public GameObject harvestedPrefab;
   
}
