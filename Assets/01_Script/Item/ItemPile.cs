using System.Collections.Generic;
using _01_Script.Managers;
using UnityEngine;

namespace _01_Script.Item
{
    public class ItemPile : MonoBehaviour
    {
        public Stack<Realtem.ItemParent> itemStack = new Stack<Realtem.ItemParent>();
        [SerializeField] private List<Realtem.ItemParent> _itemList;

        [SerializeField] private Realtem.ItemParent[] _toPushDirectlyItem;
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

        public void PushItem(Realtem.ItemParent itemParent)
        {
            itemStack.Push(itemParent);
            _itemList.Add(itemParent);
            itemParent.gameObject.transform.SetParent(this.transform);

            PositionEdit(itemParent);
        }

        public Realtem.ItemParent PopItem()
        {
            if (itemStack.Count > 0)
            {
                int lastIndex = _itemList.Count - 1;
                _itemList.RemoveAt(lastIndex);

                Realtem.ItemParent itemParent = itemStack.Pop();

                itemParent.gameObject.transform.SetParent(null);
                return itemParent;
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

        private void PositionEdit(Realtem.ItemParent itemParent)
        {
            if (itemStack.Count > 1)        //�������� ������
            {
                itemParent.gameObject.transform.localPosition = new Vector3(0, itemStack.Count * itemParent.itemSo.onGroundAdditionalPos.y, 0);
            }
            else
            {
                itemParent.gameObject.transform.localPosition = itemParent.itemSo.onGroundPos;
            }
        }
    }
}
