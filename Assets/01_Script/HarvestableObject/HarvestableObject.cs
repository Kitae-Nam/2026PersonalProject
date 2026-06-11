using System.Collections;
using _01_Script.Managers;
using UnityEngine;

namespace _01_Script.HarvestableObject
{
    public abstract class HarvestableObject : MonoBehaviour
    {
        public HarvestableSo harvestableSo;
        [SerializeField] private float delay = 0.1f; 
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
        public virtual IEnumerator Harvest()
        {
            currentHarvestCount--;
            HarvestEffect();
            Debug.Log("아야");

            if (currentHarvestCount <= 0)
            {
                HarvestDoneEffect();
                if (harvestableSo.harvestedPrefab != null)
                {
                    yield return new WaitForSeconds(delay);
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
