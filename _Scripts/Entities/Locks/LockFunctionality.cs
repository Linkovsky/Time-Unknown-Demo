using RPG.Inventory;
using RPG.ScriptableObjects.Duplicate;
using RPG.UI.Dialogue;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RPG.Entities.Chest
{
    public class LockFunctionality : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private Inventory.Inventory _inventory;
        [SerializeField] public DialogueObject _dialogueObject;
        [SerializeField] private PreventDuplicates _ifKeyUsedPreventAppearing;
        [SerializeField] private float _startValue;
        [SerializeField] private float _endValue;

        private Material _dissolveMaterial;
        private PlayerController _player;
        private BoxCollider2D _boxCollider;

        private bool _inRange;

        /// <summary>
        /// Atribui o respetivo componente às variáveis
        /// </summary>
        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _dissolveMaterial = GetComponent<TilemapRenderer>().material;
        }

        /// <summary>
        /// A cada frame verifica se o SO está a 0 o que permite ao Objeto estar visível
        /// Modifica o valor do shader material para 0 para estar visivel e ativa o boxcollider
        /// Quando o jogador carrega no E 
        /// </summary>
        private void Update()
        {
            if (_ifKeyUsedPreventAppearing.numberOfTimes == 0)
            {
                _dissolveMaterial.SetFloat("_DissolveAmount", 0f);
                _boxCollider.enabled = true;
                if (_inRange)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (CheckForKey()) return;
                        _player.interactable = this;
                        _player.interactable?.Interact(_player);
                    }
                }
            }
            else
            {
                _dissolveMaterial.SetFloat("_DissolveAmount", 1f);
                _boxCollider.enabled = false;
            }
        }

        /// <summary>
        /// Pesquisa no inventário do jogador se possui a chave necessária para abrir o lock
        /// </summary>
        /// <returns>Verdadeiro ou falso consoante o que o jogador tenha no inventário</returns>
        private bool CheckForKey()
        {
            foreach (InventoryItem item in _inventory._inventory)
            {
                switch (item.itemData.displayName)
                {
                    case "Boss Key":
                        if (gameObject.name == "BossLock")
                        {
                            _inventory.Remove(item.itemData, 1);
                            StartCoroutine(StartDissolving());
                            _ifKeyUsedPreventAppearing.numberOfTimes++;
                            _boxCollider.enabled = false;
                            _inRange = false;
                            return true;
                        }
                        break;
                    case "Small Key":
                        if (gameObject.name != "BossLock")
                        {
                            _inventory.Remove(item.itemData, 1);
                            StartCoroutine(StartDissolving());
                            _ifKeyUsedPreventAppearing.numberOfTimes++;
                            _boxCollider.enabled = false;
                            _inRange = false;
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
        /// <summary>
        /// Coroutina que inicia parando o jogador durante o processo e modifica o material do dissolve dentro 
        /// de um tempo especifico para que vá do valor 0 a 1 (invisivel)
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator StartDissolving()
        {
            _player.stopPlayer = true;
            float currentTime = 0f;
            float transitionTime = 1f;
            while (currentTime < transitionTime)
            {
                currentTime += Time.deltaTime * 2;
                float totalTime = currentTime / transitionTime;
                float value = Mathf.Lerp(_startValue, _endValue, totalTime);
                _dissolveMaterial.SetFloat("_DissolveAmount", value);
                yield return null;
            }
            _player.stopPlayer = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out PlayerController player))
            {
                _inRange = true;
                _player = player;

            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out PlayerController player))
            {
                if (player.interactable is LockFunctionality dialogueActivator && dialogueActivator == this)
                {
                    player.interactable = null;
                    _player = null;
                    _inRange = false;

                }
            }

        }

        public void Interact(PlayerController player)
        {
            player.DialogueUI.ShowDialogue(_dialogueObject);
        }
    }
}