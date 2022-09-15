using UnityEngine;

namespace RPG.ScriptableObjects.Sounds
{
    /// <summary>
    /// Scriptable Object em que consiste em dois arrays de dados entre músicas de ambiente ou dungeons
    /// </summary>
    [CreateAssetMenu]
    public class BackgroundSounds : ScriptableObject
    {
        public AudioClip[] backgroundSounds;
        public AudioClip[] dungeonSounds;
    }

}
