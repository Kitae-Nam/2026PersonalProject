using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CarryItem : MonoBehaviour
{//todo: 아이템 줍는데 여러개가 쌓일 수 있게, 아이템 버릴때 같은 아이템이면 쌓이게
    [SerializeField] private ItemPile _itemPilePrefab;

    [SerializeField] private LayerMask _itemPileLayers;
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
    private void OnTriggerEnter(Collider collision)
    {
        if (_itemStack.Contains(collision.gameObject.GetComponent<Item>()) == true)
            return;
        if ((_itemPileLayers.value & (1 << collision.gameObject.layer)) != 0 && IsCarryItem == true)
        {
            if (collision.TryGetComponent<ItemPile>(out ItemPile itemDummy))
            {
                Item itemFromPile = itemDummy.PopItem();
                if (itemFromPile != null)
                {
                    TryPickUp(itemFromPile);
                }

            }
        }
    }

    private void HandleItemImput()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && _timer <= _pickUpDelay)
        {
            if (IsCarryItem == false)     //들고 있는 아이템이 있다면(아이템 줍기)
            {
                Debug.Log("start");
                ItemPickUpProcess();
            }
            else        //아이템 버리기
            {
                Debug.Log("drop");
                ItemDropProcess();
            }
        }
        _timer = 0f;
    }

    private void ItemPickUpProcess()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + _itemCarryOffset, _itemCarryRange, Quaternion.identity, _itemPileLayers);

        if (colliders.Length > 0)//아이템 더미가 있다면
        {
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<ItemPile>(out ItemPile itemDummy))
                {
                    Item itemFromPile = itemDummy.PopItem();
                    if (itemFromPile != null)
                    {
                        Debug.Log("picked up");
                        TryPickUp(itemFromPile);
                        break;
                    }
                }
            }
        }
    }

    private void ItemDropProcess()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + _itemCarryOffset, _itemCarryRange, Quaternion.identity, _itemPileLayers);

        if (colliders.Length > 0)   //아이템 더미가 있다면
        {
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<ItemPile>(out ItemPile itemDummy))
                {
                    foreach (var item in _itemStack)
                    {
                        itemDummy.PushItem(item);
                    }
                    _itemStack.Clear();

                    Debug.Log("piled up");
                    break;
                }
            }
        }
        else    //아이템 더미가 없다면
        {
            Vector3Int cellPos = _groundTile.WorldToCell(transform.position);
            Vector3 dropPos = _groundTile.GetCellCenterWorld(cellPos);
            dropPos.y = 1;

            ItemPile newItemPile = Instantiate(_itemPilePrefab, dropPos, Quaternion.identity);
            foreach (var item in _itemStack)
            {
                newItemPile.PushItem(item);
            }
            _itemStack.Clear();
        }
    }


    private void TryPickUp(Item item)
    {
        if (_itemStack.Contains(item) == true)
            return;
        if (_currentCarryItemCount >= _canCarryItemCount)
            return;

        _currentCarryItemCount++;

        if (_currentCarryItemCount <= 0)    //아이템을 안 들고 있음
        {
            _itemStack.Push(item);

            //아이템 들어서 위치 조정
            ItemPosEdit(item, item._itemSO._itemCarryPos);
        }
        else
        {
            _itemStack.Push(item);

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + _itemCarryOffset, _itemCarryRange);
    }
}
