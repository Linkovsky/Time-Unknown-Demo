using RPG.Entities;
using RPG.Entities.DamageResources;
using RPG.Inventory;
using RPG.PlayerControlls;
using RPG.SceneManagement;
using RPG.UI.Dialogue;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Declaração de variáveis
    /// </summary>
    public static bool playerDead = false;

    [SerializeField] private Inventory _playerInventory;
    [SerializeField] private AudioClip _sword;
    [SerializeField] private AudioClip _hurt;
    [SerializeField] private float _speed;
    [SerializeField] private GameObject _itemToThrow;
    public IInteractable interactable { get; set; }

    public bool stopPlayer = false;
    public bool playerHurt = false;

    public DialogueUI DialogueUI => dialogueUI;
    private DialogueUI dialogueUI;

    private InventoryUI _inventoryUI;
    private Coroutine flashCoroutine = null;
    private SpriteRenderer _sr;
    private InputControls _inputControls;
    private PlayerAnimations _playerAnimations;
    private AudioSource _playerSounds;

    private Vector2 _dir;

    private bool _isAttacking = false;

    /// <summary>
    /// Atribuído os componentes às variáveis
    /// </summary>
    private void Awake()
    {
        _inputControls = ScriptableObject.CreateInstance<InputControls>();
        _playerAnimations = ScriptableObject.CreateInstance<PlayerAnimations>();
        _inputControls._rb = GetComponent<Rigidbody2D>();
        _playerAnimations._animator = GetComponent<Animator>();
        dialogueUI = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<DialogueUI>();
        _playerSounds = GetComponent<AudioSource>();
        _sr = GetComponent<SpriteRenderer>();
        _inventoryUI = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<InventoryUI>();
    }

    /// <summary>
    /// A cada frame verifica se o menu do inventário e se o booleano do jogador é verdadeiro o que impede
    /// o jogador de poder efetuar qualquer comando
    /// Procura dentro do inventário se o jogador contém o item da espada para poder atacar, o mesmo para o shuriken
    /// E se está perto de um NPC que ativa o isOpen e poder iniciar o diálogo
    /// </summary>
    private void Update()
    {
        
        if (_inventoryUI.isOpen || playerDead || stopPlayer || Time.timeScale == 0 || LevelManager.Instance.isLoading) return;
        if (Input.GetKeyDown(KeyCode.LeftShift) && HasItem("Shuriken"))
        {
            if (dialogueUI.isOpen) return;
            StartCoroutine(ShootItem());
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && HasItem("Sword"))
        {
            if (dialogueUI.isOpen) return;
            else
            {
                if (_playerAnimations._animator.GetFloat("Idle_V") == 0 && _playerAnimations._animator.GetFloat("Idle_H") == 0)
                    StartCoroutine(IdleAttack());
                else if (!_isAttacking)
                    StartCoroutine(Attack());
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && dialogueUI.isOpen == false)
        {
            _inputControls._rb.velocity = Vector2.zero;
            interactable?.Interact(this);
        }
        _inputControls.InputCheck();
    }

    /// <summary>
    /// Em cada fixupdate antes do Update ser chamado verificar se o os booleanos estão verdadeiros para impedir
    /// que as os comandos usando o sistema de fisica não aconteça
    /// Consoante o menu que esteja aberto, para o jogador de estar na animação de running e passa a idle na posição correta
    /// </summary>
    private void FixedUpdate()
    {
        if (playerDead || LevelManager.Instance.isLoading)
        {
            _inputControls._rb.velocity = Vector2.zero; return;
        }
        AbsMovement(out var modifiedX, out var modifiedY);


        if (_playerAnimations._animator.GetBool("Item"))
        {
            _inputControls._rb.velocity = Vector2.zero;

        }
        if (stopPlayer)
        {
            _inputControls._rb.velocity = Vector2.zero;
            _playerAnimations.FlipSprite(_inputControls._rb, modifiedX, modifiedY);
            return;
        }

        if (_isAttacking)
        {
            _inputControls._rb.velocity = Vector2.zero;
            return;
        }
        if (dialogueUI.isOpen || _inventoryUI.isOpen)
        {
            _inputControls._rb.velocity = Vector2.zero;
            _playerAnimations.FlipSprite(_inputControls._rb, modifiedX, modifiedY);
            return;
        }
        _inputControls.Running();
        _playerAnimations.FlipSprite(_inputControls._rb, modifiedX, modifiedY);

    }

    /// <summary>
    /// Determina um valor absoluto 
    /// </summary>
    /// <param name="modifiedX">X</param>
    /// <param name="modifiedY">Y</param>
    private void AbsMovement(out float modifiedX, out float modifiedY)
    {
        modifiedX = _inputControls._movement.x;
        modifiedY = _inputControls._movement.y;
        if (Math.Abs(modifiedX) != 0)
            modifiedY = 0;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Verdadeiro - Se o inventário contém o nome do item</returns>
    private bool HasItem(string name)
    {
        return _playerInventory._inventory.Any(itemName => itemName.itemData.name == name);
    }

    /// <summary>
    /// Coroutina que faz com que quando o jogo inicia e o jogador carregar logo no ataque como o jogador está virado para a frente
    /// pode então atacar
    /// </summary>
    /// <returns>Nothing</returns>
    private IEnumerator IdleAttack()
    {
        _isAttacking = true;
        _playerAnimations._animator.SetBool("isAttacking", true);
        _playerAnimations._animator.Play("Attack_Front");
        _playerSounds.PlayOneShot(_sword);
        yield return new WaitForSeconds(0.167f);
        _playerAnimations._animator.SetBool("isAttacking", false);
        _isAttacking = false;
    }
    /// <summary>
    /// Coroutina que faz com que o toque a animação de atacar
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack()
    {
        _isAttacking = true;
        _playerAnimations._animator.SetBool("isAttacking", true);
        _playerSounds.PlayOneShot(_sword);
        yield return new WaitForSeconds(0.167f);
        _isAttacking = false;
        _playerAnimations._animator.SetBool("isAttacking", false);
    }
    /// <summary>
    /// Coroutina quando o jogador leva dano a sua sprite é convertida para a cor vermelho e branco intercalado
    /// </summary>
    /// <returns></returns>
    private IEnumerator Flash()
    {
        _sr.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        _sr.material.color = Color.white;
        playerHurt = false;
        flashCoroutine = null;
    }

    /// <summary>
    /// Função de dano do jogador que verifica s eo jogador está morto e chama a coroutina para mudar de côr
    /// </summary>
    public void PlayerHurt()
    {
        if (playerDead) return;
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        _playerSounds.PlayOneShot(_hurt);
        flashCoroutine = StartCoroutine(Flash());
    }

    public void PlayerHeal()
    {
        if (playerDead) return;
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashGreen());
    }
    private IEnumerator FlashGreen()
    {
        _sr.material.color = Color.green;
        yield return new WaitForSeconds(.1f);
        _sr.material.color = Color.white;
        flashCoroutine = null;
    }
    /// <summary>
    /// Coroutina que dá a habilidade ao jogador de poder mandar shurikens, utilizado um gameobject como referência
    /// de onde o jogador está virado instancia o shuriken naquela direção
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootItem()
    {
        _isAttacking = true;
        _playerAnimations._animator.SetBool("isThrowing", true);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Transform childLocation = transform.GetChild(3).transform;
        PositionRelativeTo(childLocation);
        GameObject obj = Instantiate(_itemToThrow, childLocation.position, Quaternion.identity);
        obj.GetComponent<ShurikenScript>().Instantiated(_dir);
        yield return new WaitForSeconds(0.1f);
        _playerAnimations._animator.SetBool("isThrowing", false);
        _isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Blocks"))
        {
            collision.gameObject.GetComponent<PushBlocks>().MoveBlock(_inputControls._movement);

        }
    }

    private void PositionRelativeTo(Transform child)
    {
        Vector3 playerRelativeToObject = child.transform.position - transform.position;

        float x = playerRelativeToObject.x;
        float y = playerRelativeToObject.y;
        //UP
        if (y > 0 && (x > -y && x < y))
        {
            _dir = Vector2.up;
        }

        //RIGHT
        else if (x > 0 && (y > -x && y < x))
        {
            _dir = -Vector2.left;
        }

        //LEFT
        else if (x < 0 && (y > x && y < -x))
        {
            _dir = Vector2.left;
        }

        //DOWN
        else if (y < 0 && (x > y && x < -y))
        {
            _dir = Vector2.down;
        }
    }
}