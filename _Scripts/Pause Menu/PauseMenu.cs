using RPG.SaveSystem;
using RPG.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Entities
{
    public class PauseMenu : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private GameObject _pauseMenuUI;
        [SerializeField] private GameObject _pauseMenuUI2;
        [SerializeField] private GameObject _pauseMenuUI3;
        [SerializeField] private GameObject _pauseMenuUI4;
        [SerializeField] private bool _isPaused = false;
        [SerializeField] private SavingFile _saveFile;
        [SerializeField] private GameObject _objectToShow;
        [SerializeField] private Sprite _sprite;
        private WaitForSeconds _sleep;

        private void Awake()
        {
            _sleep = new WaitForSeconds(1f);
        }
        /// <summary>
        /// Em cada frame verifica se a scene atual é 0 ou 1 e impede de o menu ser aprsentado
        /// </summary>
        private void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0 
                || SceneManager.GetActiveScene().buildIndex == 1
                || SceneManager.GetActiveScene().buildIndex == 10
                || LevelManager.Instance.isLoading) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isPaused = !_isPaused;
            }

            if (_isPaused)
            {
                ActiveMenu();
            }
            else
            {
                DeactivateMenu();
            }
        }

        /// <summary>
        /// Mostra o menu na tela e para o tempo de jogo
        /// </summary>
        private void ActiveMenu()
        {
            _pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
        }

        /// <summary>
        /// Desativa todo o painel
        /// E retoma o tempo de jogo
        /// </summary>
        private void DeactivateMenu()
        {
            Time.timeScale = 1f;
            _pauseMenuUI.SetActive(false);
            _pauseMenuUI2.SetActive(false);
            _pauseMenuUI3.SetActive(false);
            _pauseMenuUI4.SetActive(false);
        }

        /// <summary>
        /// Quando pressionado o botão resume muda o booleano que diz se o menu pode ser desativado
        /// </summary>
        public void ResumeButton()
        {
            _isPaused = !_isPaused;
            DeactivateMenu();
        }

        /// <summary>
        /// Método para poder gravar o estado atual do jogo
        /// </summary>
        public void SaveGame()
        {
            _saveFile.SaveData();
            StartCoroutine(Confirmation());
        }

        /// <summary>
        /// Método para poder carregar o jogo
        /// </summary>
        public void LoadGame()
        {
            _isPaused = !_isPaused;
            _saveFile.LoadData();
            DeactivateMenu();
        }

        /// <summary>
        /// Botão de quit pressionado manda o jogador de volta ao menu principal e desativa o menu
        /// </summary>
        public void StartMenu()
        {
            _isPaused = !_isPaused;
            DeactivateMenu();
            LevelManager.Instance.StartCoroutine(LevelManager.Instance.TransitionCoroutine("StartMenu", _sprite));
        }

        private IEnumerator Confirmation()
        {
            _objectToShow.SetActive(true);
            yield return _sleep;
            _objectToShow.SetActive(false);
        }
    }
}