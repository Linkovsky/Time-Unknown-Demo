using RPG.NPC.Movement;
using RPG.ScriptableObjects.Duplicate;
using System.Collections;
using UnityEngine;
namespace RPG.UI.Dialogue
{
    /// <summary>
    /// Os comentários nesta classe são os mesmos que no dialoguestairs só muda algumas coisas
    /// </summary>
    public class NPCDialogueActivator : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// Declaração das variáveis
        /// </summary>
        [SerializeField] public DialogueObject _dialogueObject;
        [SerializeField] private GameObject _balloon;
        [SerializeField] private NpcMovement _npcMovement;
        [SerializeField] private DialogueUI _dialogueUI;
        [SerializeField] private PreventDuplicates _number;
        [SerializeField] private GameObject _specialItem;
        [SerializeField] private bool _deliverItem;
        [SerializeField] internal bool _dissolve;

        private float _startValue = 0f;
        private float _endValue = 1f;

        private Material _spriteRenderer;

        /// <summary>
        /// Verifica se o SO estiver com um valor diferente de 0 e se estiver destrói o objeto
        /// E se o booleano for verdadeiro que se decide dentro do Editor adiciona o componente
        /// </summary>
        private void Start()
        {
            if (_number != null)
                if (_number.numberOfTimes != 0)
                    Destroy(transform.root.gameObject);
            _dialogueUI = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<DialogueUI>();
            if (_dissolve)
            {
                _spriteRenderer = GetComponentInParent<SpriteRenderer>().material;
            }
        }
        /// <summary>
        /// Quando entra no collider se for o jogador então o objeto do balão da fala é ativado
        /// tal como os booleanos para dizer que o jogador está ao pé do NPC.
        /// </summary>
        /// <param name="collision">Colisão do gameobject</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
            {
                _balloon.SetActive(true);
                _npcMovement.isPlayerCloseToNPC = true;
                _dialogueUI._dialogueActivator = this;
                _dialogueUI._preventDuplicates = _number;
                if (_number != null && _number.name != "PreventSwordFromAppearing")
                    _number.numberOfTimes++;
                player.interactable = this;
            }
        }

        /// <summary>
        /// Cada frame verifica se o diálogo continua aberto
        /// </summary>
        private void Update()
        {
            if (!_dialogueUI.isOpen)
                return;
            _balloon.SetActive(false);
        }

        /// <summary>
        /// Quando sair do collider tudo o que foi ativado no triggerenter é tudo colocado a nulo para qu~e o jogador
        /// não tenha mais acesso a algo "invisível"
        /// </summary>
        /// <param name="collision">Colisão do jogador</param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
            {
                _balloon.SetActive(false);
                _npcMovement.isPlayerCloseToNPC = false;
                if (player.interactable is NPCDialogueActivator dialogueActivator && dialogueActivator == this)
                {
                    player.interactable = null;
                    _dialogueUI._preventDuplicates = null;
                    _dialogueUI._dialogueActivator = null;
                }
            }
        }
        public void Interact(PlayerController player)
        {
            player.DialogueUI.ShowDialogue(_dialogueObject);
        }
        public void SetActiveItem()
        {
            if (_deliverItem)
            {
                _specialItem.SetActive(true);
                _deliverItem = false;
            }
        }

        public void StartDissolving()
        {
            if (_dissolve)
            {
                StartCoroutine(DissolveNPC());
                return;
            }
        }
        /// <summary>
        /// Coroutina que é utilizada no final do demo para dissolver um npc que pretendo fazer como se fosse um fantasma
        /// Depois de estar com o float do material a 0 o objeto é destruido da scene
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator DissolveNPC()
        {
            float currentTime = 0f;
            float transitionTime = 1f;

            while (currentTime < transitionTime)
            {
                currentTime += Time.deltaTime * 2;
                float totalTime = currentTime / transitionTime;
                float value = Mathf.Lerp(_startValue, _endValue, totalTime);
                _spriteRenderer.SetFloat("_DissolveAmount", value);
                yield return null;
            }
            Destroy(transform.root.gameObject);
        }
    }
}
