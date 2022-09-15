using RPG.Inventory;
using RPG.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField] private ItemData _weapon;

    /// <summary>
    /// Collider com trigger que compara qual o objeto que entrou em contacto e se for um dos inimigos então
    /// tenta buscar o componente da vida para poder dar dano ao inimigo
    /// </summary>
    /// <param name="collision">Colisão do jogador</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss") || collision.CompareTag("LastBoss"))
        {
            var enemy = collision.gameObject.GetComponent<EnemyAI>();
            if (enemy != null)
                enemy._playerCollide = true;
            collision.gameObject.GetComponentInChildren<EnemyHealth>().Damage(_weapon.damage);
        }
        else
            return;
    }
}
