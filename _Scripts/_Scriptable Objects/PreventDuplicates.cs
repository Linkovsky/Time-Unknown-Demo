using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects.Duplicate
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Number", menuName = "ChestOpened")]
    public class PreventDuplicates : ScriptableObject
    {
        //public string description;
        public int numberOfTimes;
    }
}

