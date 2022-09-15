using RPG.SceneManagement;
using RPG.ScriptableObjects.Duplicate;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SaveSystem
{
    public class SaveDataLevel : MonoBehaviour
    {
        [SerializeField] PreventDuplicates[] _preventDuplicates;

        private void Awake()
        {
            LoadSave();
            GameObject.FindGameObjectWithTag("UI").GetComponent<LevelManager>().NullingSaveDataAndAddingTheCorrectOne(this);
        }

        /// <summary>
        /// Quando inicia um jogo novo este método é corrido para resetar todas as entradas no regedit e apagar os dados
        /// </summary>
        public void ResetAll()
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    PlayerPrefs.DeleteKey("Data_" + j + "_" + i);
                }
                PlayerPrefs.DeleteKey("Data_" + i + "_" + i);
            }
        }
        /// <summary>
        /// Cada vez que o jogador move de cena consoante os dados que se pretende guardar é corrido num for pelo array
        /// e guarda atravez do buildIndex e o valor do SO
        /// </summary>
        public void DataSave()
        {
            for (int i = 0; i < _preventDuplicates.Length; i++)
            {
                if (PlayerPrefs.GetInt("Data_" + i + "_" + SceneManager.GetActiveScene().buildIndex) == 0)
                    PlayerPrefs.SetInt("Data_" + i + "_" + SceneManager.GetActiveScene().buildIndex, _preventDuplicates[i].numberOfTimes);
            }

        }
        /// <summary>
        /// Quando volta a entrar na cena o for é iniciado para ir buscar os dados do PlayerPrefs
        /// </summary>

        public void LoadSave()
        {

            for (int i = 0; i < _preventDuplicates.Length; i++)
            {
                _preventDuplicates[i].numberOfTimes = PlayerPrefs.GetInt("Data_" + i + "_" + SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}