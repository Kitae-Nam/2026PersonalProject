using UnityEngine;

public class CarryItem : MonoBehaviour
{
    [SerializeField] private LayerMask _itemLayer;
    [SerializeField] private Transform _itemCarryPoint;
    [SerializeField] private Transform _equipCarryPoint;
    [SerializeField] private Transform _detectPoint;
    [SerializeField] private Vector3 _detectSize;
    private GameObject _carriedItem;

    [SerializeField] private GridLayout _gridLayout;

    private bool _isCarryItem = false;
    public bool IsCarryItem { get { return _isCarryItem; } }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_carriedItem == null)
            {
                TryPickUpItem();
            }
            else
            {
                DropItem();
            }
        }
    }
    private void TryPickUpItem()
    {
        Collider[] itemsInRange = Physics.OverlapBox(_detectPoint.position, _detectSize, Quaternion.identity, _itemLayer);
        
        if (itemsInRange.Length > 0)
        {
            _carriedItem = itemsInRange[0].gameObject;
            if (_carriedItem.layer == LayerMask.NameToLayer("EquipItem"))
            {
                _carriedItem.transform.SetParent(_equipCarryPoint);
                _carriedItem.transform.localPosition = new Vector3(_itemCarryPoint.localPosition.x, 0, _itemCarryPoint.localPosition.z);
                _carriedItem.transform.localRotation = Quaternion.Euler(0, 90, 0);
                _isCarryItem = false;
            }
            else
            {
                _carriedItem.transform.SetParent(_itemCarryPoint);
                _carriedItem.transform.localPosition = new Vector3(_itemCarryPoint.localPosition.x, 0, _itemCarryPoint.localPosition.z);
                _isCarryItem = true;
            }
        }
    }
    private void DropItem()//_gridLayout 찾는거 다시 하기
    {
        _carriedItem.transform.SetParent(null);

        if (_isCarryItem == false)
           _carriedItem.transform.rotation = Quaternion.Euler(90, 0, 0);

        Vector3 dropPosition = _gridLayout.WorldToCell(transform.position);
        _carriedItem.transform.position = dropPosition;

        _carriedItem = null;
        _isCarryItem = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(_detectPoint.position, _detectSize);
    }

}
