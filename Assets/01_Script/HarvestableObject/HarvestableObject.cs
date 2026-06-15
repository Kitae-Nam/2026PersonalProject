using System.Collections;
using System.Collections.Generic;
using _01_Script.Managers;
using _01_Script.Pool;
using UnityEngine;

namespace _01_Script.HarvestableObject
{
    public abstract class HarvestableObject : MonoBehaviour, IPoolable
    {
        public HarvestableSo harvestableSo;
        [SerializeField] private float delay = 0.1f;
        [SerializeField] private string effectName;
        [SerializeField] private Transform point;
        public int currentHarvestCount;

        [SerializeField] private List<Vector3> scales;

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
            if (currentHarvestCount == 0) yield break;
            --currentHarvestCount;
            Debug.Log("아야");

            if (currentHarvestCount <= 0)
            {
                yield return new WaitForSeconds(delay);
                gameObject.transform.localScale = scales[currentHarvestCount];
                HarvestDoneEffect();
                if (harvestableSo.harvestingName != null)
                {
                    PoolManager.Instance.Spawn(harvestableSo.harvestingName, transform.position + new Vector3(0,1,0), Quaternion.identity);
                }

                Destroy(gameObject);
            }
            else
            {
                yield return new WaitForSeconds(delay);
                gameObject.transform.localScale = scales[currentHarvestCount];
                HarvestEffect();
            }
            if (currentHarvestCount == 0)
            {
                PoolManager.Instance.Despawn(this.gameObject);
                yield break;
            }
        }
        public virtual void HarvestEffect()
        {
            PoolManager.Instance.Spawn(effectName, point.position, Quaternion.identity);
        }
        public virtual void HarvestDoneEffect()
        {
            PoolManager.Instance.Spawn(effectName, point.position, Quaternion.identity);
        }

        public void OnPop()
        {
        }

        public void OnPush()
        {
            currentHarvestCount = harvestableSo.harvestCount;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
