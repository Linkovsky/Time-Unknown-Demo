using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.HealthSystem
{
    [System.Serializable]
    [CreateAssetMenu]
    public class HeartContainerSO : ScriptableObject
    {
        [field: SerializeField] public int heartNumber { get; private set; }

        public void IncreaseHeartContainer()
        {
            if (heartNumber == 20) return;
            heartNumber++;
        }

        public void SetHeartContainerNumber(int number)
        {
            heartNumber = number;
        }
    } 
}
