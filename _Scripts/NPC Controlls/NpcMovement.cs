using RPG.UI.Dialogue;
using UnityEngine;

namespace RPG.NPC.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class NpcMovement : MonoBehaviour
    {
        /// <summary>
        /// Declaração das variáveis
        /// </summary>
        [Header("NPC Walk Zone")]
        [SerializeField] private Collider2D _walkZone;
        [Header("Movement Params")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _walkTime;
        [SerializeField] private float _waitTime;
        [Header("Walking Pattern")]
        [SerializeField] private bool _rightAndLeft;
        [SerializeField] private bool _upAndDown;

        [HideInInspector]
        public bool isPlayerCloseToNPC;
        // Dois vectores que correspondem ás bordas do Walk Zone Trigger Collider
        private Vector2 _minWalkPoint, _maxWalkPoint;
        
        private float _walkCounter;
        private float _waitCounter;
        private int _walkDirection;

        private DialogueUI _dialogueUI;
        private Transform _playerTransform;
        private Rigidbody2D _rb;
        private Animator _animator;

        private bool _isWalking = false;
        private bool _hasWalkZone = false;

        // Constantes do animator
        private const string NPC_WALKING_LEFT = "Walking_Left";
        private const string NPC_WALKING_RIGHT = "Walking_Right";
        private const string NPC_WALKING_UP = "Walking_Back";
        private const string NPC_WALKING_DOWN = "Walking_Front";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
        }
        /// <summary>
        /// Adiciona o componente às variáveis
        /// Chama a função de escolher uma posição aleatória
        /// E cria os boundaries em forma de quadrado
        /// </summary>
        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            _dialogueUI = FindObjectOfType<DialogueUI>();
            
            _waitCounter = _waitTime;
            _walkCounter = _walkTime;

            ChooseDirection();
            if (_walkZone != null)
            {
                _minWalkPoint = _walkZone.bounds.min;
                _maxWalkPoint = _walkZone.bounds.max;
                _hasWalkZone = true;
            }
        }
        /// <summary>
        /// Em cada frame vai verificando o estado do NPC, se o jogador está perto do NPC
        /// Se o NPC chegou a zona final do movimento escolhido aleatório, se sim inicia um cooldown para voltar
        /// a se movimentar
        /// </summary>
        private void Update()
        {
            if (isPlayerCloseToNPC)
            {
                _rb.velocity = Vector2.zero;
                StopMovement();
                if (_dialogueUI.isOpen)
                {
                    FacingPlayer();
                    return;
                }
                return;
            }
            if (!_rightAndLeft && !_upAndDown)
                return;

            else if (_isWalking)
            {
                _walkCounter -= Time.deltaTime;

                MovingNPC();

                if (_walkCounter < 0)
                {
                    _isWalking = false;
                    _waitCounter = _waitTime;
                }
            }
            else
            {
                StopMovement();
                _waitCounter -= Time.deltaTime;
                _rb.velocity = Vector2.zero;
                if (_waitCounter < 0)
                {
                    ChooseDirection();
                }
            }
        }
        // Switch que recebe a direção escolhida aleatória e que depois entra no caso a que se refere
        // Onde chama a função que inicia a animação para o lado a que o npc se move
        private void MovingNPC()
        {
            switch (_walkDirection)
            {
                case 0:
                    StartMoving(NPC_WALKING_LEFT);
                    Vector2 npcVelocity = new(-_moveSpeed, 0);
                    _rb.velocity = npcVelocity;

                    if (_hasWalkZone && transform.position.x < _minWalkPoint.x)
                    {
                        _isWalking = false;
                        _waitCounter = _waitTime;
                    }
                    break;
                case 1:
                    StartMoving(NPC_WALKING_RIGHT);
                    npcVelocity = new(_moveSpeed, 0);
                    _rb.velocity = npcVelocity;

                    if (_hasWalkZone && transform.position.x > _maxWalkPoint.x)
                    {
                        _isWalking = false;
                        _waitCounter = _waitTime;
                    }
                    break;

                case 2:
                    StartMoving(NPC_WALKING_UP);
                    npcVelocity = new(0, _moveSpeed);
                    _rb.velocity = npcVelocity;

                    if (_hasWalkZone && transform.position.y > _maxWalkPoint.y)
                    {
                        _isWalking = false;
                        _waitCounter = _waitTime;
                    }
                    break;
                case 3:
                    StartMoving(NPC_WALKING_DOWN);
                    npcVelocity = new(0, -_moveSpeed);
                    _rb.velocity = npcVelocity;

                    if (_hasWalkZone && transform.position.y < _minWalkPoint.y)
                    {
                        _isWalking = false;
                        _waitCounter = _waitTime;
                    }
                    break;
            }
        }
        // Escolha de direção aleatória consoante o que permito que o NPC faça
        private void ChooseDirection()
        {
            if (_rightAndLeft && !_upAndDown)
                _walkDirection = Random.Range(0, 2);
            if (!_rightAndLeft && _upAndDown)
                _walkDirection = Random.Range(2, 4);
            if (_rightAndLeft && _upAndDown)
                _walkDirection = Random.Range(0, 4);
            _isWalking = true;
            _walkCounter = _walkTime;
        }

        // Recebe como parametro uma string value com o nome da animação
        private void StartMoving(string stateName)
        {
            _animator.Play(stateName);
        }

        // Se a condição de animações for igual a um dos nomes propostos
        // Mudar o estado da animação para idle
        private void StopMovement()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walking_Left"))
                _animator.Play("Idle_Left");
            else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walking_Right"))
                _animator.Play("Idle_Right");
            else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walking_Front"))
                _animator.Play("Idle_Front");
            else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walking_Back"))
                _animator.Play("Idle_Back");
        }
        private void FacingPlayer()
        {
            Vector3 playerRelativeToObject = _playerTransform.transform.position - transform.position;

            float x = playerRelativeToObject.x;
            float y = playerRelativeToObject.y;

            //UP
            if (y > 0 && (x > -y && x < y))
            {
                _animator.Play("Idle_Back");
            }

            //RIGHT
            else if (x > 0 && (y > -x && y < x))
            {
                _animator.Play("Idle_Right");
            }

            //LEFT
            else if (x < 0 && (y > x && y < -x))
            {
                _animator.Play("Idle_Left");
            }

            //DOWN
            else if (y < 0 && (x > y && x < -y))
            {
                _animator.Play("Idle_Front");
            }
        }
    }
}