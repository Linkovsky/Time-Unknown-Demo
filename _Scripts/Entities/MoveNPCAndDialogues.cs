using UnityEngine;

namespace RPG.UI.Dialogue
{
    public class MoveNPCAndDialogues : MonoBehaviour
    {
        /// <summary>
        /// Declara��o de vari�veis
        /// </summary>
        [SerializeField] private DialogueObject[] _newDialogues;
        [SerializeField] private NPCDialogueActivator[] _npcNewDialogueReceiver;
        [SerializeField] private GameObject _npc;
        [SerializeField] private Transform _npcNewPos;
        [SerializeField] private Inventory.Inventory _inventory;

        /// <summary>
        /// Ao in�cio da scene se o invent�rio for diferente de 0 muda de di�logos e de posi��es
        /// </summary>
        private void Start()
        {
            if (_inventory._inventory.Count != 0)
            {
                ChangeDialogues();
                ChangePosition();
                Destroy(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_inventory._inventory.Count != 0)
            {
                ChangeDialogues();
                ChangePosition();
                Destroy(this);
            }
        }

        /// <summary>
        /// Para cada ponto no array dos objetos mudar para os di�logos escolhidos
        /// </summary>
        public void ChangeDialogues()
        {
            for (int i = 0; i < _npcNewDialogueReceiver.Length; i++)
            {
                _npcNewDialogueReceiver[i]._dialogueObject = _newDialogues[i];
            }
        }

        /// <summary>
        /// Muda a posi��o do NPC
        /// </summary>
        public void ChangePosition()
        {
            _npc.transform.position = _npcNewPos.position;
        }
    }
}