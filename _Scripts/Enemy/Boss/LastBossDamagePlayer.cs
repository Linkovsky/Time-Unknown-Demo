using RPG.PlayerControlls.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Enemy.Boss
{
    public class LastBossDamagePlayer : MonoBehaviour
    {
        /// <summary>
        /// Quando o player entrar em colisão recebe um valor random entre 1 e 4 e chama o método da vida do jogador
        /// </summary>
        /// <param name="collision">Representa o objeto que irá colidir com o collider</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                collision.gameObject.GetComponent<PlayerHealth>().PlayerHurt(Random.Range(1, 4));
        }
    }
}