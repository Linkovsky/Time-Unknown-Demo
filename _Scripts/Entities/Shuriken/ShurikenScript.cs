using RPG.Enemy;
using UnityEngine;

namespace RPG.Entities.DamageResources
{
    public class ShurikenScript : MonoBehaviour
    {
        /// <summary>
        /// Declara��o de vari�veis
        /// </summary>
        [SerializeField] private float _speed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private AudioClip _audioClip;

        private Vector3 _dir;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.PlayOneShot(_audioClip);
        }
        /// <summary>
        /// Uma vez o jogador carregando no shift o shuriken � instanciado dependendo da dire��o esquerda
        /// ou direita o shuriken � rodado
        /// </summary>
        /// <param name="dir">Dire��o a que se encontra o jogador</param>
        public void Instantiated(Vector2 dir)
        {
            if (dir == -Vector2.left)
                _rotationSpeed = -_rotationSpeed;
            _dir = dir;
            Destroy(gameObject, 4f);
        }
        /// <summary>
        /// Cada frame a posi��o � alterada consoante a posi��o que falta e a velocidade, mesmo com a rota��o do mesmo
        /// </summary>
        private void Update()
        {
            transform.position += _dir * _speed * Time.deltaTime;
            transform.Rotate(new Vector3(0, 0, _rotationSpeed * 3f * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("LastBoss"))
            {
                collision.gameObject.GetComponentInChildren<EnemyHealth>().Damage(1);
                Destroy(gameObject);
            }
            else
                Destroy(gameObject);

        }
    }
}