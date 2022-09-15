using System.Collections;
using UnityEngine;

namespace RPG.Inventory.Interface
{
    public interface ICollectible
    {
        public IEnumerator CollectItem(PlayerController player);
    }
}