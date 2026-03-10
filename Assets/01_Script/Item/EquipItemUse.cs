using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipItemUse : MonoBehaviour
{
    private CarryItem _carryItem;
    [SerializeField] private LayerMask _treeLayer;
    [SerializeField] private LayerMask _rockLayer;
    [SerializeField] private LayerMask _waterLayer;
    [SerializeField] private Vector3 _useRange;
    [SerializeField] private Vector3 _useOffset;
    [SerializeField] private float _useDelay = 1f;

    private float _timer = 0f;

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

            ItemSO topItem = _carryItem.ItemStack.Peek()._itemSO;
            if (topItem._itemType == ItemType.Equipment)//todo : 장비 아이템 종류에 따라 레이어 달리하기, 근데 물병은 달라야함...
            {
                Collider[] colliders = Physics.OverlapBox(transform.position + _useOffset, _useRange, Quaternion.identity, LayerSelect(topItem._equipmentType));

                if(colliders.Length > 0)
                {
                    colliders[0].GetComponent<HarvestableObject>()?.Harvest();
                }
            }
        }
    }

    private LayerMask LayerSelect(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Pickaxe:
                return _rockLayer;
            case EquipmentType.Axe:
                return _treeLayer;
            case EquipmentType.Bucket:
                return _waterLayer;
            default:
                return 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + _useOffset, _useRange);
    }
}
