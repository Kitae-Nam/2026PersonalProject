using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "SO/Item/ItemSO")]
public class ItemSO : ScriptableObject
{
    public ItemType _itemType;
    public EquipmentType _equipmentType;
}
