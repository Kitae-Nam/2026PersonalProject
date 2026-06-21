using System;
using TMPro;
using UnityEngine;

namespace _01_Script.Managers
{
    public class CostManager : MonoSingleton<CostManager>
    {
        public Action<int> OnCostChange;
        
        [SerializeField] private TextMeshProUGUI _costAmountText;
        private int _costAmount;

        public void Add(int costAmount)
        {
            _costAmount += costAmount;
            _costAmountText.text = _costAmount.ToString();
        }

        public bool CanSpend(int costAmount)
        {
            return _costAmount >= costAmount;
        }
        public void Spend(int costAmount)
        {
            _costAmount -= costAmount;
            _costAmountText.text = _costAmount.ToString();
        }
    }
}