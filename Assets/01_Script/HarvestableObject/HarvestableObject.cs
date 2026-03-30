using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class HarvestableObject : MonoBehaviour
{
    public HarvestableSO harvestableSO;
    public int currentHarvestCount;

    protected virtual void Awake()
    {
        if (ObjPositionManager.Instance == null) return;

        ObjPositionManager.Instance.AddHavaObjPosition(this.transform);
    }
    private void Start()
    {
        currentHarvestCount = harvestableSO.harvestCount;
    }
    public virtual void Harvest()
    {
        currentHarvestCount--;
        HarvestEffect();

        if (currentHarvestCount <= 0)
        {
            Debug.Log("채집 완료");
            HarvestDoneEffect();
            if (harvestableSO.harvestedPrefab != null)
            {
                Instantiate(harvestableSO.harvestedPrefab, transform.position, Quaternion.identity);
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
