using System.Collections;
using System.Collections.Generic;
using _01_Script.Item.Realtem;
using UnityEngine;

namespace _01_Script.Train
{
    public class CraftingTrain : MonoBehaviour
    {//todo: 일단 추가되면 위치 조정해주고 추가되었을때 레일 만들 수 있는지 확인하고 만들기
        [SerializeField] private GameObject railPrefab;
        
        private readonly List<ItemParent> woodItems = new List<ItemParent>();
        private readonly List<ItemParent> rockItems = new List<ItemParent>();
        
        public void ResourceAdd(ItemParent resourceItem)
        {
            if (resourceItem == null) return;
            
            if(resourceItem.itemSo.materialType == MaterialType.Wood)
            {
                woodItems.Add(resourceItem); 
            }
            else if (resourceItem.itemSo.materialType == MaterialType.Rock)
            {
                rockItems.Add(resourceItem);
            }
            else return;

            StartCoroutine(ResourceUpdate());
        }

        private IEnumerator ResourceUpdate()
        {
            if (woodItems.Count == 0 || rockItems.Count == 0) return null;
            //레일 만드는거

            return null;
        }
    }
}