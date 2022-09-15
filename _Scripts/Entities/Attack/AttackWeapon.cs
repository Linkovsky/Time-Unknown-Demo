using RPG.PlayerControlls.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Entities.DamageResources
{
    public class AttackWeapon : MonoBehaviour
    {
        /// <summary>
        /// Declara��o de vari�veis
        /// </summary>
        [SerializeField] private int _damage;
        /// <summary>
        /// Collider com trigger case seja o jogador, tenta buscar a refer�ncia ao componente da vida para tirar.
        /// </summary>
        /// <param name="collision">Representa o objeto que entra em colis�o</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out PlayerHealth health))
            {
                health.PlayerHurt(_damage);
                Destroy(gameObject);
            }
        }
    }
}