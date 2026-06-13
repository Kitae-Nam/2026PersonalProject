using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _01_Script.Item.Realtem;
using _01_Script.Managers;
using _01_Script.Pool;
using _01_Script.Train;
using UnityEngine;
using UnityEngine.Rendering;

namespace _01_Script.Item
{
    public class ItemPile : MonoBehaviour, IPoolable, IItemReceiver, ISender
    {
        public Stack<ItemParent> itemStack = new Stack<ItemParent>();
        [SerializeField] private List<ItemParent> _itemList;

        [SerializeField] private ItemParent[] _toPushDirectlyItem;
        public int count => itemStack.Count;
        public bool canStack = true;

        private void Awake()
        {
            foreach (var item in _itemList)
            {
                itemStack.Push(item);
            }

            if (ObjPositionManager.Instance == null) return;
            ObjPositionManager.Instance.AddItemPosition(gameObject.transform);
        }

        public void PushItem(ItemParent itemParent)
        {
            NullRemove();
            itemStack.Push(itemParent);
            _itemList.Add(itemParent);
            itemParent.gameObject.transform.SetParent(this.transform);

            PositionEdit(itemParent);
        }


        public ItemParent PopItem()
        {
            NullRemove();
            if (itemStack.Count > 0)
            {
                int lastIndex = _itemList.Count - 1;
                _itemList.RemoveAt(lastIndex);

                ItemParent itemParent = itemStack.Pop();

                itemParent.gameObject.transform.SetParent(null);
                return itemParent;
            }
            return null;
        }

        public ItemParent[] PopAllItem(int count)
        {
            NullRemove();
            if (count <= 0 || itemStack.Count == 0) return null;
            if (itemStack.Count > 0)
            {
                int actualExtractCount = Mathf.Min(count, itemStack.Count);
                
                ItemParent[] itemParents = new ItemParent[actualExtractCount];
                for (int i = 0; i < actualExtractCount; i++)
                {
                    itemParents[i] = itemStack.Pop();
                }
                return itemParents;
            }

            return null;
        }
        private void NullRemove()
        {
            Stack<ItemParent> temp = new Stack<ItemParent>(itemStack.Where(x => x != null).Reverse());
            itemStack = temp;
        }

        [ContextMenu("Directly Put")]
        public void DirectlyPut()
        {
            foreach (var directItem in _toPushDirectlyItem)
                PushItem(directItem);
        }
        [ContextMenu("CLear")]
        public void Clear()
        {
            itemStack.Clear();
            _itemList.Clear();
        }

        private void PositionEdit(ItemParent itemParent)
        {
            if (itemStack.Count > 1)
            {
                itemParent.gameObject.transform.localPosition = 
                    itemParent.itemSo.onGroundPos + 
                    new Vector3(0, (itemStack.Count - 1) * itemParent.itemSo.onGroundAdditionalPos.y, 0);
            }
            else
            {
                itemParent.gameObject.transform.localPosition = itemParent.itemSo.onGroundPos;
            }
        }

        public void OnPop()
        {
        }

        public void OnPush()
        {
        }

        public bool CanReceive(ItemType itemType, MaterialType materialType, EquipmentType equipmentType)
        {
            if (canStack == false) return false;

            if (itemStack.Count == 0) return true;

            var topSo = itemStack.Peek().itemSo;
            return topSo.itemType == itemType &&
                   topSo.materialType == materialType &&
                   topSo.equipmentType == equipmentType;
        }

        public void Receive(ItemParent item)
        {
            PushItem(item);
        }

        public ItemParent RailGet()
        {
            return PopItem();
        }
    }
}
