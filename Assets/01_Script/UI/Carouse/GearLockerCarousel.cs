using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Script.UI.Carouse
{
    public class GearLockerCarousel : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private RectTransform content;

        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;

        [Header("Settings")] [SerializeField] private float spacing = 350f;
        [SerializeField] private float moveDuration = 0.4f;
        [SerializeField] private Ease moveEase = Ease.OutCubic;
        [SerializeField] private float centerScale = 1f;
        [SerializeField] private float sideScale = 0.7f;

        private List<RectTransform> items = new List<RectTransform>();
        private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
        private float currentPosition = 0f;
        private bool isMoving = false;
        private int itemCount => items.Count;

        void Start()
        {
            CollectItems();
            UpdateLayout();

            if (leftButton != null) leftButton.onClick.AddListener(MoveLeft);
            if (rightButton != null) rightButton.onClick.AddListener(MoveRight);
        }

        void CollectItems()
        {
            // Content의 자식들을 순서대로 수집
            for (int i = 0; i < content.childCount; i++)
            {
                RectTransform rt = content.GetChild(i) as RectTransform;
                if (rt == null || !rt.gameObject.activeSelf) continue;

                items.Add(rt);

                CanvasGroup cg = rt.GetComponent<CanvasGroup>();
                if (cg == null) cg = rt.gameObject.AddComponent<CanvasGroup>();
                canvasGroups.Add(cg);

                int index = i;
                Button btn = rt.GetComponentInChildren<Button>();
                if (btn != null)
                    btn.onClick.AddListener(() => OnItemClicked(index));
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
            if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
        }

        public void MoveLeft()
        {
            if (isMoving) return;
            AnimateTo(currentPosition + spacing);
        }

        public void MoveRight()
        {
            if (isMoving) return;
            AnimateTo(currentPosition - spacing);
        }

        void AnimateTo(float target)
        {
            isMoving = true;
            DOTween.To(() => currentPosition, x =>
                {
                    currentPosition = x;
                    UpdateLayout();
                }, target, moveDuration)
                .SetEase(moveEase)
                .OnComplete(() =>
                {
                    currentPosition = target;
                    UpdateLayout();
                    isMoving = false;
                });
        }

        void UpdateLayout()
        {
            if (itemCount == 0) return;
            float totalWidth = itemCount * spacing;

            for (int i = 0; i < items.Count; i++)
            {
                float itemPos = i * spacing + currentPosition;
                itemPos = Mathf.Repeat(itemPos + totalWidth * 0.5f, totalWidth) - totalWidth * 0.5f;

                items[i].anchoredPosition = new Vector2(itemPos, 0f);

                float distance = Mathf.Abs(itemPos);
                
                bool visible = distance < spacing * 1.2f;
                items[i].gameObject.SetActive(visible);
                if (!visible) continue;
                
                float t = Mathf.Clamp01(distance / spacing);
                float scale = Mathf.Lerp(centerScale, sideScale, t);
                items[i].localScale = Vector3.one * scale;

                bool isCenter = distance < spacing * 0.5f;
                canvasGroups[i].interactable = isCenter;
                canvasGroups[i].blocksRaycasts = isCenter;
                canvasGroups[i].alpha = 1;
            }

            SortByDepth();
        }

        void SortByDepth()
        {
            List<RectTransform> sorted = new List<RectTransform>(items);
            sorted.Sort((a, b) => Mathf.Abs(b.anchoredPosition.x).CompareTo(Mathf.Abs(a.anchoredPosition.x)));
            for (int i = 0; i < sorted.Count; i++)
                sorted[i].SetSiblingIndex(i);
        }

        void OnItemClicked(int index)
        {
            Debug.Log($"Item {index} clicked!");
        }

        void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}