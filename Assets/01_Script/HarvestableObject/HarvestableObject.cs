using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class HarvestableObject : MonoBehaviour
{
    public HarvestableSO harvestableSO;
    public int _currentHarvestCount;

    protected virtual void Awake()
    {
        if (ObjPositionManager.Instance == null) return;

        ObjPositionManager.Instance.AddHavaObjPosition(this.transform);
    }
    private void Start()
    {
        _currentHarvestCount = harvestableSO._harvestCount;
    }
    public virtual void Harvest()
    {
        _currentHarvestCount--;
        HarvestEffect();

        if (_currentHarvestCount <= 0)
        {
            Debug.Log("채집 완료");
            HarvestDoneEffect();
            if (harvestableSO._harvestedPrefab != null)
            {
                Instantiate(harvestableSO._harvestedPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
    public virtual void HarvestEffect()
    {
    }
    public virtual void HarvestDoneEffect()
    {
    }
}
