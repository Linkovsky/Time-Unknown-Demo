using RPG.ScriptableObjects.Duplicate;
using System.Collections;
using TMPro;
using UnityEngine;

namespace RPG.UI.Dialogue
{

    public class DialogueUI : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private GameObject _dialogueBox;
        [SerializeField] private GameObject _continueArrow;
        [SerializeField] internal NPCDialogueActivator _dialogueActivator;

        internal PreventDuplicates _preventDuplicates;
        internal ScrollDialogue _scrollDialogue;
        internal NPCDialogueAndStairs _npcFadeOut;
        public bool isOpen { get; private set; }

        private TypeWriterEffect _typeWritrerEffect;
        /// <summary>
        /// Variável recebe o seu respetivo componente.
        /// </summary>
        private void Start()
        {
            _typeWritrerEffect = GetComponent<TypeWriterEffect>();
        }
        /// <summary>
        /// Função que inicia a converça, ativando o gameobject da caixa de diálogo
        /// e inicia uma coroutina para mostrar as letras ao jogador
        /// </summary>
        /// <param name="dialogueObject">Recebe o SO da fala do NPC</param>
        public void ShowDialogue(DialogueObject dialogueObject)
        {
            isOpen = true;
            _dialogueBox.SetActive(true);
            StartCoroutine(StepThroughDialogue(dialogueObject));
        }
        /// <summary>
        /// É criada uma string name que é usada como o nome do NPC
        /// depois é usado um for loop para ler todas as falas que estão dentro do SO
        /// é utilizada outra string que vai guardar as falas que estão naquele index usando um yield return
        /// chamamos outra coroutina ou seja a primeira tem de esperar que a segunda acaba, neste caso está é para
        /// mostrar de letra a letra. Quando ela acaba volta a dar continuidade e espera pelo input do jogador
        /// antes de mostrar a próxima fala se houver.
        /// </summary>
        /// <param name="dialogueObject">Recebe o SO da função ShowDialogue</param>
        /// <returns>A conversa do NPC</returns>
        private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
        {
            string name = dialogueObject.Name;

            for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
            {
                string dialogue = dialogueObject.Dialogue[i];

                _continueArrow.SetActive(false);

                yield return RunningTypingEffect(dialogue, name);

                _text.text = dialogue;

                _continueArrow.SetActive(true);


                yield return null;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Mouse0));
            }

            CloseDialogueBox();

        }
        /// <summary>
        /// A coroutina que é chamada para mostrar letra a letra o diálogo do NPC
        /// caso o jogador dê um input a conversa toda é  mostrada
        /// </summary>
        private IEnumerator RunningTypingEffect(string dialogue, string name)
        {
            _typeWritrerEffect.Run(dialogue, _text, name, _name);

            while (_typeWritrerEffect.IsRunning)
            {
                yield return null;
                if (Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Mouse0))
                {
                    _typeWritrerEffect.Stop();
                }
            }
        }
        /// <summary>
        /// Fecha a caixa de diálogo e verifica se algo vai acontecer no final da conversa
        /// </summary>
        public void CloseDialogueBox()
        {
            if (_npcFadeOut != null)
                StartCoroutine(_npcFadeOut.MakeStairsAppear());
            if (_preventDuplicates != null)
                if (_preventDuplicates.name == "PreventSwordFromAppearing" && _preventDuplicates.numberOfTimes == 0)
                {
                    _preventDuplicates.numberOfTimes++;
                    _dialogueActivator.SetActiveItem();
                }
            if (_dialogueActivator != null)
                if (_dialogueActivator._dissolve)
                    _dialogueActivator.StartDissolving();
            if (_scrollDialogue != null && _scrollDialogue.canShrink)
                StartCoroutine(_scrollDialogue.ShrinkItem());
            isOpen = false;
            _dialogueBox.SetActive(false);
            _text.text = string.Empty;
            _name.text = string.Empty;
        }
    }
}
