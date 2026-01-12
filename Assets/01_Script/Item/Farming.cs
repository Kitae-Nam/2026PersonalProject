using UnityEngine;
using UnityEngine.InputSystem;

public class Farming : MonoBehaviour
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
            if(_carryItem.IsCarryItem)//장비를 들고 있음
            {
                
            }
        }
    }
}
