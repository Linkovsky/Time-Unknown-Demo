using RPG.PlayerControlls.Health;
using UnityEngine;

namespace RPG.Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        /// <summary>
        /// Declara��o das vari�veis
        /// </summary>
        [Header("AI Movement Speed")]
        [SerializeField] private float _speed;
        [Header("Time to stop")]
        [SerializeField] private float _startWaitTime, _collideTime;
        [Header("Transforms of Player and Movement")]
        [SerializeField] private Transform _moveSpot;
        [Header("Boundaries and Radious")]
        [SerializeField] private float _minX, _maxX, _minY, _maxY, _searchRadious;
        [SerializeField] private float _impulseForce;

        private Rigidbody2D _rb;
        internal Animator _animator;
        internal Transform _player;

        internal bool _isDead;
        internal bool _playerCollide = false;
        private bool _isMoving;
        private float _waitTime, _collideWaitTime;

        /// <summary>
        /// Atribuindo o respetivo componente nas vari�veis
        /// </summary>
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        private void Start()
        {
            _waitTime = _startWaitTime;
            _moveSpot.position = GetRandomPositionForAI();
        }
        /// <summary>
        /// A cada frame verificar se o booleano de isDead � verdadeiro para parar o inimigo, e verifica se o inimigo est� perto do 
        /// jogador ou se colidiu com o mesmo, assim como verificar se o jogador est� em game over state
        /// </summary>
        private void Update()
        {
            if (_isDead) return;
            if (!CheckIfPlayerIsNear() && !_playerCollide)
            {
                GetRandomPosition();
                AIMovement();
            }
            else if (CheckIfPlayerIsNear() && !_playerCollide)
            {
                if (PlayerController.playerDead) return;
                AIChansingPlayer();
                RunTowardsPlayer();
            }
            if (_playerCollide)
            {
                WaitTime();
            }
        }
        /// <summary>
        /// Cada vez que colidir com algo a vari�vel _collideWaitTime decremente junto com Time.deltaTime at� chegar a 0 valores
        /// para poder voltar a se movimentar. Uso do rigidbody constraints para dar freeze a qualquer movimento aleat�rio que possa
        /// surgir.
        /// </summary>
        private void WaitTime()
        {
            if (_collideWaitTime <= 0f)
            {
                _collideWaitTime = _collideTime;
                _playerCollide = false;
                _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            else
            {
                _rb.constraints = RigidbodyConstraints2D.FreezeAll;
                _collideWaitTime -= Time.deltaTime;
                StopAndFace();
            }

        }
        /// <summary>
        /// Verifica a anima��o atual e consoante o resultado muda de anima��o
        /// </summary>
        private void StopAndFace()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walk_Left"))
                _animator.Play("Idle_Left");
            else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walk_Right"))
                _animator.Play("Idle_Right");
            else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walk_Front"))
                _animator.Play("Idle_Front");
            else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walk_Back"))
                _animator.Play("Idle_Back");
        }
        /// <summary>
        /// _rb.transform.position � utilizado o m�todo movetowards que junta a posi��o do inimigo + uma random position.
        /// </summary>
        private void AIMovement()
        {
            _rb.transform.position = Vector2.MoveTowards(_rb.transform.position, _moveSpot.position, _speed * Time.deltaTime);
        }
        /// <summary>
        /// O mesmo que AIMovement excepto que em vez da posi��o random � a posi��o atual do jogador
        /// </summary>
        private void AIChansingPlayer()
        {
            _rb.transform.position = Vector2.MoveTowards(_rb.transform.position, _player.transform.position, _speed * Time.deltaTime);
        }

        /// <summary>
        /// Verifica se a posi��o entre o inimigo e o ponto aleat�rio corresponde a menos que 0.2 unidades de movimento 
        /// se sim, Inicia outro ciclo de espera e depois quando chega a 0 valores vai buscar outra random position
        /// </summary>
        private void GetRandomPosition()
        {
            if (Vector2.Distance(_rb.transform.position, _moveSpot.position) < 0.2f)
            {
                if (_waitTime <= 0)
                {
                    _moveSpot.position = GetRandomPositionForAI();
                    _waitTime = _startWaitTime;
                }
                else
                {
                    _waitTime -= Time.deltaTime;
                    StopAndFace();
                    _isMoving = false;
                }
            }
            else if (!_isMoving)
            {
                _isMoving = true;
                Run();
            }
        }

        /// <summary>
        /// playerRelativeToObject = Posi��o do random position - a posi��o do inimigo
        /// E verifica dentro das coordenadas X e Y se est� a se mover para cima, baixo, esquerda ou direita
        /// </summary>
        private void Run()
        {
            Vector3 playerRelativeToObject = _moveSpot.transform.position - _rb.transform.position;

            float x = playerRelativeToObject.x;
            float y = playerRelativeToObject.y;

            //UP
            if (y > 0 && (x > -y && x < y))
            {
                _animator.Play("Walk_Back");
            }

            //RIGHT
            else if (x > 0 && (y > -x && y < x))
            {
                _animator.Play("Walk_Right");
            }

            //LEFT
            else if (x < 0 && (y > x && y < -x))
            {
                _animator.Play("Walk_Left");
            }

            //DOWN
            else if (y < 0 && (x > y && x < -y))
            {
                _animator.Play("Walk_Front");
            }
        }

        /// <summary>
        /// O mesmo que o de cima excepto que em vez do uso da posi��o random � usada o do jogador
        /// </summary>
        private void RunTowardsPlayer()
        {
            Vector3 playerRelativeToObject = _player.transform.position - transform.position;

            float x = playerRelativeToObject.x;
            float y = playerRelativeToObject.y;

            //UP
            if (y > 0 && (x > -y && x < y))
            {
                _animator.Play("Walk_Back");
            }

            //RIGHT
            else if (x > 0 && (y > -x && y < x))
            {
                _animator.Play("Walk_Right");
            }

            //LEFT
            else if (x < 0 && (y > x && y < -x))
            {
                _animator.Play("Walk_Left");
            }

            //DOWN
            else if (y < 0 && (x > y && x < -y))
            {
                _animator.Play("Walk_Front");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Se a dist�ncia entre o jogador ou o inimigo � Verdadeiro ou Falso</returns>
        private bool CheckIfPlayerIsNear()
        {
            return Vector2.Distance(_player.position, _rb.transform.position) <= _searchRadious;
        }
        /// <summary>
        /// Consoante as coordenadas entre min e max de X e Y introduzidas no Editor retorna um valor
        /// </summary>
        /// <returns>Novas coordenadas para movimentar o inimigo</returns>
        private Vector2 GetRandomPositionForAI()
        {
            return new Vector2(Random.Range(_minX, _maxX), Random.Range(_minY, _maxY));
        }

        /// <summary>
        /// Verifica se a colis�o ocorreu, se sim, recebe o componente de vida do player e d� dano
        /// </summary>
        /// <param name="collision">Representa o objeto que ir� colidir com o collider</param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _playerCollide = true;
                collision.gameObject.GetComponent<PlayerHealth>().PlayerHurt(2);
            }
        }

        /// <summary>
        /// Verifica se a colis�o deixou de existir com o jogador.
        /// </summary>
        /// <param name="collision">Representa o objeto que ir� sair da colis�o com o collider</param>
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _playerCollide = false;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _searchRadious);
        }
    }
}
