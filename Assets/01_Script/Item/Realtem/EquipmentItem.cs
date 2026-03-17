using UnityEngine;

public abstract class EquipmentItem : Item
{
    public float _useRange;
    public Vector3 _useOffset;
    [SerializeField] private LayerMask _havaLayer;

    
}
