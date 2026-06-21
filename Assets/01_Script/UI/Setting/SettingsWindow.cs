using DG.Tweening;
using UnityEngine;

namespace _01_Script.UI.Setting
{
    public class SettingsWindow : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.3f;

        void Awake()
        {
            // 시작 시 닫힌 상태
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            gameObject.SetActive(false);
        }

        public void Open()
        {
            Time.timeScale = 0f;   // ★ 게임 정지
            gameObject.SetActive(true);
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true);
        }

        // EXIT 버튼에 연결
        public void Close()
        {
            PlayerPrefs.Save();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.DOFade(0f, fadeDuration)
                .SetUpdate(true)   // ★ 중요
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    Time.timeScale = 1f;   // ★ 재개
                });
        }
    }
}