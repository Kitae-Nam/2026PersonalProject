using UnityEngine;
using UnityEngine.InputSystem;

public class DigUp : MonoBehaviour
{
    private CarryItem _carryItem;

    private void Awake()
    {
        _carryItem = GetComponent<CarryItem>();
    }

    private void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            if(_carryItem.IsCarryEpuip)//장비를 들고 있음
            {
                Item carriedItem = _carryItem.CarriedItem.GetComponent<Item>();

                switch (carriedItem._itemSO._equipmentType)
                {
                    case EquipmentType.Pickaxe:

                        break;
                    case EquipmentType.Axe:

                        break;
                }
            }
        }
    }
}
