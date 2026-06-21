using System;
using _01_Script.Item.Realtem;
using _01_Script.Managers;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;

namespace _01_Script.Train
{
    public class TrainMovement : MonoBehaviour
    {
        private RailManager _railManager;
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotateSpeed = 5f;
        [SerializeField] private float rotatePoint = 0.25f;
        
        private int _currentRailIndex;
        private bool _canMove = false;
        private Vector3 _nextPosition;
        private Vector3 _nextNextPosition;
        private Vector3 _currentPosition;
        private Vector3 _dirIn;
        private Vector3 _dirOut;
        private float _posY;

        private void Start()
        {
            _posY = transform.position.y;
        }

        [ContextMenu("PosInit")]
        public void PosInit()
        {
            gameObject.transform.position = RailManager.Instance.RailsList[0].transform.position;
            _canMove = true;
            _currentRailIndex = 0;
            TrainMoveStart();
        }
        
        private void TrainMoveStart()
        {
            //todo : 현재 위치에서 다음 인덱스에 있는 레일로 일정하게 움직여야함
            //dotoween으로 하기엔 속도 조절이 어려움
            if (_canMove == false) return;
            if (RailManager.Instance.GetNextRail(_currentRailIndex) == -1) return;

            var target = RailManager.Instance.RailsList[RailManager.Instance.GetNextRail(_currentRailIndex)];
            var nextTarget = RailManager.Instance.RailsList[RailManager.Instance.GetNextRail(_currentRailIndex + 1)];
            bool isTurning = false;

            _currentPosition = transform.position;
            _nextPosition = target.transform.position;
            _nextPosition.y = _posY;
            _nextNextPosition = RailManager.Instance.RailsList[RailManager.Instance.GetNextRail(_currentRailIndex + 1)].transform.position;

            _dirIn = (_nextPosition - _currentPosition).normalized;
            _dirOut = (nextTarget.transform.position - target.transform.position).normalized;
            
            isTurning = Vector3.Angle(_dirIn, _dirOut) > 5f;
            
            if (!isTurning)//돌아가는게 아닐때
            {
                Debug.Log("직선");
                transform.DOMove(target.transform.position, speed)
                    .SetSpeedBased(true)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _currentRailIndex++;
                        TrainMoveStart();
                    });
            }
            else
            {
                Debug.Log("곡선");
                Vector3 pIn = target.transform.position - _dirIn * rotatePoint;
                Vector3 pOut = target.transform.position + _dirOut * rotatePoint;
                Vector3 pMid = (pIn + pOut) * 0.5f;
                Vector3[] arc =
                {
                    pIn,
                    pOut,
                };
                transform.DOPath(arc, speed, PathType.CatmullRom)
                    .SetSpeedBased(true)
                    .SetEase(Ease.Linear)
                    .SetLookAt(0.01f)
                    .OnComplete(() =>
                    {
                        _currentRailIndex++;
                        TrainMoveStart();
                    });
            }
        }
    }
}