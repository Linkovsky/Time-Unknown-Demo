using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace RPG.StartMenu
{
    public class AudioLoadPrefs : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [Header("Volume Settings")]
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private TMP_Text _masterVolumeTextValue = null;
        [SerializeField] private Slider _masterVolumeSlider = null;
        [SerializeField] private TMP_Text _musicVolumeTextValue = null;
        [SerializeField] private Slider _musicVolumeSlider = null;
        [SerializeField] private TMP_Text _sfxVolumeTextValue = null;
        [SerializeField] private Slider _sfxVolumeSlider = null;

        /// <summary>
        /// Assim que o gameobject se torna ativo na scene verifica se o playerprefs 
        /// tem as chaves das strings do volume e se for verdadeiro atribui os valores às variáveis do volume
        /// </summary>
        private void OnEnable()
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
        }
        /// <summary>
        /// Usado no slider interativo e dynamico, sempre que o player muda o slider
        /// para trás ou para a frente muda sempre o valor e faz overwrite
        /// </summary>
        /// <param name="volume"></param>
        public void SetMaster(float volume)
        {
            _mixer.SetFloat("Master", Mathf.Log10(volume) * 20);
            _masterVolumeTextValue.text = volume.ToString("0.0");
            PlayerPrefs.SetFloat("masterVolume", volume);
            PlayerPrefs.Save();
        }
        /// <summary>
        /// Usado no slider interativo e dynamico, sempre que o player muda o slider
        /// para trás ou para a frente muda sempre o valor e faz overwrite
        /// </summary>
        /// <param name="volume"></param>
        public void SetMusic(float volume)
        {
            _mixer.SetFloat("Music", Mathf.Log10(volume) * 20);
            _musicVolumeTextValue.text = volume.ToString("0.0");
            PlayerPrefs.SetFloat("musicVolume", volume);
            PlayerPrefs.Save();
        }
        /// <summary>
        /// Usado no slider interativo e dynamico, sempre que o player muda o slider
        /// para trás ou para a frente muda sempre o valor e faz overwrite
        /// </summary>
        /// <param name="volume"></param>
        public void SetSfx(float volume)
        {
            _mixer.SetFloat("Fx", Mathf.Log10(volume) * 20);
            _sfxVolumeTextValue.text = volume.ToString("0.0");
            PlayerPrefs.SetFloat("SFXVolume", volume);
            PlayerPrefs.Save();
        }
    }
}