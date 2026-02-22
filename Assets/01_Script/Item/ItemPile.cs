using System.Collections.Generic;
using UnityEngine;

public class ItemPile : MonoBehaviour
{
    public Stack<Item> _itemStack = new Stack<Item>();
    public List<Item> _itemList;

    public Item[] _toPushDirectlyItem;

    private void Awake()
    {
        foreach (var item in _itemList)
        {
            _itemStack.Push(item);
        }
    }

    public void PushItem(Item item)
    {
        _itemStack.Push(item);
        _itemList.Add(item);
        item.gameObject.transform.SetParent(this.transform);

        PositionEdit(item);
    }

    public Item PopItem()
    {
        if (_itemStack.Count > 0)
        {
            int lastIndex = _itemList.Count - 1;
            _itemList.RemoveAt(lastIndex);

            Item item = _itemStack.Pop();

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
        _itemStack.Clear();
        _itemList.Clear();
    }

    private void PositionEdit(Item item)
    {
        if (_itemStack.Count > 0)        //아이템이 있을때
        {
            item.gameObject.transform.localPosition = new Vector3(0, _itemStack.Count * item._itemSO._additionalCarryPos.y, 0);
        }
        else
        {
            item.gameObject.transform.localPosition = Vector3.zero;
        }
    }
}
