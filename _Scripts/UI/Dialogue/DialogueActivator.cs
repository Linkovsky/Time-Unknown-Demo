using RPG.NPC.Movement;
using UnityEngine;
namespace RPG.UI.Dialogue
{

    public class DialogueActivator : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// Scipt referodp ao di�logo, cada vez que o jogador entra em colis�o e o script verifica que est� ao p� de um npc
        /// usa a interface para dizer que o objeto do di�logo caso o jogador venha falar com o npc � o que escolher no 
        /// edito usando a vari�vel _dialogueObject
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
