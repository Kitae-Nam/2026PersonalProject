using System;
using _01_Script.HarvestableObject;
using _01_Script.Item;
using _01_Script.Managers;
using _01_Script.Train;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _01_Script.UI.Carouse
{
    public enum UpgradeType { None, Player, CraftSpeed, Engine }
    public class StoreCard : MonoBehaviour
    {
        [SerializeField] private CardSo cardSo;
        [SerializeField] private int maxLevel;
        [SerializeField] private TextMeshProUGUI CostText;
        [SerializeField] private Button UpgradeBtn;
        [SerializeField] private UpgradeType upgradeType;

        [Header("Player")]
        [SerializeField] private CarryItem carryItem;
        [SerializeField] private HarvestableSo[] harvestSo;
        
        [Header("Engine")]
        [SerializeField] private TrainInfoSo trainInfo;
        
        [Header("Craft")]
        [SerializeField] private CraftingTrain craftingTrain;

        private int currentLevel;
        
        private void Start()
        {
            UpgradeBtn.onClick.AddListener(() => OnBtnClick());
            CostText.text = cardSo.cost.ToString();
        }

        private void OnBtnClick()
        {
            if (maxLevel <= currentLevel) return;
            if (CostManager.Instance.CanSpend(cardSo.cost))
            {
                CostManager.Instance.Spend(cardSo.cost);
                switch (upgradeType)
                {
                    case UpgradeType.Player:
                        carryItem._canCarryItemCount += 5;
                        foreach (var item in harvestSo)
                        {
                            item.hitCount++;
                        }
                        break;
                    case UpgradeType.Engine:
                        trainInfo.Init();
                        break;
                    case UpgradeType.CraftSpeed:
                        craftingTrain.railMakingTime--;
                        break;
                }
                currentLevel++;
                CostText.text = cardSo.cost.ToString();
            }
            else
            {
                return;
            }
        }
    }
}