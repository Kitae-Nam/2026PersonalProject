using System;
using UnityEngine;

namespace _01_Script.Item.Realtem
{
    public class PickaxeItemParentScript : EquipmentItemParent
    {
        public override void Use(GameObject harvestObj)
        {
            if (harvestObj.TryGetComponent<HarvestableObject.HarvestableObject>(out var havaCompo))
            {
                StartCoroutine(havaCompo.Harvest());
            }
        }

        public override void CarredItem()
        {
            
        }

        public override void DropedItem()
        {
            
        }
    }
}
