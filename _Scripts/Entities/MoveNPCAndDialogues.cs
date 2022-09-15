using UnityEngine;

namespace RPG.UI.Dialogue
{
    public class MoveNPCAndDialogues : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private DialogueObject[] _newDialogues;
        [SerializeField] private NPCDialogueActivator[] _npcNewDialogueReceiver;
        [SerializeField] private GameObject _npc;
        [SerializeField] private Transform _npcNewPos;
        [SerializeField] private Inventory.Inventory _inventory;

        /// <summary>
        /// Ao início da scene se o inventário for diferente de 0 muda de diálogos e de posições
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
        /// Para cada ponto no array dos objetos mudar para os diálogos escolhidos
        /// </summary>
        public void ChangeDialogues()
        {
            for (int i = 0; i < _npcNewDialogueReceiver.Length; i++)
            {
                _npcNewDialogueReceiver[i]._dialogueObject = _newDialogues[i];
            }
        }

        /// <summary>
        /// Muda a posição do NPC
        /// </summary>
        public void ChangePosition()
        {
            _npc.transform.position = _npcNewPos.position;
        }
    }
}