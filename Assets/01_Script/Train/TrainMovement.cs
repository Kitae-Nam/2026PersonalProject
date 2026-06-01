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
        [Inject] private RailManager _railManager;
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotateSpeed = 5f;
        
        private int _currentRailIndex;
        private bool _canMove = false;
        private Vector3 _nextPosition;
        private float _posY;

        private void Start()
        {
            _posY = transform.position.y;
        }

        [ContextMenu("PosInit")]
        public void PosInit()
        {
            gameObject.transform.position = _railManager.RailsList[0].transform.position;
            _canMove = true;
            _currentRailIndex = 0;
            TrainMoveStart();
        }
        
        private void TrainMoveStart()
        {
            //todo : 현재 위치에서 다음 인덱스에 있는 레일로 일정하게 움직여야함
            //dotoween으로 하기엔 속도 조절이 어려움
            if (_canMove == false) return;
            if (_railManager.GetNextRail(_currentRailIndex) == -1) return;
            
            _nextPosition = _railManager.RailsList[_railManager.GetNextRail(_currentRailIndex)].transform.position;
            _nextPosition.y = _posY;

            Vector3 dir =  _nextPosition - transform.position;
            if (dir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.DORotateQuaternion(targetRotation, rotateSpeed * Time.deltaTime);
            }
            
            transform.DOMove(_nextPosition, speed)
                .SetSpeedBased()
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    transform.DOKill();
                    _currentRailIndex++;
                    TrainMoveStart();
                });
        }
    }
}