using System.Collections.Generic;
using _01_Script.Rails;
using UnityEngine;

namespace _01_Script.Managers
{
    public class RailPositionManager : MonoSingleton<RailPositionManager>
    {
        private readonly List<Rail> _rails = new List<Rail>();

        public void Register(Rail rail)
        {
            if (!_rails.Contains(rail))
                _rails.Add(rail);
        }

        public void Unregister(Rail rail)
        {
            _rails.Remove(rail);
        }

        public Rail GetAt(Vector3 position, float radius, Rail self)
        {
            _rails.RemoveAll(r => r == null);

            float radiusSqr = radius * radius;
            float nearestSqr = float.MaxValue;
            Rail nearest = null;

            foreach (var rail in _rails)
            {
                if (rail == self) continue;

                Vector3 diff = rail.transform.position - position;
                diff.y = 0f; 
                float sqr = diff.sqrMagnitude;

                if (sqr <= radiusSqr && sqr < nearestSqr)
                {
                    nearestSqr = sqr;
                    nearest = rail;
                }
            }
            return nearest;
        }
    }
}