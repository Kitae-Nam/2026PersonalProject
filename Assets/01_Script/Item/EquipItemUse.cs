using UnityEngine;
using UnityEngine.InputSystem;

public class EquipItemUse : MonoBehaviour
{
    private CarryItem _carryItem;
    [SerializeField] private LayerMask _treeLayer;
    [SerializeField] private LayerMask _rockLayer;
    [SerializeField] private float _useRange = 3f;
    [SerializeField] private float _useDelay = 1f;
    [SerializeField] private Vector3 _useOffset;

    private float _timer = 0f;

    private void Awake()
    {
        _carryItem = GetComponent<CarryItem>();
    }

    //private void Update()
    //{
    //    _timer += Time.deltaTime;
    //    if (Mouse.current.leftButton.wasPressedThisFrame && _carryItem.IsCarryEpuip)
    //    {
    //        if (_useDelay <= _timer)//장비를 들고 있음
    //        {
    //            Item carriedItem = _carryItem.CarriedItem.GetComponent<Item>();

    //            switch (carriedItem._itemSO._equipmentType)
    //            {
    //                case EquipmentType.Pickaxe:
    //                    ItemUse(_rockLayer);
    //                    break;
    //                case EquipmentType.Axe:
    //                    ItemUse(_treeLayer);
    //                    break;
    //            }
    //            _timer = 0f;
    //        }
    //    }
    //}
    //private void ItemUse(LayerMask layer)
    //{
    //    Collider[] colliders = Physics.OverlapBox(transform.position + _useOffset, Vector3.one * _useRange, Quaternion.identity, layer);

    //    foreach (var collider in colliders)
    //    {
    //        if (collider.TryGetComponent<HarvestableObject>(out HarvestableObject HObject))
    //        {
    //            HObject.Harvest();
    //        }
    //        Debug.Log(collider.name);
    //    }
    //}
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(transform.position + _useOffset, Vector3.one * _useRange);
    //}
}
