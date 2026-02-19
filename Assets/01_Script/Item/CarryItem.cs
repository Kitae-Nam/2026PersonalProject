using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CarryItem : MonoBehaviour
{//todo: 아이템 줍는데 여러개가 쌓일 수 있게, 아이템 버릴때 같은 아이템이면 쌓이게
    [SerializeField] private LayerMask _itemLayers;
    [SerializeField] private Vector3 _itemCarryRange;
    [SerializeField] private Vector3 _itemCarryOffset;
    [SerializeField] private Transform _itemCarryParent;
    [SerializeField] private float _pickUpDelay = 0.5f;

    private float _timer = 0f;

    public int _canCarryItemCount = 3;
    public int _currentCarryItemCount = 0;

    private Stack<Item> _itemStack = new Stack<Item>();
    private bool IsCarryItem => _itemStack.Count > 0;
    private Tilemap _groundTile => GameManager.Instance._groundTile;


    private void Update()
    {
        _timer += Time.deltaTime;
        HandleItemImput();
    }

    private void HandleItemImput()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && _timer <= _pickUpDelay)
        {
            if (IsCarryItem == false)     //들고 있는 아이템이 없다면
            {
                Debug.Log("start");
                Collider[] colliders = Physics.OverlapBox(transform.position + _itemCarryOffset, _itemCarryRange, Quaternion.identity, _itemLayers);

                if (colliders.Length > 0)
                {
                    foreach (var collider in colliders)
                    {
                        if (collider.TryGetComponent<Item>(out Item item))
                        {
                            TryPickUp(item);
                            Debug.Log("picked up");
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("drop");
                DropItem();
            }
        }
        _timer = 0f;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if ((_itemLayers.value & (1 << collision.gameObject.layer)) != 0 && IsCarryItem == true)
        {
            TryPickUp(collision.gameObject.GetComponent<Item>());
        }
    }

    private void TryPickUp(Item item)
    {
        if (_itemStack.Contains(item) == true)
            return;
        if (_currentCarryItemCount >= _canCarryItemCount)
            return;

        Debug.Log("try pick up");
        if (_currentCarryItemCount <= 0)    //아이템을 안 들고 있음
        {
            _itemStack.Push(item);
            _currentCarryItemCount++;

            //아이템 들어서 위치 조정
            ItemPosEdit(item, item._itemSO._itemCarryPos);
        }
        else
        {
            _itemStack.Push(item);
            _currentCarryItemCount++;

            ItemPosEdit(item,
                item._itemSO._additionalCarryPos * _currentCarryItemCount);
        }
    }

    private void ItemPosEdit(Item item, Vector3 pos)
    {
        item.transform.SetParent(_itemCarryParent);
        item.transform.localPosition = pos;
        item.transform.localRotation = Quaternion.identity;
    }

    private void DropItem()
    {
        int itemCount = 0;
        if (_currentCarryItemCount >= 0)
        {
            foreach (var item in _itemStack)
            {
                item.transform.SetParent(null);
                Vector3Int cellPos = _groundTile.WorldToCell(transform.position);
                Vector3 dropPos = _groundTile.GetCellCenterWorld(cellPos);

                item.transform.position = dropPos + item._itemSO._additionalCarryPos * itemCount;
                itemCount++;
            }

            _itemStack.Clear();
            _currentCarryItemCount = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + _itemCarryOffset, _itemCarryRange);
    }
}
