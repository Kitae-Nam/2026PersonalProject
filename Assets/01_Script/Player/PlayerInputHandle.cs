using UnityEngine;

public class PlayerInputHandle : MonoBehaviour
{
    [SerializeField] private PlayerInputSO _playerInput;
    [SerializeField] private CarryItem _carryItem;
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private EquipItemUse _equipItemUse;

    private void Awake()
    {
        _playerInput.OnInteractionChange += _carryItem.HandleItemImput;
        _playerInput.OnMovementChange += _playerMove.HandleMoveInput;
        _playerInput.OnAttackChange += _equipItemUse.HandleInteractionInput;
    }
    private void OnDestroy()
    {
        _playerInput.OnInteractionChange -= _carryItem.HandleItemImput;
        _playerInput.OnMovementChange -= _playerMove.HandleMoveInput;
        _playerInput.OnAttackChange -= _equipItemUse.HandleInteractionInput;
    }
}
