using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _01_Script.Event;
using _01_Script.Item;
using _01_Script.Item.Realtem;
using _01_Script.Managers;
using Reflex.Attributes;
using UnityEngine.Splines;

namespace _01_Script.Rails
{
    public class Rail : ItemParent
    {
        [Inject] [SerializeField] private RailManager railManager;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject[] railArr;

        [SerializeField] private float maxDistance = 2f;
        [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);
        [SerializeField] private LayerMask railMask;

        public bool isConnected;
        public bool isFixedShape = false;
        public bool isStationRail = false;

        private EventSoData _eventSoData;
        private Collider _collider;
        private bool canUpdate = true;
        private readonly Vector3[] _directions = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
        private Vector3 worldDir;
        private int _bit = 0b0000;
        private int _rememberBit = 0b0000;
        private int _count;
        private bool _isFirstRail = false;
        private bool _pendingFix = false;
        
        private SplineContainer _currentSpline;
        public SplineContainer CurrentSpline => _currentSpline;

        private static readonly Dictionary<int, int> _railIndex = new Dictionary<int, int>()
        {
            { 0b1100, 0 },  //상-하
            { 0b1010, 1 },  //상-좌
            { 0b1001, 2 },  //상-우
            { 0b0110, 3 },  //하-좌
            { 0b0101, 4 },  //하-우
            { 0b0011, 5 },  //좌-우

            { 0b0001, 5 },
            { 0b0010, 5 },
            { 0b0100, 0 },
            { 0b1000, 0 },
        };

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            if (isConnected)
            {
                railManager.RailAdd(this);
                _isFirstRail = true;
                GetComponentInParent<ItemPile>().canStack = false;
            }
            else if (isStationRail)
            {
                isCanCarry = false;
                isConnected = true;
                GetComponentInParent<ItemPile>().canStack = false;
            }
            _currentSpline = GetComponentInChildren<SplineContainer>();
        }

        public override void CarredItem()
        {
            
        }

        public override void DropedItem()
        {
            StartCoroutine(UpdateAfterDrop());
        }

        [ContextMenu("Update")]
        public void SetTrue()
        {
            railManager.RailAdd(this);
            
            if (isConnected && !isCanCarry) return;
            
            isConnected = true;
            isCanCarry = false;
            if(_isFirstRail)
                _collider.isTrigger = true;
            var itemPile = GetComponentInParent<ItemPile>();
            if(itemPile)
                itemPile.canStack = false;
        }

        private IEnumerator UpdateAfterDrop()
        {
            yield return null;
            yield return new WaitForEndOfFrame();
            yield return null;
            UpdateRailDirection();
            NotifyNearbyRails();
            CommitFix();
            
            if (isConnected)
                SetTrue();
        }

        private void UpdateRailDirection()
        {
            _bit = 0b0000;
            for (int i = 0; i < 4; i++)
            {
                worldDir = transform.TransformDirection(_directions[i]);
                bool isHit = Physics.Raycast(transform.position, worldDir, out var hit, maxDistance, railMask);
                if (isHit && hit.collider.TryGetComponent<Rail>(out var rail))
                {
                    if (rail.isConnected && rail.isFixedShape == false)
                    {
                        isConnected = true;
                        _bit = (byte)(_bit | (1 << i));
                    }
                }
            }

            CountBit(_bit);
            if (_count >= 2)
            {
                _rememberBit = 0;
                ChangeShape(_bit);
                _pendingFix = true;
            }

            if (_count == 1)
            {
                if (_rememberBit != 0)
                {
                    _bit = _bit | _rememberBit;
                    CountBit(_bit);
                    ChangeShape(_bit);
                    _pendingFix = true;
                }
                else
                {
                    _rememberBit = _bit;
                    ChangeShape(_bit);
                }

                if (_isFirstRail)
                    _pendingFix = true;
            }
        }

        private void CountBit(int bit)
        {
            _count = 0;
            for (int i = 0; i < 4; i++)
            {
                if ((bit & (1 << i)) != 0)
                {
                    _count++;
                }
            }
        }
        public void CommitFix()
        {
            if (!_pendingFix || isFixedShape) return;
            _pendingFix = false;

            canUpdate = false;
            _collider.isTrigger = true;
            isCanCarry = false;
            isFixedShape = true;
        }
        private void ChangeShape(int bit)
        {
            if (isFixedShape) return;
            if (!_railIndex.TryGetValue(bit, out int index)) return;

            foreach (var rail in railArr) rail.SetActive(false);
            railArr[index].SetActive(true);
            
            _currentSpline = GetComponentInChildren<SplineContainer>();
        }

        private void NotifyNearbyRails()
        {
            var neighbors = new List<Rail>(4);

            for (int i = 0; i < 4; i++)
            {
                worldDir = transform.TransformDirection(_directions[i]);
                if (!Physics.Raycast(transform.position, worldDir, out var hit, maxDistance, railMask))
                    continue;
                if (!hit.collider.TryGetComponent<Rail>(out var rail)) continue;
                if (!rail.isConnected) continue;

                neighbors.Add(rail);
            }

            foreach (var rail in neighbors)
                rail.UpdateRailDirection();

            foreach (var rail in neighbors)
            {
                rail.CommitFix();
                rail.SetTrue();
                this.SetTrue();
                Debug.Log(rail.name);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.forward * maxDistance + transform.position );
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, -transform.forward * maxDistance + transform.position);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.right * maxDistance + transform.position);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, -transform.right * maxDistance + transform.position);
        }
    }
}
