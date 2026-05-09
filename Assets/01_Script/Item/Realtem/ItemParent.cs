using UnityEngine;

namespace _01_Script.Item.Realtem
{
    public abstract class ItemParent : MonoBehaviour
    {
        public ItemSO itemSo;
        public GameObject itemGo => gameObject;
        public bool isCanCarry { get; protected set; } = true;
        public abstract void CarredItem();
        public abstract void DropedItem();
    }
}
