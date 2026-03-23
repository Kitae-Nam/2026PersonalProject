using Unity.VisualScripting;
using UnityEngine;

public class EquipItemUse : MonoBehaviour
{
    private CarryItem _carryItem;
    [SerializeField] private LayerMask _treeLayer;
    [SerializeField] private LayerMask _rockLayer;
    [SerializeField] private LayerMask _waterLayer;
    [SerializeField] private float _useRange;
    [SerializeField] private Vector3 _useOffset;
    [SerializeField] private float _useDelay = 1f;

    private float _timer = 0f;
    private ItemSO _topItem;

    private void Awake()
    {
        _carryItem = GetComponent<CarryItem>();
    }
    private void Update()
    {
        _timer += Time.deltaTime;
    }
    public void HandleInteractionInput()
    {
        if (_timer >= _useDelay)
        {
            if (_carryItem.IsCarryItem == false) return;

            _topItem = _carryItem.ItemStack.Peek()._itemSO;
            if (_topItem == null) return;

            if (_topItem._itemType == ItemType.Equipment)
            {
                EquipmentUse();
            }
            _timer = 0;
        }
    }

    private void EquipmentUse()
    {
        EquipmentItem equipItem = _topItem.GetComponent<EquipmentItem>();

        Transform nearHarvest = ObjPositionManager.Instance.GetNearestHavaPosition(transform.position + _useOffset, _useRange);

        GameObject harvestObj = nearHarvest.gameObject;

        if (equipItem != null)
        {
            equipItem.Use(harvestObj);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + _useOffset, _useRange);
    }
}
