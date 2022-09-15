using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Enemy.Boss
{
    public class LastBossBattleStart : MonoBehaviour
    {
        /// <summary>
        /// Declara��o das vari�veis
        /// </summary>
        [SerializeField] private LastBossBattle bossBattle = null;
        [SerializeField] private Animator gate = null;
        [SerializeField] private AudioClip _lastBattleTheme;
        [SerializeField] private float _speed;

        private LevelManager _levelManager;

        /// <summary>
        /// Vari�veis recebem o seu pr�prio componente
        /// </summary>
        private void Start()
        {
            _levelManager = GameObject.FindGameObjectWithTag("UI").GetComponent<LevelManager>();
        }

        /// <summary>
        /// Quando o Objeto sair da colis�o, inic�a a batalha + troca de m�sica
        /// </summary>
        /// <param name="collision"> Representa o objeto que ir� colidir com o collider</param>
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