using System.Collections.Generic;
using _01_Script.Item.Realtem;
using _01_Script.Managers;
using UnityEngine;

namespace _01_Script.Item
{
    public class ItemPile : MonoBehaviour
    {
        public Stack<ItemParent> itemStack = new Stack<ItemParent>();
        [SerializeField] private List<ItemParent> _itemList;

        [SerializeField] private ItemParent[] _toPushDirectlyItem;
        public int count => itemStack.Count;

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
            itemStack.Push(itemParent);
            _itemList.Add(itemParent);
            itemParent.gameObject.transform.SetParent(this.transform);

            PositionEdit(itemParent);
        }

        public ItemParent PopItem()
        {
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
            if (itemStack.Count > 0)
            {
                ItemParent[] itemParents = new ItemParent[count];
                for (int i = 0; i < itemStack.Count; i++)
                {
                    if (count > i)
                    {
                        itemParents[i] = itemStack.Pop();
                    }
                }
                return itemParents;
            }

            return null;
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
            if (itemStack.Count > 1)        //�������� ������
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
    }
}
