using System.Collections.Generic;
using _01_Script.Rails;
using Reflex.Core;
using UnityEngine;

namespace _01_Script.Managers
{
    public class RailManager : MonoBehaviour, IInstaller
    {
        private readonly List<Rail> _railsList = new List<Rail>();
        
        public List<Rail> RailsList => _railsList;
        
        public void RailAdd(Rail rail)
        {
            if (_railsList.Contains(rail)) return;
            _railsList.Add(rail);
        }

        public void RailRemove(Rail rail)
        {
            if (!_railsList.Contains(rail)) return;
            _railsList.Remove(rail);
        }

        public int GetNextRail(int index)
        {
            if(!_railsList.Contains(_railsList[index + 1])) return -1;
            return index + 1;
        }
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue(this);
        }
    }
}