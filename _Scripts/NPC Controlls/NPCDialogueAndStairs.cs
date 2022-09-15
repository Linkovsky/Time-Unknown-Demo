using RPG.NPC.Movement;
using RPG.SceneManagement;
using RPG.ScriptableObjects.Duplicate;
using System.Collections;
using UnityEngine;
namespace RPG.UI.Dialogue
{

    public class NPCDialogueAndStairs : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] public DialogueObject _dialogueObject;
        [SerializeField] private DialogueObject _newDialogue;
        [SerializeField] private GameObject _balloon;
        [SerializeField] private NpcMovement _npcMovement;
        [SerializeField] private Material _fadeOutMaterial;
        [SerializeField] private PreventDuplicates _preventDoubleTimes;
        [SerializeField] private AudioClip _magicalSound;
        [SerializeField] private bool _turnCollider;

        private DialogueUI _dialogueUI;
        private BoxCollider2D _collider;

        /// <summary>
        /// Variáveis recebem os componentes
        /// Verifica se o SO de prevenção de duplicados foi modificado, para colocar as escadas visiveis ou não
        /// </summary>
        private void Start()
        {
            _dialogueUI = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<DialogueUI>();
            _collider = GameObject.FindGameObjectWithTag("Stairs").GetComponentInChildren<BoxCollider2D>();
            if (_preventDoubleTimes.numberOfTimes > 0)
            {
                if (_turnCollider)
                    _collider.enabled = true;
                _dialogueObject = _newDialogue;
                _fadeOutMaterial.SetFloat("_fade", 1f);
            }
            else
            {
                _fadeOutMaterial.SetFloat("_fade", 0f);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
            {
                _balloon.SetActive(true);
                _npcMovement.isPlayerCloseToNPC = true;
                player.interactable = this;
                _dialogueUI._npcFadeOut = this;
            }
        }

        private void Update()
        {
            if (!_dialogueUI.isOpen)
                return;
            _balloon.SetActive(false);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
            {
                _balloon.SetActive(false);
                _npcMovement.isPlayerCloseToNPC = false;
                _dialogueUI._npcFadeOut = null;
                if (player.interactable is NPCDialogueAndStairs dialogueActivator && dialogueActivator == this)
                {
                    player.interactable = null;
                }
            }
        }

        public void Interact(PlayerController player)
        {
            player.DialogueUI.ShowDialogue(_dialogueObject);
        }

        /// <summary>
        /// Testa mais uma vez se o SO foi modificado, se não, faz com que as escadas apareçam usando o material do dissolve
        /// </summary>
        /// <returns>Nothing</returns>
        public IEnumerator MakeStairsAppear()
        {

            if (_preventDoubleTimes.numberOfTimes == 0)
            {

                float transitionTime = 1f;
                float startValue = 0f;
                float endValue = 1f;
                float currentTime = 0f;

                while (currentTime < transitionTime)
                {
                    currentTime += Time.deltaTime * 0.5f;
                    float totalTime = currentTime / transitionTime;
                    float value = Mathf.Lerp(startValue, endValue, totalTime);
                    _fadeOutMaterial.SetFloat("_fade", value);
                    yield return null;
                }
                if (_turnCollider)
                    _collider.enabled = true;
                LevelManager.Instance.GetComponent<AudioSource>().PlayOneShot(_magicalSound);
                _preventDoubleTimes.numberOfTimes++;
                _dialogueObject = _newDialogue;
            }
        }
    }
}
