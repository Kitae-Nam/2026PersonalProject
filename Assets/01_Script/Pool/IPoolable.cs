using UnityEngine;

namespace _01_Script.Pool
{
    public interface IPoolable
    {
        public void OnPop();
        public void OnPush();
    }
}