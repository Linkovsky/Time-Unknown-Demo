using Cinemachine;
using RPG.SceneManagement;
using RPG.UI.HealthSystem;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SaveSystem
{
    public class SavingFile : MonoBehaviour
    {
        /// <summary>
        /// Declara��o de vari�veis
        /// </summary>
        [SerializeField] private HeartContainerSO _heartContainerSO;
        [SerializeField] private Inventory.Inventory _inventory;
        private PlayerData _playerData;
        private SaveDataLevel _saveDataLevel;

        internal string _path = "";
        /// <summary>
        /// Procura pela diretoria padr�o e adiciona � vari�vel
        /// </summary>
        private void Awake()
        {
            _path = Application.persistentDataPath + "/" + "save.aes";
        }

        /// <summary>
        /// Consoante os dados necess�rios para guardar o progresso do jogador, os dados s�o buscados a partir dos componentes
        /// e adiciona �s vari�veis para depois ser chamado o construtor da class playerdata para ser poss�vel guardar no ficheiro
        /// </summary>
        private void CreatePlayerData()
        {
            string musicName = LevelManager.Instance.GetAudioName();
            if(_saveDataLevel != null)
            {
                _saveDataLevel = GameObject.FindGameObjectWithTag("Save").GetComponent<SaveDataLevel>();
                _saveDataLevel.DataSave();
            }
            Transform position = GameObject.FindGameObjectWithTag("Player").transform;
            string camera = GameObject.FindGameObjectWithTag("vCam").GetComponent<CinemachineConfiner2D>().m_BoundingShape2D.name;
            HealthUI health = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<HealthUI>();
            int currentHealthFragments = health._healthSystem.CurrentFragments();
            _playerData = new PlayerData(new Vector3(position.position.x, position.position.y, position.position.z)
                , SceneManager.GetActiveScene().buildIndex, camera
                , _heartContainerSO.heartNumber
                , musicName
                , currentHealthFragments
                , _inventory);
        }
        /// <summary>
        /// A data do jogador � criada e a mesma � atribu�da num ficheiro criado pelo filestream onde tamb�m � convertido em 
        /// formato bin�rio para uma maior seguran�a
        /// </summary>
        public void SaveData()
        {
            CreatePlayerData();
            string savePath = _path;

            if (File.Exists(savePath)) File.Delete(savePath);

            using FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter binaryFormatter = new();
            binaryFormatter.Serialize(fs, _playerData);
            fs.Close();

        }
        /// <summary>
        /// Os dados s�o lidos a partir do ficheiro e mandados para o Level Manager que � o respons�vel pela atribui��o dos valores
        /// </summary>
        public void LoadData()
        {

            using FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read);

            BinaryFormatter binaryFormatter = new();
            PlayerData data = binaryFormatter.Deserialize(fs) as PlayerData;
            if (data == null)
            {
                SaveData();
            }
            print(data.currentHealth);
            _heartContainerSO.SetHeartContainerNumber(data.heartContainerNumber);
            LevelManager.Instance.StartCoroutine(LevelManager.Instance.TransitionFromFile(data.currentIndex
                , new Vector3(data.position[0], data.position[1], data.position[2])
                , data.gameObjectName
                , data.musicName
                , data.currentHealth));
            for (int i = 0; i < _inventory._inventory.Count; i++)
            {
                if (_inventory._inventory[i].itemData.name == data.nameOfItem[i])
                {
                    _inventory._inventory[i].stackSize = data.numberOfItems[i];
                }
            }
            fs.Close();
        }
        public void DeleteSaveFile()
        {
            if (File.Exists(_path)) File.Delete(_path);
        }
    }
}