using UnityEngine;

public class PickaxeItemScript : EquipmentItem
{
    public override void Use(GameObject harvestObj)
    {
        if (harvestObj.TryGetComponent<HarvestableObject>(out HarvestableObject havaCompo))
        {
            havaCompo.Harvest();
        }
    }
}
