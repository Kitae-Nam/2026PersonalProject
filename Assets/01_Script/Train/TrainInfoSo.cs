using UnityEngine;

namespace _01_Script.Train
{
    [CreateAssetMenu(fileName = "TrainInfoSo", menuName = "SO/Train", order = 0)]
    public class TrainInfoSo : ScriptableObject
    {
        public float speed;
    }
}