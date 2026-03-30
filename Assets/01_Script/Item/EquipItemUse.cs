using UnityEngine;

public class EquipItemUse : MonoBehaviour
{
    private CarryItem _carryItem;
    [SerializeField] private LayerMask _treeLayer;
    [SerializeField] private LayerMask _rockLayer;
    [SerializeField] private LayerMask _waterLayer;
    [SerializeField] private float _useRange;
    [SerializeField] private GameObject _useOffset;
    [SerializeField] private float _useDelay = 1f;

    private float _timer = 0f;
    private Item _topItem;

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
            Debug.Log("Interaction start");
            if (_carryItem.IsCarryItem == false) return;

            _topItem = _carryItem.ItemStack.Peek();
            if (_topItem == null) return;

            if (_topItem.itemSO.itemType == ItemType.Equipment)
            {
                EquipmentUse();
            }
            _timer = 0;
        }
    }

    private void EquipmentUse()
    {
        EquipmentItem equipItem = _topItem.itemGO.GetComponent<EquipmentItem>();
        Transform nearHarvest = ObjPositionManager.Instance.GetNearestHavaPosition(_useOffset.transform.position, _useRange);

        if (nearHarvest == null || equipItem == null)
        {
            Debug.Log(nearHarvest == null);
            Debug.Log(equipItem == null);
            return;
        }
        //해당 장비가 획득 가능한 재료인지 확인
        if (equipItem != null)
        {
            if (GetHavaLayer(equipItem) == nearHarvest.gameObject.layer)
            {
                GameObject harvestObj = nearHarvest.gameObject;
                equipItem.Use(harvestObj);
            }
        }
    }

    private LayerMask GetHavaLayer(EquipmentItem equipment)
    {
        LayerMask layerMask = 0;
        switch (equipment.itemSO.equipmentType)
        {
            case EquipmentType.Pickaxe:
                layerMask = _rockLayer;
                break;
            case EquipmentType.Axe:
                layerMask = _treeLayer;
                break;
            case EquipmentType.Bucket:
                layerMask = _waterLayer;
                break;
        }
        return layerMask;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_useOffset.transform.position, _useRange);
    }
}
