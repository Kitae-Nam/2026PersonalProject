using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "SO/Item/ItemSO")]
public class ItemSO : ScriptableObject
{
    public ItemType itemType;
    public MaterialType materialType; 
    public EquipmentType equipmentType;

    public Vector3 itemCarryPos;
    public Vector3 additionalCarryPos;
    public Vector3 onGroundPos;
    public Vector3 onGroundAdditionalPos;
}
