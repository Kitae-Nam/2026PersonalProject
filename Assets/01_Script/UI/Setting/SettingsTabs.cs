using DG.Tweening;
using UnityEngine;

namespace _01_Script.UI.Setting
{
    public class SettingsTabs : MonoBehaviour
    {
        [SerializeField] private CanvasGroup graphicsPanel;
        [SerializeField] private CanvasGroup soundPanel;
        [SerializeField] private float fadeDuration = 0.25f;

        void OnEnable()
        {
            Debug.Log("ddd");
            ShowGraphics(); // 기본 탭
        }

        public void ShowGraphics() => Switch(graphicsPanel, soundPanel);
        public void ShowSound()    => Switch(soundPanel, graphicsPanel);

        private void Switch(CanvasGroup show, CanvasGroup hide)
        {
            hide.alpha = 0f;
            hide.blocksRaycasts = false;
            hide.interactable = false;
            hide.gameObject.SetActive(false);

            show.gameObject.SetActive(true);
            show.blocksRaycasts = true;
            show.interactable = true;
            show.DOFade(1f, fadeDuration);
            show.DOFade(1f, fadeDuration).SetUpdate(true);
        }
    }
}