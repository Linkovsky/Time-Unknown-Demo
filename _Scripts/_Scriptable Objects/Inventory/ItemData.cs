using UnityEngine;

namespace RPG.Inventory
{
    [System.Serializable]   
    [CreateAssetMenu]
    public class ItemData : ScriptableObject
    {
        public string displayName;
        public int damage;
        public int heal;
        public Sprite icon;
        public bool isStakable;

    }
}