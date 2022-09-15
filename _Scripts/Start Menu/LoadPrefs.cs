using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace RPG.StartMenu
{
    public class LoadPrefs : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [Header("General Settings")]
        [SerializeField] private bool _canUse = false;
        [SerializeField] private MenuController _menuController;

        [Header("Volume Settings")]
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private TMP_Text _masterVolumeTextValue = null;
        [SerializeField] private Slider _masterVolumeSlider = null;
        [SerializeField] private TMP_Text _musicVolumeTextValue = null;
        [SerializeField] private Slider _musicVolumeSlider = null;
        [SerializeField] private TMP_Text _sfxVolumeTextValue = null;
        [SerializeField] private Slider _sfxVolumeSlider = null;

        [Header("Quality Level Settings")]
        [SerializeField] private TMP_Dropdown _qualityDropDown;

        [Header("Fullscreen Settings")]
        [SerializeField] private Toggle _fullScreenToggle;
        /// <summary>
        /// Script usado apenas na primeira Scene onde quando o jogador modifica os valores de som ou
        /// de resolução, sempre que voltar ao menu principal estes valores teêm de voltar a ser carregados
        /// aos valores escolhidos pelo jogador
        /// Utiliza o PlayerPrefs para is buscar todas as chaves e os valores respetivos o mesmo que sobre os gráficos
        /// Mathf.Log10, tem qu eser convertido o valor linear para décibel para poder atualizar a frequência de som
        /// </summary>
        private void Start()
        {
            if (_canUse)
            {
                if (PlayerPrefs.HasKey("masterVolume"))
                {
                    float masterVolume = PlayerPrefs.GetFloat("masterVolume");

                    _masterVolumeTextValue.text = masterVolume.ToString("0.0");
                    _masterVolumeSlider.value = masterVolume;
                    masterVolume = Mathf.Log10(masterVolume) * 20f;
                    _mixer.SetFloat("Master", masterVolume);
                    if (PlayerPrefs.HasKey("musicVolume"))
                    {
                        float musicVolume = PlayerPrefs.GetFloat("musicVolume");
                        _musicVolumeTextValue.text = musicVolume.ToString("0.0");
                        _musicVolumeSlider.value = musicVolume;
                        musicVolume = Mathf.Log10(musicVolume) * 20f;
                        _mixer.SetFloat("Music", musicVolume);
                        if (PlayerPrefs.HasKey("SFXVolume"))
                        {
                            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
                            _sfxVolumeTextValue.text = sfxVolume.ToString("0.0");
                            _sfxVolumeSlider.value = sfxVolume;
                            sfxVolume = Mathf.Log10(sfxVolume) * 20f;
                            _mixer.SetFloat("Music", sfxVolume);
                        }
                    }
                }
                else
                {
                    _menuController.ResetButton("Audio");
                }
                if (PlayerPrefs.HasKey("masterQuality"))
                {
                    int localQuality = PlayerPrefs.GetInt("masterQuality");
                    _qualityDropDown.value = localQuality;
                    QualitySettings.SetQualityLevel(localQuality);
                }

                if (PlayerPrefs.HasKey("masterFullscreen"))
                {
                    int localFullscreen = PlayerPrefs.GetInt("masterFullscreen");
                    if (localFullscreen == 1)
                    {
                        Screen.fullScreen = true;
                        _fullScreenToggle.isOn = true;
                    }
                    else
                    {
                        Screen.fullScreen = false;
                        _fullScreenToggle.isOn = false;
                    }
                }
            }
        }
    }
}