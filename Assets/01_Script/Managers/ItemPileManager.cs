using System.Collections.Generic;
using System.Linq;
using _01_Script.Item;
using _01_Script.Pool;
using UnityEngine;

namespace _01_Script.Managers
{
    public class ItemPileManager : MonoSingleton<ItemPileManager>
    {
        public List<ItemPile> itemPiles;

        public void AddItemPile(ItemPile itemPile)
        {
            itemPiles.Add(itemPile);
        }

        public void RefreshItemPiles()
        {
            foreach (var items in itemPiles)
            {
                if(items == null) { continue; }
                if (items.Count <= 0)
                {
                    PoolManager.Instance.Despawn(items.gameObject);
                }
            }
        }
    }
}