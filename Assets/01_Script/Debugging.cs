using UnityEngine;

namespace _01_Script
{
    public class Debugging : MonoBehaviour
    {
        #if UNITY_EDITOR
        public static void Log<T>(T msg)
        {
            Debug.Log(msg);
        }

        public static void LogWarning<T>(T msg)
        {
            Debug.LogWarning(msg);
        }
        #endif
    }
}