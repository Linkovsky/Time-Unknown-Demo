using RPG.Enemy;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Enemy.Boss
{
    public class LastBossBattle : MonoBehaviour
    {
        /// <summary>
        /// Declara��o das variaveis
        /// </summary>
        public bool _startBattle;

        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private AudioClip _audioClip2;

        [SerializeField] private GameObject _targetDestination;
        [SerializeField] private Image _healthImage;
        [SerializeField] private Transform _player;
        [SerializeField] private EnemyHealth _bossHealth;
        [SerializeField] private float _radious;
        [SerializeField] private float _speed;
        [SerializeField] private float _rangeToAttack;

        private Animator _animator;
        private AudioSource _audioSource;
        private WaitForSeconds _waitForAttacking;
        private WaitForSeconds _waitAfterAttacking;
        private float _waitTime = 0f;
        private float _timeToWait = 1.5f;

        private bool _isAttacking;
        private bool _isMoving = true;

        /// <summary>
        /// Adicionar os respetivos componentes �s vari�veis.
        /// </summary>
        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _animator = GetComponent<Animator>();
            _bossHealth = GetComponentInChildren<EnemyHealth>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _waitForAttacking = new WaitForSeconds(0.2f);
            _waitAfterAttacking = new WaitForSeconds(0.750f);
            _animator.Play("LastBoss_Idle");
        }

        /// <summary>
        /// Em cada frame testa se a vida atual do boss � menor que 50, testa tamb�m a que distancia est� o jogador e se est� em range 
        /// de o inimigo se mover ou de o inimigo atacar
        /// </summary>
        private void Update()
        {
            if (_bossHealth._currentHealth < 50)
            {
                _timeToWait /= 2f;
            }

            if (_startBattle)
            {
                if (Vector2.Distance((Vector2)_player.position, (Vector2)transform.position) < _rangeToAttack && !_isAttacking)
                {
                    _isMoving = false;
                    _isAttacking = true;
                    StartCoroutine(StartAttacking());
                }
                else if (Vector2.Distance((Vector2)_player.position, (Vector2)transform.position) < _radious && !_isAttacking && _isMoving)
                {
                    if (_waitTime < 0.1f)
                    {
                        _waitTime = _timeToWait;
                        TrackPlayerPosition();
                    }

                    if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("LastBoss_Move"))
                    {
                        _animator.Play("LastBoss_Move");
                        _audioSource.PlayOneShot(_audioClip);
                    }

                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(_targetDestination.transform.position.x
                                                                                            , _targetDestination.transform.position.y + 1f)
                                                                                            , _speed * Time.deltaTime);

                    if ((Vector2)transform.position == new Vector2(_targetDestination.transform.position.x, _targetDestination.transform.position.y + 1f))
                        _isMoving = false;
                }
                else if (!_isMoving && !_isAttacking)
                {
                    _animator.Play("LastBoss_Idle");
                    _waitTime -= Time.deltaTime;
                    if (_waitTime < 0.1f)
                        _isMoving = true;
                }
            }
        }

        /// <summary>
        /// Move o gameobject que � usado para a pr�xima posi��o do inimigo para ser igual � posi��o daquele exato momento
        /// do jogador + 1f no Y para que o boss possa ir para tr�s do jogador pois devido ao seu �nico ataque ser pela frente
        /// </summary>
        private void TrackPlayerPosition()
        {
            _targetDestination.transform.position = _player.position;
        }

        /// <summary>
        /// Inicia a anima��o de atacar e coloca os booleanos para impedir de se movimentar e que poder� atacar
        /// </summary>
        /// <returns>Espera pelo tempo j� definido acima no m�todo Start</returns>
        private IEnumerator StartAttacking()
        {
            _animator.Play("LastBoss_Charge");
            yield return _waitForAttacking;
            _animator.Play("LastBoss_Attack");
            _audioSource.PlayOneShot(_audioClip2);
            yield return _waitAfterAttacking;
            _animator.Play("LastBoss_Idle");
            _isAttacking = false;
            _isMoving = true;

        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _radious);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _rangeToAttack);
        }
    }
}