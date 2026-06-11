using Unity.VisualScripting;
using UnityEngine;

namespace _01_Script.Item.Realtem
{
    public abstract class ItemParent : MonoBehaviour, Pool.IPoolable
    {
        public ItemSO itemSo;
        public GameObject itemGo => gameObject;
        [field:SerializeField] public bool isCanCarry { get; protected set; } = true;
        public abstract void CarredItem();
        public abstract void DropedItem();
        public void OnPop()
        {
            
        }

        public void OnPush()
        {
        }
    }
}
