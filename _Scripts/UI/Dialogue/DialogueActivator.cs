using RPG.NPC.Movement;
using UnityEngine;
namespace RPG.UI.Dialogue
{

    public class DialogueActivator : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// Scipt referodp ao diálogo, cada vez que o jogador entra em colisão e o script verifica que está ao pé de um npc
        /// usa a interface para dizer que o objeto do diálogo caso o jogador venha falar com o npc é o que escolher no 
        /// edito usando a variável _dialogueObject
        /// </summary>
        [SerializeField] private DialogueObject _dialogueObject;
        [SerializeField] private GameObject _balloon;


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
            {
                _balloon.SetActive(true);
                player.interactable = this;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
            {
                _balloon.SetActive(false);
                if (player.interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
                {
                    player.interactable = null;
                }
            }
        }
        public void Interact(PlayerController player)
        {

            player.DialogueUI.ShowDialogue(_dialogueObject);
        }
    }
}
