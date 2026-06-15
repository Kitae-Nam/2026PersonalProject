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
        [SerializeField] private CanvasGroup[] contents;

        private void Start()
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                int index = i;
                tabs[i].AddEvent(EventTriggerType.PointerClick, () => ShowTab(index));
                contents[i].alpha = 0;
            }
            ShowTab(0);
        }

        private void ShowTab(int index)
        {
            for (int i = 0; i < contents.Length; i++)
            {
                bool isActive = (i == index);
                contents[i].alpha = isActive ? 1 : 0;
            }
        }
    }
}