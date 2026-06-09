using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _01_Script.Item.Realtem;
using _01_Script.Rails;
using UnityEngine;

namespace _01_Script.Train
{
    public class CraftingTrain : MonoBehaviour
    {//todo: 일단 추가되면 위치 조정해주고 추가되었을때 레일 만들 수 있는지 확인하고 만들기
        [SerializeField] private GameObject railPrefab;
        [SerializeField] private Transform railPoint;
        [SerializeField] private Transform woodPoint;
        [SerializeField] private Transform rockPoint;
        [SerializeField] private float railMakingTime;
        
        public ItemParent woodItem;
        public ItemParent rockItem;
        
        private Stack<ItemParent> woodItems = new Stack<ItemParent>();
        private Stack<ItemParent> rockItems = new Stack<ItemParent>();
        private bool isMaking = false;
        private ItemParent _currentItem;

        [ContextMenu("WoodAdd")]
        private void WoodAdd()
        {
            ResourceAdd(woodItem);
        }

        [ContextMenu("RockAdd")]
        private void RockAdd()
        {
            ResourceAdd(rockItem);
        }
        
        public void ResourceAdd(ItemParent resourceItem)
        {
            if (resourceItem == null) return;
            _currentItem = resourceItem;
            
            if(resourceItem.itemSo.materialType == MaterialType.Wood)
            {
                woodItems.Push(resourceItem); 
            }
            else if (resourceItem.itemSo.materialType == MaterialType.Rock)
            {
                rockItems.Push(resourceItem);
            }
            else return;

            _currentItem.itemGo.transform.position = PosEdit(resourceItem.itemSo.materialType);
            if(isMaking == false)
                StartCoroutine(ResourceUpdate());
        }

        private IEnumerator ResourceUpdate()
        {//레일 만드는거
            if (woodItems.Count <= 0 || rockItems.Count <= 0)
            {
                isMaking = false;
                yield break;
            }
            
            isMaking = true;
            ItemParent woodItem = woodItems.Pop();
            ItemParent rockItem = rockItems.Pop();
            //임시
            Destroy(woodItem.itemGo);
            Destroy(rockItem.itemGo);
            
            var rail = Instantiate(railPrefab, railPoint.position, Quaternion.identity);
            if (rail.TryGetComponent(out RailAnimation rc))
            {
                rc.OnRailMade?.Invoke();
            }

            yield return new WaitForSeconds(railMakingTime);
            StartCoroutine(ResourceUpdate());
        }

        private Vector3 PosEdit(MaterialType materialType)
        {
            if (materialType == MaterialType.Wood)
            {
                _currentItem.transform.SetParent(woodPoint);
                if (woodItems.Count == 0)
                {
                    return woodPoint.position + _currentItem.itemSo.onGroundPos;
                }
                else
                {
                    return woodPoint.position + _currentItem.itemSo.onGroundAdditionalPos;
                }
            }
            else
            {
                _currentItem.transform.SetParent(rockPoint);
                if (rockItems.Count == 0)
                {
                    return woodPoint.position + _currentItem.itemSo.onGroundPos;
                }
                else
                {
                    return woodPoint.position + _currentItem.itemSo.onGroundAdditionalPos;
                }
                
            }
        }

    }
}