using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _01_Script.Managers;
using DG.Tweening;
using Reflex.Attributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Splines;

namespace _01_Script.Train
{
    public class TrainSplineMove : MonoBehaviour
    {
        public Transform trainPosition;
        public Transform trainRotation;

        public static event Action OnRailBroken;
        public static event Action OnRailStation;

        [Inject] [SerializeField] private RailManager railManager;
        [SerializeField] private TrainInfoSo trainSo;
        private int _currentRailIndex = 0;
        private bool _isReverse = false;//false=0->1, true=1->0
        private Spline _currentSpline;
        private SplineContainer _currentSplineContainer;
        private float _totalLength;
        private float _distance;
        private float _railStartPos;

        private Vector3 _targetPosition;
        private float _lookDistance;
        private float _lookDisOffset;
        private Vector3 _nextPosLook;
        private Vector3 _moveDir;

        private void Start()
        {
            if(trainPosition == null) trainPosition = transform;
        }

        [ContextMenu("초기화")]
        private void IndexInstialize()
        {
            _currentRailIndex = 0;
        }
        [ContextMenu("TrainMove")]
        public void TrainMoveCalculate()
        {//todo : 레일의 스플린을 따라 이동한다. 스플린의 리스트 번호대로 움직이는것이 아닌 현재 위치에 따라 유동적으로 움직인다.
            //todo : 다 움직이면 다음 레일의 스플린을 찾아 다시 간다.
            if(!railManager) return;
            if (railManager.GetNextRail(_currentRailIndex) == -1)
            {
                OnRailBroken?.Invoke();
                return;
            }
            
            _currentSplineContainer = railManager.RailsList[_currentRailIndex].CurrentSpline;
            _currentSpline = _currentSplineContainer.Spline;

            RailStartPosCalculate();

            StartCoroutine(FollowSplineRoute(_isReverse));
        }

        private void RailStartPosCalculate()
        {
            float distance1 = Vector3.Distance(trainPosition.position, _currentSplineContainer.EvaluatePosition(0));
            float distance2 = Vector3.Distance(trainPosition.position, _currentSplineContainer.EvaluatePosition(1));
            
            bool whoClose = distance1 < distance2;//true = 1번이 더 멀다
            _isReverse = !whoClose;
        }

        private IEnumerator FollowSplineRoute(bool isReverse)
        {
            _totalLength = _currentSpline.GetLength();
            
            Vector3 splineStart = _currentSplineContainer.EvaluatePosition(isReverse ? 1f : 0f);
            float gap = Vector3.Distance(transform.position, splineStart);

            if (gap > 0.01f)
            {
                Vector3 fromPos = transform.position;
                Quaternion fromRot = transform.rotation;

                Vector3 startTangent = (Vector3)_currentSplineContainer.EvaluateTangent(isReverse ? 1f : 0f);
                if (isReverse) startTangent = -startTangent;
                Quaternion toRot = startTangent != Vector3.zero
                    ? Quaternion.LookRotation(startTangent)
                    : fromRot;

                float travelled = 0f;
                while (travelled < gap)
                {
                    if (trainSo.speed == 0) yield break;
                    travelled += trainSo.speed * Time.deltaTime;
                    float t = Mathf.Clamp01(travelled / gap);

                    transform.position = Vector3.Lerp(fromPos, splineStart, t);
                    transform.rotation = Quaternion.Slerp(fromRot, toRot, t);
                    yield return null;
                }
            }
            
            _distance = isReverse? _totalLength : 0;
            
            while (isReverse ? (_distance > 0f) : (_distance < _totalLength))
            {
                if(trainSo.speed == 0)  yield break;
                
                if (isReverse)
                {
                    _distance -= trainSo.speed * Time.deltaTime;
                }
                else
                {
                    _distance += trainSo.speed * Time.deltaTime;
                }
                _distance = Mathf.Clamp(_distance,0f, _totalLength);

                _targetPosition = _currentSplineContainer.EvaluatePosition(_distance / _totalLength);
                
                // _lookDisOffset = isReverse ? -0.1f : 0.1f;
                // _lookDistance = Mathf.Min(_distance + _lookDisOffset, _totalLength);
                // _nextPosLook = _currentSplineContainer.EvaluatePosition(_lookDistance / _totalLength);
                // _moveDir = isReverse ? (transform.position - _nextPosLook) : (_nextPosLook - transform.position);
                
                _moveDir = _currentSplineContainer.EvaluateTangent(_distance / _totalLength);

                if (isReverse)
                {
                    _moveDir = -_moveDir;
                }
                if (_moveDir != Vector3.zero)
                {
                    trainRotation.rotation = Quaternion.LookRotation(_moveDir);
                }
                
                trainPosition.position = _targetPosition;
                yield return null;
            }
            OnPathComplete();
        }

        private void OnPathComplete()
        {
            var currentRail = railManager.RailsList[_currentRailIndex];

            if (currentRail.isStationRail) 
            {
                OnRailStation?.Invoke();
                return;
            }
            
            _currentRailIndex++;
            TrainMoveCalculate();
        }
    }
}