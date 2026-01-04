using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody _rigid;
    [SerializeField] private float _moveSpeed = 5f;
    private float _xAxis;
    private float _zAxis;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        _xAxis = Input.GetAxisRaw("Horizontal");
        _zAxis = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        Vector3 velocity = new Vector3(_xAxis, 0f, _zAxis).normalized * _moveSpeed;
        _rigid.linearVelocity = velocity;
    }
}
