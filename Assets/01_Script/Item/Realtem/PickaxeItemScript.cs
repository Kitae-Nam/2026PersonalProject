using UnityEngine;

namespace _01_Script.Item.Realtem
{
    public class PickaxeItemScript : EquipmentItem
    {
        public override void Use(GameObject harvestObj)
        {
            if (harvestObj.TryGetComponent<HarvestableObject.HarvestableObject>(out HarvestableObject.HarvestableObject havaCompo))
            {
                havaCompo.Harvest();
            }
        }
    }
}
