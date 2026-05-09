using UnityEngine;

namespace _01_Script.Item.Realtem
{
    public abstract class EquipmentItemParent : ItemParent
    {
        [SerializeField] private LayerMask havaLayer;

        public abstract void Use(GameObject harvestObj);
    }
}
