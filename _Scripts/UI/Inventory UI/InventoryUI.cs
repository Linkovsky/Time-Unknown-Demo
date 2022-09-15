using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPG.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        /// <summary>
        /// Declara��o de vari�veis
        /// </summary>
        [SerializeField] private GameObject _uiPanel, _uiName;
        [SerializeField] private Image[] _itemImage;
        [SerializeField] private TextMeshProUGUI[] _itemName;
        [SerializeField] private TextMeshProUGUI[] _itemStack;
        [SerializeField] private Inventory _inventory;

        public bool isOpen = false;
        /// <summary>
        /// Quando inicia o jogo, � escondido o UI do jogador, bem como colocar todo o texto em empty
        /// </summary>
        private void Start()
        {
            HideUI();
            ResetText();
            PrepareUI(_inventory);
        }
        /// <summary>
        /// Em cada frame verifica se o jogador pressiona a tecla TAB para abrir o invent�rio
        /// </summary>
        private void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0
                || SceneManager.GetActiveScene().buildIndex == 1
                || SceneManager.GetActiveScene().buildIndex == 10
                || LevelManager.Instance.isLoading) return;
            if (Input.GetKeyDown(KeyCode.Tab) && !isOpen && !PlayerController.playerDead)
            {
                PrepareUI(_inventory);
                ShowUI();
                isOpen = true;
                return;
            }
            else if (Input.GetKeyDown(KeyCode.Tab) && isOpen)
            {
                HideUI();
                isOpen = false;
                return;
            }
        }
        /// <summary>
        /// Torna os objetos que estavam desativados como true
        /// </summary>
        private void ShowUI()
        {
            _uiPanel.SetActive(true);
            _uiName.SetActive(true);
        }
        /// <summary>
        /// Torna os objetos que estavam ativados como false
        /// </summary>
        private void HideUI()
        {
            _uiPanel.SetActive(false);
            _uiName.SetActive(false);
        }
        /// <summary>
        /// D� reset a todo o texto dos objetos da lista
        /// </summary>
        private void ResetText()
        {
            for (int i = 0; i < _itemName.Length; i++)
            {
                _itemStack[i].text = string.Empty;
                _itemName[i].text = string.Empty;
                _itemName[i].gameObject.SetActive(false);
                _itemStack[i].gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// Dentro de um for todos os objetos s�o colocados a falso para n�o mostrar imagens brancas
        /// No segundo for dentro da lista qualquer objeto que contenha alguma informa��o � apresentado no ecr�
        /// tal como o nome do mesmo e o respetivo n�mero de stack
        /// </summary>
        private void PrepareUI(Inventory item)
        {
            for (int i = 0; i < _itemImage.Length; i++)
            {
                _itemImage[i].gameObject.SetActive(false);
            }
            ResetText();
            for (int i = 0; i < item._inventory.Count; i++)
            {
                if (item._inventory[i] != null)
                {
                    _itemImage[i].gameObject.SetActive(true);
                    _itemImage[i].sprite = item._inventory[i].itemData.icon;
                    _itemName[i].gameObject.SetActive(true);
                    _itemName[i].text = item._inventory[i].itemData.displayName;
                    _itemStack[i].gameObject.SetActive(true);
                    _itemStack[i].text = item._inventory[i].stackSize.ToString();
                }
            }
        }
    }
}