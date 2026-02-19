using UnityEngine;

public abstract class HarvestableObject : MonoBehaviour
{
    public HarvestableSO harvestableSO;
    public int _currentHarvestCount;

    private void Start()
    {
        _currentHarvestCount = harvestableSO._harvestCount;
    }
    public virtual void Harvest()
    {
        _currentHarvestCount--;

        if (_currentHarvestCount <= 0)
        {
            Debug.Log("채집 완료");
            //Destroy(gameObject);
            //if (harvestableSO._harvestedPrefab != null)
            //{
            //    Instantiate(harvestableSO._harvestedPrefab, transform.position, Quaternion.identity);
            //}
        }
    }
}
