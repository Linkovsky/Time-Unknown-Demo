using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement.ScriptableObjects
{
    [CreateAssetMenu]

    public class LevelPositionsSO : ScriptableObject
    {
        public Vector2[] Positions;
        public Vector2 StartPosition;
        public Vector2[] RazorForestPos;
        public Vector2[] SparklingVillage;
        public Vector2 ForestOfMountains;
        public Vector2 LoopCave;
        public Vector2 BlizzVillage;
    }
}