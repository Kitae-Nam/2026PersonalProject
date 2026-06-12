using System;
using UnityEngine;

namespace _01_Script.Train
{
    public class EngineTrain : MonoBehaviour,ITrain
    {
        private TrainSplineMove trainSplineMove;

        private void Awake()
        {
            trainSplineMove = GetComponent<TrainSplineMove>();
        }

        public void Move()
        {
            trainSplineMove.TrainMoveCalculate();
        }
    }
}