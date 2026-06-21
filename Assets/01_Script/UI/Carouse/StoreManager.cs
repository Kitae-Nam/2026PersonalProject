using System;
using _01_Script.Managers;
using DG.Tweening;
using UnityEngine;

namespace _01_Script.UI.Carouse
{
    public class StoreManager : MonoSingleton<StoreManager>
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.3f;
        private CanvasGroup[] windows;

        private void Start()
        {
            windows = canvasGroup.GetComponentsInChildren<CanvasGroup>();
            Close();
        }

        public void Open()
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            canvasGroup.DOFade(1f, fadeDuration);

            foreach (var window in windows)
            {
                window.blocksRaycasts = true;
                window.interactable = true;
                window.DOFade(1f, fadeDuration);
            }
        }

        public void Close()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.DOFade(0f, fadeDuration);

            foreach (var window in windows)
            {
                window.blocksRaycasts = false;
                window.interactable = false;
                window.DOFade(0f, fadeDuration);
            }
        }
    }
}