using UnityEngine;

public abstract class EquipmentItem : Item
{
    [SerializeField] private LayerMask _havaLayer;

    public abstract void Use(GameObject harvestObj);
}
