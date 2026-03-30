using System.Collections.Generic;
using UnityEngine;

public class ItemPile : MonoBehaviour
{
    public Stack<Item> itemStack = new Stack<Item>();
    [SerializeField] private List<Item> _itemList;

    [SerializeField] private Item[] _toPushDirectlyItem;
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

    public void PushItem(Item item)
    {
        itemStack.Push(item);
        _itemList.Add(item);
        item.gameObject.transform.SetParent(this.transform);

        PositionEdit(item);
    }

    public Item PopItem()
    {
        if (itemStack.Count > 0)
        {
            int lastIndex = _itemList.Count - 1;
            _itemList.RemoveAt(lastIndex);

            Item item = itemStack.Pop();

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

    private void PositionEdit(Item item)
    {
        if (itemStack.Count > 1)        //아이템이 있을때
        {
            item.gameObject.transform.localPosition = new Vector3(0, itemStack.Count * item.itemSO.onGroundAdditionalPos.y, 0);
        }
        else
        {
            item.gameObject.transform.localPosition = item.itemSO.onGroundPos;
        }
    }
}
