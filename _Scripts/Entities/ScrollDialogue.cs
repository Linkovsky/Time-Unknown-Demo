using System.Collections;
using UnityEngine;

namespace RPG.UI.Dialogue
{
    public class ScrollDialogue : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// Declara��o de vari�veis
        /// </summary>
        [SerializeField] public DialogueObject _dialogueObject;

        public bool canShrink;

        private CircleCollider2D _circleCollider2D;
        private DialogueUI _dialogueUI;
        private Animator _player;

        internal int allowShrink = 0;
        private float _duration = 0.3f;

        /// <summary>
        /// Respetivas vari�veis recebem o seu componente
        /// </summary>
        private void Start()
        {
            _dialogueUI = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<DialogueUI>();
            _circleCollider2D = GetComponent<CircleCollider2D>();
        }
        /// <summary>
        /// Sempre que o jogador entrar em contacto com o collider do item, ele autom�ticamente inicia o dialogo atrav�s da interface 
        /// IInteractable onde passamos a refer�ncia do di�logo e de seguida a colocamos a nulo para n�o poder voltar a ter o di�logo,
        /// Depois � feito o shrink do item at� ser reduzido a 0
        /// </summary>
        /// <param name="collision">Representa o objeto que entrou em colis�o</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player) && collision.TryGetComponent(out Animator animator))
            {
                canShrink = true;
                allowShrink++;
                if (_circleCollider2D != null)
                {
                    _circleCollider2D.offset = new Vector2(_circleCollider2D.offset.x, -0.05442619f);

                    _circleCollider2D.radius = 0.6718159f;
                }
                _player = animator;
                _player.SetBool("Item", true);
                _dialogueUI._scrollDialogue = this;
                player.interactable = this;
                player.interactable?.Interact(player);
                if (player.interactable is ScrollDialogue dialogueActivator && dialogueActivator == this)
                {
                    player.interactable = null;
                }
            }
        }

        /// <summary>
        /// O di�logo passa a ser o di�logo selecionado no Editor
        /// </summary>
        /// <param name="player">Acesso ao componente do PlayerController</param>
        public void Interact(PlayerController player)
        {
            player.DialogueUI.ShowDialogue(_dialogueObject);
        }
        /// <summary>
        /// In�cio da coroutina onde reduzimos o tamanho do objeto at� chegar a 0 e depois o destruimos
        /// </summary>
        /// <returns>Nothing</returns>
        public IEnumerator ShrinkItem()
        {
            Vector3 startScale = transform.localScale;
            Vector3 endScale = Vector3.zero;
            float currentTime = 0;
            while (currentTime < _duration)
            {
                currentTime += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / _duration);
                yield return null;
            }
            transform.localScale = endScale;
            _player.SetBool("Item", false);
            Destroy(gameObject);
        }
    }
}