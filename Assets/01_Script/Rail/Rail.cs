using System.Collections.Generic;
using UnityEngine;

public class Rail : Item
{
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private LayerMask railMask;
    [SerializeField] private RailInfoSO railInfoSO;

    private Collider _collider;
    private bool _isUpate = true;
    private Vector3[] _directions = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
    private byte _bit = 0b0000;
    private int _count;

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
                _bit = (byte)(_bit | (1 << i));     // 앞 뒤 우 좌 = 0001 0010 0100 1000
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if ((_bit & (1 << i)) != 0)
            {
                _count++;
            }
        }

        if (_count == 2)
        {
            SetDirection();
        }
    }

    private void SetDirection()
    {
        _isUpate = false;
        _collider.isTrigger = true;
        railInfoSO.MeshChange(_bit);
    }

    private bool DirectionRaycast(Vector3 direction)
    {
        bool isHit = Physics.Raycast(transform.position, direction, maxDistance,
            railMask, QueryTriggerInteraction.Ignore);//트리거이면 무시

        if (isHit)
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.forward * maxDistance);
        Gizmos.DrawLine(transform.position, -transform.forward * maxDistance);
        Gizmos.DrawLine(transform.position, transform.right * maxDistance);
        Gizmos.DrawLine(transform.position, -transform.right * maxDistance);
    }
}
