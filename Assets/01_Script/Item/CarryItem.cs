using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CarryItem : MonoBehaviour
{
    [SerializeField] private ItemPile _itemPilePrefab;

    [SerializeField] private LayerMask _itemPileLayers;
    [SerializeField] private float _itemCarryRange;
    [SerializeField] private Vector3 _itemCarryOffset;
    [SerializeField] private Transform _itemCarryParent;
    [SerializeField] private float _pickUpDelay = 0.5f;

    private float _timer = 0f;

    [SerializeField] private int _canCarryItemCount = 3;
    [SerializeField] private int _currentCarryItemCount = 0;

    private Stack<Item> _itemStack = new Stack<Item>();

    private ItemType _currentCarryItemType = ItemType.None;
    private MaterialType _currentCarryMaterialType = MaterialType.None;
    private EquipmentType _currentCarryEquipmentType = EquipmentType.None;

    public bool IsCarryItem { get { return _itemStack.Count > 0; } }
    public Stack<Item> ItemStack { get { return _itemStack; } }
    private Tilemap _groundTile => GameManager.Instance.groundTile;

    private void Update()
    {
        _timer += Time.deltaTime;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (_canCarryItemCount <= _currentCarryItemCount) return;

        bool isItemPileLayer = (_itemPileLayers.value & (1 << collision.gameObject.layer)) != 0;
        if (isItemPileLayer && IsCarryItem)
        {
            if (collision.TryGetComponent<ItemPile>(out ItemPile itemPile))
            {
                if (itemPile.itemStack.Count == 0) return;

                if (CanPileOn(itemPile))
                {
                    Item topItem = itemPile.PopItem();
                    TryPickUp(topItem);
                }
            }
        }
    }

    public void HandleItemImput()
    {
        if (_timer >= _pickUpDelay)
        {
            if (IsCarryItem == false)
            {
                Debug.Log("start");
                ItemPickUpProcess();
            }
            else
            {
                Debug.Log("drop");
                ItemDropProcess();
            }
            _timer = 0f;
        }
    }

    private void ItemPickUpProcess()
    {
        Transform nearObj = ObjPositionManager.Instance.GetNearestItemPosition(transform.position + _itemCarryOffset, _itemCarryRange);

        if (nearObj != null)
        {
            if (nearObj.TryGetComponent<ItemPile>(out ItemPile itemDummy))
            {
                Item itemFromPile = itemDummy.PopItem();
                if (itemFromPile != null)
                {
                    Debug.Log("picked up");
                    TryPickUp(itemFromPile);
                }
            }
        }
        else
        {
            Debug.Log("itempile null");
        }
    }

    private void ItemDropProcess()
    {
        Transform nearObj = ObjPositionManager.Instance.GetNearestItemPosition(transform.position + _itemCarryOffset, _itemCarryRange);

        if (nearObj != null)
        {
            if (nearObj.TryGetComponent<ItemPile>(out ItemPile itemDummy))
            {
                if (CanPileOn(itemDummy))
                {
                    foreach (var item in _itemStack)
                    {
                        itemDummy.PushItem(item);
                        Debug.Log("piled up");
                    }
                    _currentCarryItemCount = 0;
                    _itemStack.Clear();
                    return;
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            Vector3Int cellPos = _groundTile.WorldToCell(transform.position);
            Vector3 dropPos = _groundTile.GetCellCenterWorld(cellPos);
            dropPos.y = 1;

            ItemPile newItemPile = Instantiate(_itemPilePrefab, dropPos, Quaternion.identity);
            newItemPile.transform.SetParent(GameManager.Instance.ItemPileParent);
            foreach (var item in _itemStack)
            {
                newItemPile.PushItem(item);
            }
            _currentCarryItemCount = 0;
            _itemStack.Clear();
            Debug.Log("dropped");
        }
        _currentCarryItemType = ItemType.None;
        _currentCarryMaterialType = MaterialType.None;
        _currentCarryEquipmentType = EquipmentType.None;
    }

    private bool CanPileOn(ItemPile pile)
    {
        if (pile.itemStack.Count == 0) return true;

        var topSO = pile.itemStack.Peek().itemSO;
        return topSO.itemType == _currentCarryItemType &&
               topSO.materialType == _currentCarryMaterialType &&
               topSO.equipmentType == _currentCarryEquipmentType;
    }

    private void TryPickUp(Item item)
    {
        if (_itemStack.Contains(item) == true)
            return;
        if (_currentCarryItemCount >= _canCarryItemCount)
            return;

        _currentCarryItemCount++;

        if (_currentCarryItemCount <= 1)
        {
            Debug.Log("picked up first item");
            _itemStack.Push(item);

            _currentCarryItemType = item.itemSO.itemType;
            _currentCarryMaterialType = item.itemSO.materialType;
            _currentCarryEquipmentType = item.itemSO.equipmentType;

            ItemPosEdit(item, item.itemSO.itemCarryPos);
        }
        else
        {
            Debug.Log("picked");
            _itemStack.Push(item);

            ItemPosEdit(item,
                item.itemSO.additionalCarryPos * _currentCarryItemCount);
        }
    }

    private void ItemPosEdit(Item item, Vector3 pos)
    {
        item.transform.SetParent(_itemCarryParent);
        item.transform.localPosition = pos;
        item.transform.localRotation = Quaternion.identity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _itemCarryOffset, _itemCarryRange);
    }
}
