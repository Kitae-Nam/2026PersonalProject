using UnityEngine;

namespace _01_Script.Item.Realtem
{
    public abstract class EquipmentItem : Item
    {
        [SerializeField] private LayerMask havaLayer;

        public abstract void Use(GameObject harvestObj);
    }
}
