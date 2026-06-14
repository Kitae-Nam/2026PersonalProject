using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _01_Script.UI
{
    public abstract class Triggerable :  MonoBehaviour
    {
        private EventTrigger _eventTrigger;

        protected virtual void Start()
        {
            _eventTrigger = GetComponent<EventTrigger>();
        }

        public void AddEvent(EventTriggerType eventTrigger, UnityAction action)
        {
            _eventTrigger.triggers.RemoveAll(e => e.eventID == eventTrigger);
            var entry = new EventTrigger.Entry();
            entry.eventID = eventTrigger;
            entry.callback.AddListener(eventData => action());
            _eventTrigger.triggers.Add(entry);
        }
    }
}