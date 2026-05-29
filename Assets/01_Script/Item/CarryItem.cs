using System;
using System.Collections.Generic;
using _01_Script.Item.Realtem;
using _01_Script.Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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

        private Stack<ItemParent> _itemStack = new Stack<ItemParent>();

        private ItemType _currentCarryItemType = ItemType.None;
        private MaterialType _currentCarryMaterialType = MaterialType.None;
        private EquipmentType _currentCarryEquipmentType = EquipmentType.None;

        public bool IsCarryItem { get { return _itemStack.Count > 0; } }
        public Stack<ItemParent> ItemStack { get { return _itemStack; } }
        private Tilemap _groundTile => GameManager.Instance.groundTile;
        
        private void Update()
        {
            _timer += Time.deltaTime;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (_canCarryItemCount <= _currentCarryItemCount) return;

            bool isItemPileLayer = (_itemPileLayers.value & (1 << collision.gameObject.layer)) != 0;
            Debugging.Log(isItemPileLayer);
            if (isItemPileLayer && IsCarryItem)
            {
                if (collision.gameObject.TryGetComponent<ItemPile>(out ItemPile itemPile))
                {
                    if (itemPile.itemStack.Count == 0) return;

                    if (CanPileOn(itemPile))
                    {
                        ItemParent[] topItemParent = itemPile.PopAllItem(_canCarryItemCount - _currentCarryItemCount);
                        foreach (var item in topItemParent)
                        {
                            if(item == null) continue;
                            TryPickUp(item);
                        }
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
                    ItemPickUpProcess();
                }
                else
                {
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
                    ItemParent itemParentFromPile = itemDummy.PopItem();
                    if (itemParentFromPile != null)
                    {
                        TryPickUp(itemParentFromPile);
                    }
                }
            }
            else
            {
                Debugging.Log("itempile null");
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
                        }
                        _currentCarryItemCount = 0;
                        _itemStack.Clear();
                        
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

        private void TryPickUp(ItemParent itemParent)
        {
            if (_itemStack.Contains(itemParent) == true || _currentCarryItemCount >= _canCarryItemCount || itemParent.isCanCarry == false)
                return;

            _currentCarryItemCount++;

            if (_currentCarryItemCount <= 1)
            {
                _itemStack.Push(itemParent);

                _currentCarryItemType = itemParent.itemSo.itemType;
                _currentCarryMaterialType = itemParent.itemSo.materialType;
                _currentCarryEquipmentType = itemParent.itemSo.equipmentType;

                ItemPosEdit(itemParent, itemParent.itemSo.itemCarryPos);
            }
            else
            {
                _itemStack.Push(itemParent);

                ItemPosEdit(itemParent, itemParent.itemSo.itemCarryPos +
                    itemParent.itemSo.additionalCarryPos * (_currentCarryItemCount - 1));
            }
        }

        private void ItemPosEdit(ItemParent itemParent, Vector3 pos)
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
