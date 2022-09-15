using RPG.PlayerControlls.Health;
using System.Collections;
using UnityEngine;

namespace RPG.Entities.DamageResources
{
    public class FireBall : MonoBehaviour
    {
        /// <summary>
        /// Declara��o de vari�veis
        /// </summary>
        [SerializeField] private float _speed;
        [SerializeField] private int _damage;
        [SerializeField] private AudioClip _audioClip;

        private PlayerHealth _health;
        private PlayerController _controller;
        private AudioSource _audioSource;
        private WaitForSeconds _sleep;
        private Transform _player;
        private Vector2 _target;
        private Animator _animator;
        private CircleCollider2D _collider;
        private bool _stopMoving;

        /// <summary>
        /// Atribu� cada componente � sua respetiva vari�vel
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _health = _player.GetComponent<PlayerHealth>();
            _controller = _player.GetComponent<PlayerController>();
            _collider = GetComponent<CircleCollider2D>();
        }
        private void Start()
        {
            _sleep = new WaitForSeconds(0.667f);
            _audioSource.clip = _audioClip;
            _audioSource.PlayOneShot(_audioClip);
            _target = new Vector2(_player.position.x, _player.position.y);
        }

        /// <summary>
        /// Em cada frame testa se o a FireBall deve continuar a se mover ou se entrou em colis�o
        /// </summary>
        private void Update()
        {
            if (!_stopMoving)
            {
                transform.position = Vector2.MoveTowards(transform.position, _target, _speed * Time.deltaTime);

            }

            if ((Vector2)transform.position == _target)
            {
                StartCoroutine(Collided());
            }
        }

        /// <summary>
        /// Collider da fireball como trigger, quando entra em colis�o teste se for � o inimigo n�o faz nada
        /// Se for qualquer outro objeto ent�o inicia a colis�o, caso seja o player junto com a colis�o ainda d� dano ao jogador
        /// </summary>
        /// <param name="collision">Representa o objeto � qual a fireball entrou em contacto</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy")) return;
            
            if (collision.CompareTag("Player"))
            {
                _collider.enabled = false;
                _stopMoving = true;
                _health.PlayerHurt(_damage);
                _controller.playerHurt = true;
                _controller.PlayerHurt();
                StartCoroutine(Collided());
            }
            else if(collision.gameObject.layer == 3 || collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
            {
                _collider.enabled = false;
                _stopMoving = true;
                StartCoroutine(Collided());
            }
        }

        /// <summary>
        /// Coroutina da colis�o que d� o efeito que explos�o
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator Collided()
        {
            _animator.Play("Destroy");
            yield return _sleep;
            Destroy(gameObject);
        }
    }
}