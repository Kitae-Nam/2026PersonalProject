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

            if(_topItem == null)
                _topItem = _carryItem.ItemStack.Peek()._itemSO;

            if (_topItem._itemType == ItemType.Equipment)
            {

            }
            _timer = 0;
        }
    }

    private void EquipmentUse()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + _useOffset, _useRange);
    }
}
