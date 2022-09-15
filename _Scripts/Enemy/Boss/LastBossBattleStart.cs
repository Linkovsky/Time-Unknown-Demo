using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Enemy.Boss
{
    public class LastBossBattleStart : MonoBehaviour
    {
        /// <summary>
        /// Declaração das variáveis
        /// </summary>
        [SerializeField] private LastBossBattle bossBattle = null;
        [SerializeField] private Animator gate = null;
        [SerializeField] private AudioClip _lastBattleTheme;
        [SerializeField] private float _speed;

        private LevelManager _levelManager;

        /// <summary>
        /// Variáveis recebem o seu próprio componente
        /// </summary>
        private void Start()
        {
            _levelManager = GameObject.FindGameObjectWithTag("UI").GetComponent<LevelManager>();
        }

        /// <summary>
        /// Quando o Objeto sair da colisão, inicía a batalha + troca de música
        /// </summary>
        /// <param name="collision"> Representa o objeto que irá colidir com o collider</param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (_levelManager != null)
                {
                    _levelManager._audioSource.clip = _lastBattleTheme;
                    _levelManager._audioSource.Play();
                }
                bossBattle._startBattle = true;
                gate.Play("FadeIn");
            }
        }
    }
}