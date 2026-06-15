using System;
using UnityEngine;

namespace _01_Script.Event
{
    [CreateAssetMenu(fileName = "TrainInfoChangeSo", menuName = "SO/Train/Event", order = 0)]
    public class TrainInfoChangeSo : ScriptableObject
    {
        public Action<float> OnSpeedChange;
        public Action<Transform> OnStationChange;
        
        public void OnSpeedChangeInvoke(float speed) => OnSpeedChange?.Invoke(speed);
        public void OnStationChangeInvoke(Transform ts) => OnStationChange?.Invoke(ts);
    }
}