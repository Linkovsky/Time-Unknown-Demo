using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.SceneManagement
{
    public class SceneName : MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        [SerializeField] private Sprite _background;

        /// <summary>
        /// Usado para mover o jogador de scene em scene quando colide com o collider
        /// </summary>
        /// <param name="collision">Player é o colidor</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().stopPlayer = true;
                LevelManager.Instance.StartCoroutine(LevelManager.Instance.TransitionCoroutine(_sceneName, _background));
            }
        }
    }
}