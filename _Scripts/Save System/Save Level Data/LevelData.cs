using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SaveSystem
{
    [System.Serializable]

    public class LevelData
    {
        public int numberOfTimes;

        public LevelData(int numberOfTimes)
        {
            this.numberOfTimes = numberOfTimes;
        }
    }
}