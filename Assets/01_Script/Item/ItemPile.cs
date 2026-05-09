using System.Collections.Generic;
using _01_Script.Managers;
using UnityEngine;

namespace _01_Script.Item
{
    public class ItemPile : MonoBehaviour
    {
        public Stack<Realtem.Item> itemStack = new Stack<Realtem.Item>();
        [SerializeField] private List<Realtem.Item> _itemList;

        [SerializeField] private Realtem.Item[] _toPushDirectlyItem;
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

        public void PushItem(Realtem.Item item)
        {
            itemStack.Push(item);
            _itemList.Add(item);
            item.gameObject.transform.SetParent(this.transform);

            PositionEdit(item);
        }

        public Realtem.Item PopItem()
        {
            if (itemStack.Count > 0)
            {
                int lastIndex = _itemList.Count - 1;
                _itemList.RemoveAt(lastIndex);

                Realtem.Item item = itemStack.Pop();

                item.gameObject.transform.SetParent(null);
                return item;
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

        private void PositionEdit(Realtem.Item item)
        {
            if (itemStack.Count > 1)        //�������� ������
            {
                item.gameObject.transform.localPosition = new Vector3(0, itemStack.Count * item.itemSo.onGroundAdditionalPos.y, 0);
            }
            else
            {
                item.gameObject.transform.localPosition = item.itemSo.onGroundPos;
            }
        }
    }
}
