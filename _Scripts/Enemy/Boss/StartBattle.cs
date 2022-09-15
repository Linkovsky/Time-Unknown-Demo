using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RPG.SceneManagement;

namespace RPG.Enemy.Boss
{
    public class StartBattle : MonoBehaviour
    {
        [SerializeField] private GameObject _boss;
        [SerializeField] private AudioClip _bossClip;

        /// <summary>
        /// Quando come�ar a colis�o dos objetos inic�a a batalha por ir buscando o m�todo DoTween "DOPLAY" para iniciar o caminho
        /// do inimigo. Muda a m�sica e come�a a tocar.
        /// </summary>
        /// <param name="collision">Representa o objeto que ir� colidir com o collider</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _boss.GetComponentInParent<DOTweenPath>().DOPlay();
                LevelManager.Instance._audioSource.clip = _bossClip;

                LevelManager.Instance._audioSource.Play();
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                _boss.GetComponentInParent<DOTweenPath>().DOPause();
                LevelManager.Instance.PlayDungeonMusic(0);
            }
        }
    }
}