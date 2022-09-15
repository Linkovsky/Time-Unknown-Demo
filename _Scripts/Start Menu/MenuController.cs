using RPG.SaveSystem;
using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPG.StartMenu
{
    public class MenuController : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private Sprite somthing;
        [SerializeField] private Inventory.Inventory _inventory;
        [SerializeField] private bool _isFullScreen;
        [Header("Volume Settings")]
        [SerializeField] private AudioMixer _audioMixer = null;
        [SerializeField] private TMP_Text _masterVolumeTextValue = null;
        [SerializeField] private Slider _masterVolumeSlider = null;
        [SerializeField] private TMP_Text _musicVolumeTextValue = null;
        [SerializeField] private Slider _musicVolumeSlider = null;
        [SerializeField] private TMP_Text _sfxVolumeTextValue = null;
        [SerializeField] private Slider _sfxVolumeSlider = null;
        [SerializeField] private float _defaultVolume = 1.0f;
        [Header("Resolution Dropdowns")]
        public TMP_Dropdown resolutionDropDown;
        private Resolution[] _resolutions;

        [Header("Graphics Settings")]
        [Space(10)]
        [SerializeField] private TMP_Dropdown _qualityDropDown;
        [SerializeField] private Toggle _fullScreenToggle;


        [Header("Confirmation")]
        [SerializeField] private GameObject _confirmationPrompt = null;


        [SerializeField] private GameObject _noSavedGameDialogue = null;

        private SavingFile _saveSystem;
        private WaitForSeconds _wait;
        private int _qualityLevel;
        private void Awake()
        {
            _wait = new WaitForSeconds(2f);
        }

        /// <summary>
        /// Quando o menu principal é iniciado, são logo estabilicidos valores de padrão para cada variável.
        /// É feita um scan ao número total de resoluções possíveis que poderá apresentar no ecrã do jogador
        /// E depois a partir de um for até ao total de resoluções encontradas é adicionada a uma string a chave dessa resolução
        /// que depois é atribuida a um dropdown menu
        /// </summary>
        private void Start()
        {

            _saveSystem = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<SavingFile>();
            _resolutions = Screen.resolutions;
            resolutionDropDown.ClearOptions();

            List<string> options = new();

            int currentResolutionIndex = 0;

            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;
                options.Add(option);

                if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
                    currentResolutionIndex = i;
            }

            resolutionDropDown.AddOptions(options);
            resolutionDropDown.value = currentResolutionIndex;
            resolutionDropDown.RefreshShownValue();
        }
        /// <summary>
        /// Utilizado num botão para quando o jogador iniciar um new game apagar tudo do inventário
        /// </summary>
        public void ResetAll()
        {
            _inventory.ResetAll();
        }
        /// <summary>
        /// Através do dropdown menu é escolhido a resolução pretendida pelo jogador
        /// </summary>
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
        /// <summary>
        /// Utilizado num botão que faz load ao início do jogo
        /// </summary>
        public void NewGameDialogueYes()
        {
            LevelManager.Instance.LoadScene("Prelude", somthing);
        }
        /// <summary>
        /// Utilizado num botão para fazer load game e verifica se já existe algum save file para carregar
        /// </summary>
        public void LoadGameDialogueYes()
        {
            if (File.Exists(_saveSystem._path))
            {
                _saveSystem.LoadData();
            }
            else
            {
                _noSavedGameDialogue.SetActive(true);
            }
        }
        /// <summary>
        /// Utilizado num botão quando carregado sai da aplicação
        /// </summary>
        public void ExitButton()
        {
            Application.Quit();
        }
        /// <summary>
        /// Usado no slider interativo e dynamico, sempre que o player muda o slider
        /// para trás ou para a frente muda sempre o valor e faz overwrite
        /// </summary>
        public void SetMaster(float volume)
        {
            _audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
            _masterVolumeTextValue.text = volume.ToString("0.0");
            PlayerPrefs.SetFloat("masterVolume", volume);
            PlayerPrefs.Save();
        }
        /// <summary>
        /// Usado no slider interativo e dynamico, sempre que o player muda o slider
        /// para trás ou para a frente muda sempre o valor e faz overwrite
        /// </summary>
        public void SetMusic(float volume)
        {
            _audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
            _musicVolumeTextValue.text = volume.ToString("0.0");
            PlayerPrefs.SetFloat("musicVolume", volume);
            PlayerPrefs.Save();
        }
        /// <summary>
        /// Usado no slider interativo e dynamico, sempre que o player muda o slider
        /// para trás ou para a frente muda sempre o valor e faz overwrite
        /// </summary>
        public void SetSfx(float volume)
        {
            _audioMixer.SetFloat("Fx", Mathf.Log10(volume) * 20);
            _sfxVolumeTextValue.text = volume.ToString("0.0");
            PlayerPrefs.SetFloat("SFXVolume", volume);
            PlayerPrefs.Save();
        }
        /// <summary>
        /// Inicia a coroutina de confirmação das modificações
        /// </summary>
        public void VolumeApply()
        {
            StartCoroutine(ConfirmationBox());
        }
        /// <summary>
        /// Usado num botão quando o jogador carrega em reset to default values
        /// Procura pelo nome do menu que está a ser utilizado de momento e conforme o menu
        /// modifica as variáveis para valores padrão 
        /// </summary>
        /// <param name="menuType">Nome do menu em que o jogador está</param>
        public void ResetButton(string menuType)
        {
            if (menuType == "Audio")
            {
                _masterVolumeSlider.value = _defaultVolume;
                _masterVolumeTextValue.text = _defaultVolume.ToString("0.0");
                _musicVolumeSlider.value = _defaultVolume;
                _musicVolumeTextValue.text = _defaultVolume.ToString("0.0");
                _sfxVolumeSlider.value = _defaultVolume;
                _sfxVolumeTextValue.text = _defaultVolume.ToString("0.0");
                VolumeApply();
            }

            if (menuType == "Graphics")
            {
                _qualityDropDown.value = 3;
                QualitySettings.SetQualityLevel(3);

                _fullScreenToggle.isOn = true;
                Screen.fullScreen = true;

                Resolution currentResolution = Screen.currentResolution;
                Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
                resolutionDropDown.value = _resolutions.Length;
                GraphicsApply();
            }
        }
        /// <summary>
        /// Usado num botão que muda o booleano de fullscreen para verdadeiro ou falso
        /// </summary>
        /// <param name="isFullScreen"></param>
        public void SetFullScreen(bool isFullScreen)
        {
            _isFullScreen = isFullScreen;
        }
        /// <summary>
        /// Usado num botão que muda o número int do nível de qulidade dos gráficos
        /// </summary>
        /// <param name="qualityIndex"></param>
        public void SetQuality(int qualityIndex)
        {
            _qualityLevel = qualityIndex;
        }
        /// <summary>
        /// Usado sempre que o jogador sai do menu é adicionado e guardado no playerprefs as respetivas 
        /// referências dos gráficos
        /// </summary>
        public void GraphicsApply()
        {
            PlayerPrefs.SetInt("masterQuality", _qualityLevel);
            QualitySettings.SetQualityLevel(_qualityLevel);

            PlayerPrefs.SetInt("masterFullScreen", _isFullScreen ? 1 : 0);
            Screen.fullScreen = _isFullScreen;

            StartCoroutine(ConfirmationBox());
        }
        public IEnumerator ConfirmationBox()
        {
            _confirmationPrompt.SetActive(true);
            yield return _wait;
            _confirmationPrompt.SetActive(false);
        }
    }
}