using System.Collections.Generic;
using UnityEngine;

public class Rail : Item
{
    [SerializeField] private GameObject[] railArr;

    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private LayerMask railMask;

    private Collider _collider;
    public bool _isUpate = true;
    private Vector3[] _directions = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
    private byte _bit = 0b0000;
    private int _count;

    private static readonly Dictionary<int, int> _railIndex = new Dictionary<int, int>()
    {
        { 0b1100, 0 },  //谅-快
        { 0b1010, 1 },  //第-谅
        { 0b1001, 2 },  //菊-谅
        { 0b0110, 3 },  //第-快
        { 0b0101, 4 },  //菊-快
        { 0b0011, 5 },  //菊-第

        { 0b0001, 5 },
        { 0b0010, 5 },
        { 0b0100, 0 },
        { 0b1000, 0 },
    };

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        if (_isUpate)
        {
            UpdateRailDirection();
        }
    }
    private void UpdateRailDirection()
    {
        for (int i = 0; i < 4; i++)
        {
            if (DirectionRaycast(_directions[i]))
            {
                _bit = (byte)(_bit | (1 << i));     // 菊 第 快 谅 = 0001 0010 0100 1000
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if ((_bit & (1 << i)) != 0)
            {
                _count++;
            }
        }
        if (_count == 1)
        {
            ChangeShape(_bit);
        }
        else if (_count == 2)
        {
            SetDirection();
        }
    }

    private void SetDirection()
    {
        _isUpate = false;
        _collider.isTrigger = true;
        isCanCarry = false;
        ChangeShape(_bit);
    }
    private void ChangeShape(byte bit)
    {
        if (!_railIndex.TryGetValue(bit, out int index)) return;

        foreach (var rail in railArr) rail.SetActive(false);
        railArr[index].SetActive(true);
    }
    
    private bool DirectionRaycast(Vector3 direction)
    {
        bool isHit = Physics.Raycast(transform.position, direction, maxDistance,
            railMask, QueryTriggerInteraction.Ignore);//飘府芭捞搁 公矫

        if (isHit)
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.forward * maxDistance + transform.position);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, -transform.forward * maxDistance + transform.position);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.right * maxDistance + transform.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, -transform.right * maxDistance + transform.position);
    }
}
