using UnityEngine;

namespace RPG.PlayerControlls
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimations : ScriptableObject
    {
        internal Animator _animator;

        internal void FlipSprite(Rigidbody2D _rb, float x, float y)
        {
            // Vira a personagem consoante os valores do Input que o utilizador dá
            // moveInput.x -1 Flips Left - moveInput.x 1 Flips Right e o mesmo para o eixo Y
            _animator.SetFloat("Horizontal", x);
            _animator.SetFloat("Vertical", y);
            _animator.SetFloat("Velocity", _rb.velocity.sqrMagnitude);

            // Se o input for diferente de 0 então o player está parado e a personagem fica a olhar para a direção do ultimo 
            // valor recebido pelo método Run()
            if (_rb.velocity != Vector2.zero)
            {
                _animator.SetFloat("Idle_H", x);
                _animator.SetFloat("Idle_V", y);
            }
        }
    }
}
