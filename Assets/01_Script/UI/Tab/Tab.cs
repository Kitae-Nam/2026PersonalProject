using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _01_Script.UI.Tab
{
    public class Tab : Triggerable
    {
        private CanvasGroup _canvasGroup;

        public void TurnOn()
        {
            _canvasGroup.alpha = 1;
        }

        public void TurnOff()
        {
            _canvasGroup.alpha = 0;
        }
    }
}