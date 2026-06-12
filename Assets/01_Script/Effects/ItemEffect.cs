using System;
using System.Collections;
using _01_Script.Pool;
using UnityEngine;

namespace _01_Script.Effects
{
    public class ItemEffect : MonoBehaviour, IPoolable
    {
        [SerializeField] private ParticleSystem ps;
        private float _particleTime;

        private void Start()
        {
            var main =  ps.main;
            
            float duration = main.duration;

            float maxLifetime = main.startLifetime.constantMax; 
        
            _particleTime = duration + maxLifetime;
        }

        public void OnPop()
        {
            ps.Play();
            StartCoroutine(PlayDone());
        }

        private IEnumerator PlayDone()
        {
            yield return new WaitForSeconds(_particleTime);
            yield return new WaitForSeconds(_particleTime/2);
            PoolManager.Instance.Despawn(this.gameObject);
        }

        public void OnPush()
        {
            ps.Stop();
        }
    }
}