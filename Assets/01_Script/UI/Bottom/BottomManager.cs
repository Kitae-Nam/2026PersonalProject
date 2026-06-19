using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Script.UI.Bottom
{
    public class BottomManager : MonoBehaviour
    {
        [SerializeField] private BottomUiChangeSo eventSo;
        
        [Header("인벤토리")]
        [SerializeField] private Image inventoryIcon;
        [SerializeField] private TextMeshProUGUI inventoryCount;
        [SerializeField] private List<Sprite> icons; //0:나무 1:돌 2:곡갱이 3:도끼 4:레일 5:none
        
        [Header("컨테이너")]
        [SerializeField] private TextMeshProUGUI woodCount;
        [SerializeField] private TextMeshProUGUI stoneCount;

        [Header("만들기 칸")]
        [SerializeField] private TextMeshProUGUI railCount;

        private void Start()
        {
            eventSo.OnInventoryChanged += InventoryUpdate;
            eventSo.OnContainerChanged += ContainerUpdate;
            eventSo.OnMakingChanged += MakingUpdate;
            
            inventoryCount.text = string.Empty;
            woodCount.text = "0";
            stoneCount.text = "0";
            railCount.text = "0";
        }

        private void OnDestroy()
        {
            eventSo.OnInventoryChanged -= InventoryUpdate;
            eventSo.OnContainerChanged -= ContainerUpdate;
            eventSo.OnMakingChanged -= MakingUpdate;
        }

        private void MakingUpdate(MakingItem obj)
        {
            railCount.text = obj.RailCount.ToString();
        }

        private void ContainerUpdate(ContainerItem obj)
        {
            woodCount.text = obj.WoodCount.ToString();
            stoneCount.text = obj.StoneCount.ToString();
        }

        private void InventoryUpdate(InventoryItem obj)
        {
            Debug.Log($"InventoryUpdate 호출됨: mat={obj.MaterialType}, matCount={obj.MaterialCount}, rail={obj.RailCount}");
            
            if (obj.MaterialType == MaterialType.None &&
                obj.EquipmentType == EquipmentType.None &&
                obj.ItemType != ItemType.Rail)
            {
                inventoryCount.text = string.Empty;
                inventoryIcon.sprite = icons[5];
                return;
            }
            if (obj.MaterialType != MaterialType.None)//재료 들때
            {
                inventoryCount.text = obj.MaterialCount.ToString();
                
                if (obj.MaterialType == MaterialType.Wood)
                    inventoryIcon.sprite = icons[0];
                else if (obj.MaterialType == MaterialType.Rock)
                    inventoryIcon.sprite = icons[1];
            }
            else if (obj.EquipmentType != EquipmentType.None)//도구  들때
            {
                inventoryCount.text = string.Empty;
                if(obj.EquipmentType == EquipmentType.Pickaxe)
                    inventoryIcon.sprite = icons[2];
                else if(obj.EquipmentType == EquipmentType.Axe)
                    inventoryIcon.sprite = icons[3];
            }
            else if (obj.ItemType == ItemType.Rail) //레일 들때
            {
                inventoryCount.text = obj.RailCount.ToString();
                inventoryIcon.sprite = icons[4];
            }
        }
    }
}