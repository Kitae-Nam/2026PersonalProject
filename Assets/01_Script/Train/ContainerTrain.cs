using _01_Script.Item;
using _01_Script.Item.Realtem;
using UnityEngine;

namespace _01_Script.Train
{
    public interface ISender
    {
        public ItemParent RailGet();
    }

    public class ContainerTrain : MonoBehaviour, ISender
    {//todo : 레일 넣는걸 받아서 자식으로 있는 아이템 파일한테 전달?
        [SerializeField] private ItemPile itemPile;

        public void RailAdd(ItemParent item)
        {
            itemPile.PushItem(item);
        }

        public ItemParent RailGet()
        {
            return itemPile.PopItem();
        }
    }
}