using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _01_Script.UI
{
    public class UiEffecter : MonoBehaviour
    {
        [SerializeField] private float duration = 0.5f;
        protected CanvasGroup _canvasGroup;
        private Coroutine currentFadeRoutine;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void FadeIn()
        {
            StartFade(1f, duration, true);
        }
        public virtual void FadeOut()
        {
            StartFade(0f, duration, false);
        }
        private void StartFade(float targetAlpha, float duration, bool isVisible)
        {
            if (currentFadeRoutine != null)
            {
                StopCoroutine(currentFadeRoutine);
            }

            currentFadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, duration, isVisible));
        }

        private IEnumerator FadeRoutine(float targetAlpha, float duration, bool isVisible)
        {
            float startAlpha = _canvasGroup.alpha;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                yield return null;
            }

            _canvasGroup.alpha = targetAlpha;
        }
    }
}