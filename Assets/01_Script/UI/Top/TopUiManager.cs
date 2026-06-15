using System;
using _01_Script.Event;
using _01_Script.Train;
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
            _stationTransform = GameManager.Instance.station.transform;
            _startXPosition = CurrentTrainTransform.position.x;

            _startDistanceX = Mathf.Abs(_stationTransform.position.x - _startXPosition);
        }

        private void StationChange(Transform obj)
        {
            _stationTransform = GameManager.Instance.station.transform;
        }

        private void SpeedUiUpdate(float obj)
        {
            string result = obj.ToString("F2");
            trainSpeedTxt.text = $"{result}ms";
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