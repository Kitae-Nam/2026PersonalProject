using System;
using UnityEngine;

namespace _01_Script.UI.Bottom
{
    [CreateAssetMenu(fileName = "BottomUiChangeSo", menuName = "SO/Ui/Event", order = 0)]
    public class BottomUiChangeSo : ScriptableObject
    {
        public Action<InventoryItem> OnInventoryChanged;
        public Action<ContainerItem> OnContainerChanged;
        public Action<MakingItem> OnMakingChanged;
        
        public void InventoryChangedInvoke(InventoryItem item)
        {
            OnInventoryChanged?.Invoke(item);
        }

        public void ContainerChangedInvoke(ContainerItem item)
        {
            OnContainerChanged?.Invoke(item);
        }

        public void MakingChangedInvoke(MakingItem item)
        {
            OnMakingChanged?.Invoke(item);
        }
    }

    public struct InventoryItem
    {
        public ItemType ItemType;
        public MaterialType MaterialType;
        public EquipmentType EquipmentType;

        public int RailCount;
        public int MaterialCount;
    }

    public struct ContainerItem
    {
        public int WoodCount;
        public int StoneCount;
    }

    public struct MakingItem
    {
        public int RailCount;
    }
}