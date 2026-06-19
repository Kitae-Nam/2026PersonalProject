using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _01_Script.Item;
using _01_Script.Item.Realtem;
using _01_Script.Managers;
using _01_Script.Pool;
using _01_Script.Rails;
using _01_Script.UI.Bottom;
using UnityEngine;

namespace _01_Script.Train
{
    public class CraftingTrain : MonoBehaviour, ITrain, IItemReceiver
    {//todo: 일단 추가되면 위치 조정해주고 추가되었을때 레일 만들 수 있는지 확인하고 만들기
        [SerializeField] private BottomUiChangeSo bottomUiChange;
        [SerializeField] private string railName;
        [SerializeField] private ContainerTrain railPoint;
        [SerializeField] private Transform woodPoint;
        [SerializeField] private Transform rockPoint;
        [SerializeField] private float railMakingTime;
        
        [SerializeField] private int _maxWoodCount = 5;
        [SerializeField] private int _maxRockCount = 5;
        
        private Stack<ItemParent> woodItems = new Stack<ItemParent>();
        private Stack<ItemParent> rockItems = new Stack<ItemParent>();
        private bool isMaking = false;
        private ItemParent _currentItem;


        private void Start()
        {
            ObjPositionManager.Instance.AddItemPosition(this.gameObject.transform);
        }
        
        public void ResourceAdd(ItemParent resourceItem)
        {
            if (resourceItem == null) return;
            
            if (resourceItem.itemSo.materialType == MaterialType.Wood)
            {
                if (woodItems.Count >= _maxWoodCount) return;
            }
            else if (resourceItem.itemSo.materialType == MaterialType.Rock)
            {
                if (rockItems.Count >= _maxRockCount) return;
            }
            else return;

            _currentItem = resourceItem;

            if (resourceItem.itemSo.materialType == MaterialType.Wood)
            {
                woodItems.Push(resourceItem);
            }
            else if (resourceItem.itemSo.materialType == MaterialType.Rock)
            {
                rockItems.Push(resourceItem);
            }

            _currentItem.itemGo.transform.position = PosEdit(resourceItem.itemSo.materialType);
            if(isMaking == false)
                StartCoroutine(ResourceUpdate());
            _currentItem.itemGo.transform.position = PosEdit(resourceItem.itemSo.materialType);

            RefreshContainerUi();

            if (isMaking == false)
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
            
            RefreshContainerUi();

            var rail = PoolManager.Instance.Spawn(railName, railPoint.transform.position, railPoint.transform.rotation);
            rail.transform.SetParent(railPoint.transform);
            if (rail.TryGetComponent(out RailAnimation rc))
            {
                railPoint.RailAdd(rc.GetComponent<Rail>());
                rc.OnRailMade?.Invoke();
            }

            yield return new WaitForSeconds(railMakingTime);
            StartCoroutine(ResourceUpdate());
        }
        private void RefreshContainerUi()
        {
            bottomUiChange.ContainerChangedInvoke(new ContainerItem
            {
                WoodCount = woodItems.Count,
                StoneCount = rockItems.Count,
            });
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
                    return woodPoint.position+ _currentItem.itemSo.onGroundPos + _currentItem.itemSo.onGroundAdditionalPos;
                }
            }
            else
            {
                _currentItem.transform.SetParent(rockPoint);
                if (rockItems.Count <= 1)
                {
                    return rockPoint.position + _currentItem.itemSo.onGroundPos;
                }
                else
                {
                    return rockPoint.position + _currentItem.itemSo.onGroundAdditionalPos * (rockItems.Count -1);
                }
            }
        }

        public bool CanReceive(ItemType itemType, MaterialType materialType, EquipmentType equipmentType)
        {
            if (materialType == MaterialType.Wood)
                return woodItems.Count < _maxWoodCount;

            if (materialType == MaterialType.Rock)
                return rockItems.Count < _maxRockCount;

            return false;
        }

        public void Receive(ItemParent item)
        {
            ResourceAdd(item);
        }
        
    }
}