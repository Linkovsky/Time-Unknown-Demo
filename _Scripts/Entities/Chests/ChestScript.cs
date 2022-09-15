using RPG.ScriptableObjects.Duplicate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Entities.Chest
{
    public class ChestScript : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private PreventDuplicates _redChest;
        [SerializeField] private GameObject _item;
        [SerializeField] private AudioClip _chestOpenSfx;

        private Animator _animator;
        private AudioSource _audioSource;
        private bool isPlayerNear;

        
        /// <summary>
        /// Atribuí o seu componente a cada variável
        /// </summary>
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audioSource = GameObject.FindGameObjectWithTag("UI").GetComponent<AudioSource>();
        }
        private void Start()
        {
            if (_redChest.numberOfTimes > 0 || _redChest.numberOfTimes > 0 && gameObject.activeSelf == false)
            {
                _animator.SetBool("isChestOpened", true);
            }
        }
        /// <summary>
        /// A cada frame verifica se o jogador está perto do objeto, se der verdadeiro, e o jogador carregar na tecla E
        /// Inicia a coroutina para abrir o chest
        /// </summary>
        private void Update()
        {
            if (isPlayerNear)
            {
                if (!_animator.GetBool("isChestOpened"))
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        StartCoroutine(OpenChest());
                    }
            }
        }

        /// <summary>
        /// Collider em trigger e verifica se quem entrou dentro do collider estiver com a tag "Player"
        /// Case verdadeiro então modifica o booleano para verdadeiro para o jogador poder abrir o chest
        /// </summary>
        /// <param name="collision">Representa o objecto ao qual ocorre a colisão</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && !_animator.GetBool("isChestOpened"))
            {
                isPlayerNear = true;
            }
        }
        /// <summary>
        /// Quando o colisor for o Player e este sair da colisão então move o booleano para falso, não 
        /// dando a oportunidade de abrir o chest fora das suas boundaries
        /// </summary>
        /// <param name="collision">O objeto sai da colisão</param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && !_animator.GetBool("isChestOpened"))
            {
                isPlayerNear = false;
            }
        }
        /// <summary>
        /// Coroutina que inicia a abertura do chest, tocando um som e colocando o Game Object que
        /// o jogador vai capturar a verdadeiro
        /// E incrementa +1 ao SO para que o jogador não possa voltar a apanhar o mesmo item
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator OpenChest()
        {
            _audioSource.PlayOneShot(_chestOpenSfx);
            yield return new WaitForSeconds(0.2f);
            _item.SetActive(true);
            _animator.SetBool("isChestOpened", true);
            _redChest.numberOfTimes++;
        }
    }
}