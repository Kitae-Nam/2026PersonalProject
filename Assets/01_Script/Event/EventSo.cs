using System;
using UnityEngine;

namespace _01_Script.Event
{
    [CreateAssetMenu(fileName = "EventSo", menuName = "SO/EventSo")]
    public class EventSo : ScriptableObject
    {
        public event Action<EventSoData> OnCarryEvent;
        
        public void OnCarry(EventSoData es) => OnCarryEvent?.Invoke(es);
    }

    public struct EventSoData
    {
        
    }
}