using UnityEngine;
using UnityEngine.InputSystem;

public class EquipItemUse : MonoBehaviour
{
    private CarryItem _carryItem;
    [SerializeField] private LayerMask _treeLayer;
    [SerializeField] private LayerMask _rockLayer;
    [SerializeField] private Vector3 _useRange;
    [SerializeField] private float _useDelay = 1f;
    [SerializeField] private Vector3 _useOffset;

    private float _timer = 0f;

    private void Awake()
    {
        _carryItem = GetComponent<CarryItem>();
    }

    private void Update()
    {
        if(Input.GetMouseButton(0) && _carryItem.IsCarryItem == true)
        {
            
        }
    }
}
