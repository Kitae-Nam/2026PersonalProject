using System;
using _01_Script.Event;
using _01_Script.Managers;
using _01_Script.Train;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace _01_Script.UI.Top
{
    public class TopUiManager : MonoBehaviour
    {
        [SerializeField] private TrainInfoChangeSo trainEventSo;
        [SerializeField] private TrainInfoSo trainInfoSo;
        
        [SerializeField] private TextMeshProUGUI trainSpeedTxt;
        [SerializeField] private TextMeshProUGUI trainDistanceTxt;
        [SerializeField] private Image processImage;
        [SerializeField] private Image positionImage;
        [SerializeField] private Image trainImage;

        [SerializeField] private float posSpeed;
        [SerializeField] private float posDistance;
        [SerializeField] private float trainDistance;
        [SerializeField] private float trainSpeedMultiple;
        private float _trainSpeed;
        private Vector2 _startPosPosition;
        public Vector2 _startTrainPosition;
        private float _yOffset;
        private float _yOffset2;

        private Transform _stationTransform;
        private Transform CurrentTrainTransform => GameManager.Instance.engineTrain.transform;

        private float _startDistanceX;
        private float _startXPosition;
        
        private float _distanceZ;
        private float _barWidth;
        private float _pivotX;
        private float _fillAmount;
        private float _localXPosition;

        private void Start()
        {
            if (trainEventSo != null)
            {
                trainEventSo.OnSpeedChange += SpeedUiUpdate;
                trainEventSo.OnStationChange += StationChange;
            }
            SpeedUiUpdate(trainInfoSo.speed);
            _trainSpeed = trainInfoSo.speed *trainSpeedMultiple;
            _stationTransform = GameManager.Instance.station.transform;
            _startXPosition = CurrentTrainTransform.position.x;

            _startDistanceX = Mathf.Abs(_stationTransform.position.x - _startXPosition);
            _startPosPosition = positionImage.rectTransform.localPosition;
            _startTrainPosition = trainImage.rectTransform.localPosition;
        }

        private void StationChange(Transform obj)
        {
            _stationTransform = GameManager.Instance.station.transform;
        }

        private void SpeedUiUpdate(float obj)
        {
            string result = obj.ToString("F2");
            trainSpeedTxt.text = $"{result}ms";
            _trainSpeed = trainInfoSo.speed * trainSpeedMultiple;
        }

        private void Update()
        {
            _yOffset = Mathf.Sin(Time.time * posSpeed) * posDistance;
            _yOffset2 = Mathf.Sin(Time.time * _trainSpeed) * trainDistance;
            positionImage.rectTransform.localPosition = new Vector2(_startPosPosition.x, _startPosPosition.y + _yOffset);
            trainImage.rectTransform.localPosition = new Vector2(_startTrainPosition.x, _startTrainPosition.y + _yOffset2);

            trainDistanceTxt.text = $"{TrainManager.Instance.TotalDistance.ToString("F1")}m";
        }

        private void LateUpdate()
        {
            ProcessBarUpdate();
        }

        private void ProcessBarUpdate()
        {
            float movedDistance = Mathf.Abs(CurrentTrainTransform.position.x - _startXPosition);

            processImage.fillAmount = Mathf.Clamp01(movedDistance / _startDistanceX);
            
            _barWidth = processImage.rectTransform.rect.width;
            _pivotX = processImage.rectTransform.pivot.x;
            _fillAmount = processImage.fillAmount;
            _localXPosition = (-_pivotX * _barWidth) + (_fillAmount * _barWidth);
            
            positionImage.rectTransform.localPosition   
                = new Vector3(_localXPosition, positionImage.rectTransform.localPosition.y, positionImage.rectTransform.localPosition.z);
        }
    }
}