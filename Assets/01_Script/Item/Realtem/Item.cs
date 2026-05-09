using UnityEngine;

namespace _01_Script.Item.Realtem
{
    public abstract class Item : MonoBehaviour
    {
        public ItemSO itemSo;
        public GameObject itemGo => gameObject;
        public bool isCanCarry { get; protected set; } = true;
    }
}
