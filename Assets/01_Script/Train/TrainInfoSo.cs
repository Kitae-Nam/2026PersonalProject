using UnityEngine;

namespace _01_Script.Train
{
    [CreateAssetMenu(fileName = "TrainInfoSo", menuName = "SO/Train", order = 0)]
    public class TrainInfoSo : ScriptableObject
    {
        public float firstSpeed = 0.15f;
        public float speed;
        
        public void Init()
        {
            speed = firstSpeed;
        }
    }
}