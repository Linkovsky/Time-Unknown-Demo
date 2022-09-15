using DG.Tweening;
using RPG.ScriptableObjects.Duplicate;
using RPG.ScriptableObjects.List;
using UnityEngine;

namespace RPG.Entities
{
    public class PushBlocks : MonoBehaviour
    {
        [SerializeField] private PreventDuplicates _preventMovement;
        [SerializeField] private ListSO _block;
        [SerializeField] private bool _justmove;

        /// <summary>
        /// Verifica se o objeto já foi movimentado pelo menos 1 vez e impede de uma segunda.
        /// Caso o movimento seja de um objeto então faz um raycast2d na posição a que o jogador se encontra
        /// um ray sempre na direção a frente. Teste se tem algum objeto ao lado se der falso pode prosseguir para mover.
        /// </summary>
        /// <param name="dir">Recebe a direção do jogador no momento da colisão</param>
        public void MoveBlock(Vector2 dir)
        {

            if (_preventMovement.numberOfTimes != 0) return;
            if (_justmove)
            {
                var newPos = (Vector2)transform.position + dir;
                transform.DOMove(newPos, 1f);
                _preventMovement.numberOfTimes++;
                return;
            }
            if (!_justmove)
            {
                _block.AddToList(gameObject, transform.position);

                // Esta parte está toda a funcionar
                var newPos = (Vector2)transform.position + dir;
                var hit = Physics2D.Raycast(transform.position, dir, 1f);

                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Walls")
                        || hit.collider.CompareTag("Camera")
                        || hit.collider.CompareTag("Blocks")) return;
                }

                transform.DOMove(newPos, 1f);
                _preventMovement.numberOfTimes++;
            }
        }

        /// <summary>
        /// Função asyncrona que vai busar a lista dos objetos e a inverte começando no último elemento
        /// e vai buscar a lista das posições invertendo a mesma com o mesmo objetivo.
        /// Move os objetos para a sua posição original e só move o próximo (await) quando o que se está a mover
        /// acabou de efetuar o movimento.
        /// </summary>
        public async void ResetPositions()
        {
            var _blocks = _block.GetList();
            var _blocksPos = _block.GetListPos();
            for (int i = 0; i < _blocks.Count; i++)
            {
                _blocks[i].GetComponent<PushBlocks>()._preventMovement.numberOfTimes--; // Dá oportunidade de volta ao player para mover o bloco
                await _blocks[i].transform.DOMove(_blocksPos[i], 1f).AsyncWaitForCompletion(); //Depois do ultimo bloco movido passa ao próximo
                _blocksPos[i] = new Vector2(0, 0); // Reseta o array de volta a 0,0 para adicionar de novo o transform do bloco
            }

            _blocks.Clear();
            _blocksPos.Clear();
        }
    }
}

