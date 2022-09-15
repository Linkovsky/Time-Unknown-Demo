using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventory
    
{
    [System.Serializable]
    [CreateAssetMenu]
    public class Inventory : ScriptableObject
    {
        public List<InventoryItem> _inventory = new List<InventoryItem>();
        private readonly Dictionary<ItemData, InventoryItem> _itemDictionary = new Dictionary<ItemData, InventoryItem>();

        private void OnEnable()
        {
            Item.OnItemCollected += Add;
            foreach(var item in _inventory)
            {
                foreach(var itemD in _itemDictionary)
                {
                    if (_itemDictionary.ContainsKey(item.itemData) && item.itemData.isStakable)
                        itemD.Value.stackSize += item.stackSize;
                    else
                        _itemDictionary.Add(item.itemData, item);
                }
            }
        }

        private void OnDisable()
        {
            Item.OnItemCollected -= Add;
        }

        private void Add(ItemData itemData, int num)
        {
            if(_itemDictionary.TryGetValue(itemData, out InventoryItem item) && itemData.isStakable)
                item.AddToStack(num);
            else
            {
                InventoryItem newItem = new InventoryItem(itemData, num);
                _inventory.Add(newItem);
                _itemDictionary.Add(itemData, newItem);
            }
        }

        public void Remove(ItemData itemData, int num)
        {
            if (!_itemDictionary.TryGetValue(itemData, out InventoryItem item)) return;
            item.RemoveFromStack(num);
            if (item.stackSize != 0) return;
            _inventory.Remove(item);
            _itemDictionary.Remove(itemData);
        }

        public void ResetAll()
        {
            _inventory.Clear();
            _itemDictionary.Clear();
        }
    }
}