using RPG.Inventory.Interface;
using System;
using System.Collections;
using UnityEngine;
using RPG.UI.HealthSystem;
namespace RPG.Inventory
{
    public class Item : MonoBehaviour, ICollectible
    {
        /// <summary>
        /// Evento que faz com que qualquer classe que faça subscribe ao evento corra algum método quando 
        /// o mesmo é fired.
        /// </summary>
        public static event HandleItemCollected OnItemCollected;

        public delegate void HandleItemCollected(ItemData itemData, int num);

        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private ItemData newItemData;
        [SerializeField] private AudioClip _audio;
        [SerializeField] private int _amount;
        [SerializeField] private bool _increaseHeartContainer;

        public bool canCollect;

        private AudioSource _audioSource;
        private CircleCollider2D _collider;
        private HealthUI _healthUI;

        /// <summary>
        /// Atribuição do componente à sua respetiva variável
        /// </summary>
        private void Start()
        {
            _audioSource = GameObject.FindGameObjectWithTag("UI").GetComponent<AudioSource>();
            _collider = GetComponent<CircleCollider2D>();
            _healthUI = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<HealthUI>();
        }

        /// <summary>
        /// Colisão em trigger
        /// Testa se o col representa o player, se sim testa se o item equivale a um heartcontainer e se pode ser coletado
        /// Se for Heart, incrementa +1 coração
        /// </summary>
        /// <param name="col">Representa o objeto que entra em colisão</param>
        private void OnTriggerEnter2D(Collider2D col)
        {

            if (!col.CompareTag("Player")) return;
            if (_increaseHeartContainer)
            {
                _healthUI.IncreaseTotalHearts();
                return;
            }
            if (_collider != null)
                 _collider.enabled = false;
            col.TryGetComponent(out PlayerController _player);
            if (canCollect)
                StartCoroutine(CollectItem(_player));
            else
                StartCoroutine(GotItemDialogue(_player));
        }

        /// <summary>
        /// Coroutina onde aciona o evento para que o mesmo possa correr noutros scripts que tenham feito subscribe
        /// toca um som e move o item para cima do player, e o item é adicionado ao inventário
        /// </summary>
        /// <param name="player">Recebe o componente do jogador como parâmetro de entrada</param>
        /// <returns>Nothing</returns>
        public IEnumerator CollectItem(PlayerController player)
        {
            OnItemCollected?.Invoke(newItemData, _amount);
            transform.position = player.gameObject.transform.GetChild(2).position;
            _audioSource.PlayOneShot(_audio);
            yield return null;
        }

        /// <summary>
        /// Coroutina de diálogo onde o item não é adicionado ao inventário
        /// </summary>
        /// <param name="player">Recebe o componente do jogador como parâmetro de entrada</param>
        /// <returns>Nothing</returns>
        private IEnumerator GotItemDialogue(PlayerController player)
        {
            transform.position = player.gameObject.transform.GetChild(2).position;
            _audioSource.PlayOneShot(_audio);
            yield return null;
        }
    }
}