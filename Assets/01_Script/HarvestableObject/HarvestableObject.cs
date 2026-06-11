using System.Collections;
using _01_Script.Managers;
using _01_Script.Pool;
using UnityEngine;

namespace _01_Script.HarvestableObject
{
    public abstract class HarvestableObject : MonoBehaviour, IPoolable
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
            Debug.Log("아야");

            if (currentHarvestCount <= 0)
            {
                yield return new WaitForSeconds(delay);
                HarvestDoneEffect();
                if (harvestableSo.harvestedPrefab != null)
                {
                    Instantiate(harvestableSo.harvestedPrefab, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
            else
            {
                yield return new WaitForSeconds(delay);
                HarvestEffect();
            }
        }
        public virtual void HarvestEffect()
        {
        }
        public virtual void HarvestDoneEffect()
        {
        }

        public void OnPop()
        {
        }

        public void OnPush()
        {
        }
    }
}
