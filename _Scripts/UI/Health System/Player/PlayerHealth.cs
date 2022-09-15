using RPG.UI.HealthSystem;
using UnityEngine;

namespace RPG.PlayerControlls.Health
{
    public class PlayerHealth : MonoBehaviour
    {
        private PlayerController _controller;
        private HealthUI _healthUI;

        private void Start()
        {
            _controller = GetComponent<PlayerController>();
            _healthUI = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<HealthUI>();
        }
        /// <summary>
        /// Função usada para passar o valor de ganhar vida para o sistema de vida
        /// </summary>
        public void PlayerHeal(int value)
        {
            _healthUI._healthSystem.Heal(value);
            _controller.PlayerHeal();
        }
        /// <summary>
        /// Função usada para passar o valor de dano para o sistema de vida
        /// </summary>
        public void PlayerHurt(int value)
        {
            _healthUI._healthSystem.Damage(value);
            _controller.PlayerHurt();
        }
    }
}