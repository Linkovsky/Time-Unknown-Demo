using UnityEngine;
namespace RPG.PlayerControlls
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class InputControls : ScriptableObject
    {
        internal Rigidbody2D _rb;

        internal Vector2 _movement;
        private float _moveSpeed = 5.5f;
        private float _diagonalLimiter = 0.65f;

        internal void InputCheck()
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");
        }
        internal void Running()
        {
            // Limita o movimento na diagonal se o jogador estiver a carregar em duas teclas
            // de moviemnto em simultãneo
            if (_movement.x != 0 && _movement.y != 0)
            {
                _movement.x *= _diagonalLimiter;
                _movement.y *= _diagonalLimiter;
            }

            // O rigidobody do jogador recebe um novo vector2 que inicia o seu movimento
            Vector2 _playerVelocity = new(_movement.x * _moveSpeed, _movement.y * _moveSpeed);
            _rb.velocity = _playerVelocity;
        }
    }
}
