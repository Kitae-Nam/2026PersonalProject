using System;
using UnityEngine;

namespace _01_Script.Item.Realtem
{
    public class PickaxeItemParentScript : EquipmentItemParent
    {
        private void Start()
        {
            
        }

        public override void Use(GameObject harvestObj)
        {
            if (harvestObj.TryGetComponent<HarvestableObject.HarvestableObject>(out var havaCompo))
            {
                havaCompo.Harvest();
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
