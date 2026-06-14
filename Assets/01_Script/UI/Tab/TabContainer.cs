using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _01_Script.UI.Tab
{
    public class TabContainer : MonoBehaviour
    {
        [SerializeField] private Tab[] tabs;
        [SerializeField] private GameObject[] contents;

        private void Start()
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                int index = i;
                tabs[i].AddEvent(EventTriggerType.PointerClick, () => ShowTab(index));
                if (contents[i].TryGetComponent(out CanvasGroup canvasGroup))
                {
                    canvasGroup.alpha = 0;
                }
                else
                {
                    contents[i].SetActive(false);
                }
            }
            ShowTab(0);
        }

        private void ShowTab(int index)
        {
            for (int i = 0; i < contents.Length; i++)
            {
                if (contents[i].TryGetComponent(out CanvasGroup canvasGroup))
                {
                    bool isActive = i == index;
                    canvasGroup.alpha = isActive ? 1 : 0;
                }
                else
                {
                    contents[i].SetActive(i == index);
                }
            }
        }
    }
}