using System.Collections.Generic;
using _01_Script.Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _01_Script.Item
{
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

        private Stack<Realtem.ItemParent> _itemStack = new Stack<Realtem.ItemParent>();

        private ItemType _currentCarryItemType = ItemType.None;
        private MaterialType _currentCarryMaterialType = MaterialType.None;
        private EquipmentType _currentCarryEquipmentType = EquipmentType.None;

        public bool IsCarryItem { get { return _itemStack.Count > 0; } }
        public Stack<Realtem.ItemParent> ItemStack { get { return _itemStack; } }
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
                        Realtem.ItemParent topItemParent = itemPile.PopItem();
                        TryPickUp(topItemParent);
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
                    Realtem.ItemParent itemParentFromPile = itemDummy.PopItem();
                    if (itemParentFromPile != null)
                    {
                        Debug.Log("picked up");
                        TryPickUp(itemParentFromPile);
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
                            item.transform.rotation = Quaternion.identity;
                            itemDummy.PushItem(item);
                            item.DropedItem();
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
                    item.transform.rotation = Quaternion.identity;
                    newItemPile.PushItem(item);
                    item.DropedItem();
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

            var topSO = pile.itemStack.Peek().itemSo;
            return topSO.itemType == _currentCarryItemType &&
                   topSO.materialType == _currentCarryMaterialType &&
                   topSO.equipmentType == _currentCarryEquipmentType;
        }

        private void TryPickUp(Realtem.ItemParent itemParent)
        {
            if (_itemStack.Contains(itemParent) == true || _currentCarryItemCount >= _canCarryItemCount || itemParent.isCanCarry == false)
                return;

            _currentCarryItemCount++;

            if (_currentCarryItemCount <= 1)
            {
                Debug.Log("picked up first item");
                _itemStack.Push(itemParent);

                _currentCarryItemType = itemParent.itemSo.itemType;
                _currentCarryMaterialType = itemParent.itemSo.materialType;
                _currentCarryEquipmentType = itemParent.itemSo.equipmentType;

                ItemPosEdit(itemParent, itemParent.itemSo.itemCarryPos);
            }
            else
            {
                Debug.Log("picked");
                _itemStack.Push(itemParent);

                ItemPosEdit(itemParent,
                    itemParent.itemSo.additionalCarryPos * _currentCarryItemCount);
            }
        }

        private void ItemPosEdit(Realtem.ItemParent itemParent, Vector3 pos)
        {
            itemParent.transform.SetParent(_itemCarryParent);
            itemParent.transform.localPosition = pos;
            itemParent.transform.localRotation = Quaternion.identity;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + _itemCarryOffset, _itemCarryRange);
        }
    }
}
