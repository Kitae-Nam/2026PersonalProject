using _01_Script.Managers;
using UnityEngine;

namespace _01_Script.HarvestableObject
{
    public abstract class HarvestableObject : MonoBehaviour
    {
        public HarvestableSo harvestableSo;
        public int currentHarvestCount;

        protected virtual void Awake()
        {
            if (ObjPositionManager.Instance == null) return;

            ObjPositionManager.Instance.AddHavaObjPosition(this.transform);
        }
        private void Start()
        {
            currentHarvestCount = harvestableSo.harvestCount;
        }
        public virtual void Harvest()
        {
            currentHarvestCount--;
            HarvestEffect();

            if (currentHarvestCount <= 0)
            {
                Debugging.Log("채집됨");
                HarvestDoneEffect();
                if (harvestableSo.harvestedPrefab != null)
                {
                    Instantiate(harvestableSo.harvestedPrefab, transform.position, Quaternion.identity);
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
}
