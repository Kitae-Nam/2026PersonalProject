using _01_Script.Item.Realtem;

namespace _01_Script.Item
{
    public interface IItemReceiver
    {
        bool CanReceive(ItemType itemType, MaterialType materialType, EquipmentType equipmentType);
        void Receive(ItemParent item);
    }
}