using System;
using System.Collections.Generic;
using _01_Script.Rails;
using Reflex.Core;
using UnityEngine;

namespace _01_Script.Managers
{
    public class RailManager : MonoSingleton<RailManager>
    {
        private readonly List<Rail> _railsList = new List<Rail>();
        
        public Action OnRailUpdated;
        public List<Rail> RailsList => _railsList;
        
        public void RailAdd(Rail rail)
        {
            if (_railsList.Contains(rail)) return;
            _railsList.Add(rail);
            OnRailUpdated?.Invoke();
        }

        public void RailRemove(Rail rail)
        {
            if (!_railsList.Contains(rail)) return;
            _railsList.Remove(rail);
            OnRailUpdated?.Invoke();
        }

        public int GetNextRail(int index)
        {
            if(index >= RailsList.Count) return -1;
            return index + 1;
        }
    }
}