using System;
using System.Collections;
using System.Collections.Generic;
using _01_Script.Train;
using TMPro;
using UnityEngine;

namespace _01_Script.Managers
{
    public class TrainManager : MonoSingleton<TrainManager>
    {
        [SerializeField] private TrainSplineMove head;
        [SerializeField] private List<Transform> cars;// 뒷칸들, 머리칸 제외
        [SerializeField] private List<float> carSpacing = new List<float>();
        [SerializeField] private float recordStep = 0.05f;
        [SerializeField] private float teleportThreshold = 3f;
        [SerializeField] private GameObject count;
        [SerializeField] private TextMeshProUGUI countTimeTxt;
        [SerializeField] private float startTime = 5f;
        [SerializeField] private ParticleSystem trainParticles;
 
        private struct TrailSample
        {
            public Vector3 Pos;
            public Quaternion Rot;
            public float Dist;
        }
 
        private readonly List<TrailSample> _trail = new();
        private float _headTravelled;
        private Vector3 _lastHeadPos;
        
        private float[] _offsets;
        private float TrainLength => _offsets[^1];
        public float TotalDistance => _headTravelled;
 
        private void Start()
        {
            BuildOffsets();
            SeedTrail();
            StartCoroutine(RailMoveStart());
        }

        public void Restart()
        {
            trainParticles.Stop();
            StartCoroutine(RestartRoutine());
        }

        private IEnumerator RestartRoutine()
        {
            count.SetActive(true);
            for (int i = 0; i < startTime; i++)
            {
                countTimeTxt.text = $"{startTime - i}";
                yield return new WaitForSeconds(1);
            }

            head.TrainMoveCalculate();
            count.SetActive(false);
            trainParticles.Play();
            
            _lastHeadPos = head.transform.position;
        }

        private void BuildOffsets()
        {
            _offsets = new float[cars.Count];
            float sum = 0f;
            for (int i = 0; i < cars.Count; i++)
            {
                sum += carSpacing[i];
                _offsets[i] = sum;
            }
        }

        private IEnumerator RailMoveStart()
        {
            for (int i = 0; i < startTime; i++)
            {
                countTimeTxt.text = $"{startTime - i}";
                yield return new WaitForSeconds(1);
            }
            head.TrainMoveCalculate();
            count.SetActive(false);
            trainParticles.Play();
        }

        private void SeedTrail()
        {
            _trail.Clear();
            _headTravelled = 0f;
            _lastHeadPos = head.transform.position;
 
            for (int i = cars.Count - 1; i >= 0; i--)
            {
                _trail.Add(new TrailSample
                {
                    Pos = cars[i].position,
                    Rot = cars[i].rotation,
                    Dist = -_offsets[i]
                });
            }
 
            _trail.Add(new TrailSample
            {
                Pos = head.transform.position,
                Rot = head.transform.rotation,
                Dist = 0f
            });
        }
 
        private void LateUpdate()
        {
            RecordHead();
            UpdateCars();
            TrimTrail();
        }
 
        private void RecordHead()
        {
            Vector3 headPos = head.transform.position;
            float delta = Vector3.Distance(headPos, _lastHeadPos);
 
            if (delta <= Mathf.Epsilon) return;
 
            if (delta > teleportThreshold)
            {
                _lastHeadPos = headPos;
                SeedTrail();
                return;
            }
 
            _headTravelled += delta;
            _lastHeadPos = headPos;
 
            if (_headTravelled - _trail[^1].Dist >= recordStep)
            {
                _trail.Add(new TrailSample
                {
                    Pos = headPos,
                    Rot = head.transform.rotation,
                    Dist = _headTravelled
                });
            }
        }
 
        private void UpdateCars()
        {
            for (int i = 0; i < cars.Count; i++)
            {
                float targetDist = _headTravelled - _offsets[i];
                (Vector3 pos, Quaternion rot) = SampleTrail(targetDist);
                cars[i].SetPositionAndRotation(pos, rot);
            }
        }
 
        private (Vector3, Quaternion) SampleTrail(float d)
        {
            if (_trail.Count == 0) return (transform.position, transform.rotation);
            if (d <= _trail[0].Dist) return (_trail[0].Pos, _trail[0].Rot);
            if (d >= _trail[^1].Dist) return (_trail[^1].Pos, _trail[^1].Rot);
 
            for (int i = _trail.Count - 1; i > 0; i--)
            {
                if (d >= _trail[i - 1].Dist)
                {
                    TrailSample a = _trail[i - 1];
                    TrailSample b = _trail[i];
                    float t = Mathf.InverseLerp(a.Dist, b.Dist, d);
                    return (Vector3.Lerp(a.Pos, b.Pos, t), Quaternion.Slerp(a.Rot, b.Rot, t));
                }
            }
            return (_trail[0].Pos, _trail[0].Rot);
        }
 
        private void TrimTrail()
        {
            const float margin = 2f;
            float minNeeded = _headTravelled - TrainLength - margin;
            int removeCount = 0;
            while (removeCount < _trail.Count - 1 && _trail[removeCount + 1].Dist < minNeeded)
                removeCount++;
            if (removeCount > 0)
                _trail.RemoveRange(0, removeCount);
        }
    }
}