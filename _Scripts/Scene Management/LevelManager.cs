using Cinemachine;
using RPG.SaveSystem;
using RPG.SceneManagement.ScriptableObjects;
using RPG.ScriptableObjects.Sounds;
using RPG.UI.HealthSystem;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPG.SceneManagement
{
    public class LevelManager : MonoBehaviour
    {
        /// <summary>
        /// Declaração das variáveis
        /// </summary>
        public static LevelManager Instance;

        [SerializeField] private Animator _gameOverAnimator;
        [SerializeField] private GameObject _loaderCanvas;
        [SerializeField] private Image _progressBar;
        [SerializeField] private Image _background;
        [SerializeField] private Sprite _backgroundGameOver;
        [SerializeField] private Material _screenTransitionMaterial;
        [SerializeField] private LevelPositionsSO _playerPositions;
        [SerializeField] private BackgroundSounds _ambientMusic;
        [SerializeField] private Inventory.Inventory _playerInventory;
        [SerializeField] private GameObject _playerObject;
        [SerializeField] private SceneNameBorder _sceneName;
        [SerializeField] internal SaveDataLevel _saveDataLevel;
        [SerializeField] private GameObject _health;
        [SerializeField] private HealthUI _healthUI;
        public AudioSource _audioSource;

        public bool pauseMusic;
        public bool isLoading;
        public bool endCredits;
        public string currentScene;

        private Vector3 _positionFromFile;

        private string _cameraName;
        private string _musicName;
        private float _transitionTime = 1f;
        private float _startValue = 1f;
        private float _endValue = 0f;
        private float _target;
        private int saveFileCurrentHealth;
        private int _previousScene;
        private bool _loadFromFile;

        /// <summary>
        /// Cria uma instância desta classe para que apenas possa existir 1 neste caso um singleton
        /// caso existe mais que uma instância destrói o objeto
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            _audioSource = GetComponent<AudioSource>();
        }
        /// <summary>
        /// Chama a função de tocar música de fundo
        /// </summary>
        private void Start()
        {

            SceneManager.sceneUnloaded += AfterSceneIsUnloaded;
            SceneManager.sceneLoaded += ExtraStuffBeforeShowingScene;
            PlayBackgroundMusic();
        }
        /// <summary>
        /// Botão que diz respeito ao game over quando o jogador carregar no restart é feito o reload do nível em que se encontra
        /// e é healado de novo
        /// </summary>
        public void ButtonRestart()
        {
            StartCoroutine(FadeOutAndReload());
        }

        private IEnumerator FadeOutAndReload()
        {
            _gameOverAnimator.SetTrigger("Out");
            yield return new WaitForSeconds(1f);
            StartCoroutine(TransitionCoroutine(SceneManager.GetActiveScene().name, _backgroundGameOver));
        }

        /// <summary>
        /// Botão para voltar ao menu principal
        /// </summary>
        public void ButtonQuit()
        {
            StartCoroutine(FadeAndStartMenu());
        }

        private IEnumerator FadeAndStartMenu()
        {
            _gameOverAnimator.SetTrigger("Out");
            yield return new WaitForSeconds(1f);
            StartCoroutine(TransitionCoroutine("StartMenu", _backgroundGameOver));
        }

        /// <returns>O nome da música que está a tocar de momento</returns>
        public string GetAudioName()
        {
            return _audioSource.clip.name;
        }
        /// <summary>
        /// Usado um for para percorrer todos os index do SO onde estão guardados os dados das músicas
        /// E caso a scene atual corresponda ao index é atribuido o clip respetivo desse index
        /// </summary>
        /// <returns>O nome do clip</returns>
        public string GetAudioNameFromSO()
        {
            for (int i = 0; i < _ambientMusic.backgroundSounds.Length; i++)
            {
                if (SceneManager.GetActiveScene().buildIndex == i)
                {
                    _audioSource.clip = _ambientMusic.backgroundSounds[i];
                    break;
                }
            }
            return _audioSource.clip.name;
        }
        /// <summary>
        /// Usando um for para percorrer todos os indexs do SO das músicas e toca a respetiva música
        /// </summary>
        public void PlayBackgroundMusic()
        {
            for (int i = 0; i < _ambientMusic.backgroundSounds.Length; i++)
            {
                if (SceneManager.GetActiveScene().buildIndex == i)
                {
                    _audioSource.clip = _ambientMusic.backgroundSounds[i];
                    _audioSource.Play();
                    break;
                }
            }
        }
        /// <summary>
        /// O mesmo que a função acima excepto que neste caso é executado a partir de um ficheiro que continha
        /// a informação da música que estava a tocar na altura em que o jogador fez pausa e gravou o estado do jogo
        /// Depois é verificado se o jogador estava num nível normal ou numa dungeon e toca a respetiva música
        /// </summary>
        private void PlayBackgroundMusicFromFile()
        {
            for (int i = 0; i < _ambientMusic.backgroundSounds.Length; i++)
            {
                if (_ambientMusic.backgroundSounds[i].name == _musicName)
                {
                    _audioSource.clip = _ambientMusic.backgroundSounds[i];
                    _audioSource.Play();
                    break;
                }
                else if (i < _ambientMusic.dungeonSounds.Length)
                {
                    if (_ambientMusic.dungeonSounds[i].name == _musicName)
                    {
                        _audioSource.clip = _ambientMusic.dungeonSounds[i];
                        _audioSource.Play();
                        break;
                    }
                }
            }
            _musicName = string.Empty;
        }
        /// <summary>
        /// Percorre os indexes todos do dungeon e toca a que recebe como parâmetro na função
        /// </summary>
        /// <param name="dungeon"></param>
        public void PlayDungeonMusic(int dungeon)
        {
            for (int i = 0; i < _ambientMusic.dungeonSounds.Length; i++)
            {
                if (i == dungeon)
                {
                    _audioSource.clip = _ambientMusic.dungeonSounds[i];
                    _audioSource.Play();
                    break;
                }
            }
        }
        /// <summary>
        /// Verifica qual a scene em que o jogador se encontra e se o inventário estiver igual a 0 significa que é um new game
        /// Caso contrário testa sempre para verificar se o jogador vem de uma scene para uma outra onde contém várias saídas para outras
        /// scenes para o jogador ser colocado no sítio correto
        /// Caso nenhuma delas é verdadeira está a entrar no mundo pelos valores pré definidos de início
        /// </summary>
        private void PlacePlayer()
        {

            if (SceneManager.GetActiveScene().buildIndex == 2 && _playerInventory._inventory.Count == 0)
            {
                _health.SetActive(true);
                FindObjectOfType<PlayerController>().transform.position = _playerPositions.StartPosition;
                CinemachineVirtualCamera _vCam = GameObject.FindGameObjectWithTag("vCam").GetComponent<CinemachineVirtualCamera>();
                _vCam.Follow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                _vCam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = GameObject.Find("StartingBoundary").GetComponent<PolygonCollider2D>();
            }
            else
            {
                for (int i = 0; i < _playerPositions.Positions.Length; i++)
                {

                    if (_previousScene == 4 && SceneManager.GetActiveScene().name == "Razor Forest")
                    {
                        _health.SetActive(true);
                        FindObjectOfType<PlayerController>().transform.position = _playerPositions.RazorForestPos[0];
                        CinemachineVirtualCamera _vCam = GameObject.FindGameObjectWithTag("vCam").GetComponent<CinemachineVirtualCamera>();
                        _vCam.Follow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                        return;
                    }
                    if (_previousScene == 5 && SceneManager.GetActiveScene().name == "Razor Forest")
                    {
                        _health.SetActive(true);
                        FindObjectOfType<PlayerController>().transform.position = _playerPositions.RazorForestPos[1];
                        CinemachineVirtualCamera _vCam = GameObject.FindGameObjectWithTag("vCam").GetComponent<CinemachineVirtualCamera>();
                        _vCam.Follow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                        return;
                    }
                    if (_previousScene == 6 && SceneManager.GetActiveScene().name == "Sparkling Village")
                    {
                        _health.SetActive(true);
                        FindObjectOfType<PlayerController>().transform.position = _playerPositions.SparklingVillage[0];
                        CinemachineVirtualCamera _vCam = GameObject.FindGameObjectWithTag("vCam").GetComponent<CinemachineVirtualCamera>();
                        _vCam.Follow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                        return;
                    }
                    if (_previousScene == 7 && SceneManager.GetActiveScene().name == "Forest Of Mountains")
                    {
                        _health.SetActive(true);
                        FindObjectOfType<PlayerController>().transform.position = _playerPositions.ForestOfMountains;
                        CinemachineVirtualCamera _vCam = GameObject.FindGameObjectWithTag("vCam").GetComponent<CinemachineVirtualCamera>();
                        _vCam.Follow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                        return;
                    }
                    if (_previousScene == 8 && SceneManager.GetActiveScene().name == "Loop Cave")
                    {
                        _health.SetActive(true);
                        FindObjectOfType<PlayerController>().transform.position = _playerPositions.LoopCave;
                        CinemachineVirtualCamera _vCam = GameObject.FindGameObjectWithTag("vCam").GetComponent<CinemachineVirtualCamera>();
                        _vCam.Follow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                        return;
                    }
                    if (_previousScene == 9 && SceneManager.GetActiveScene().name == "Blizz Village")
                    {
                        _health.SetActive(true);
                        FindObjectOfType<PlayerController>().transform.position = _playerPositions.BlizzVillage;
                        CinemachineVirtualCamera _vCam = GameObject.FindGameObjectWithTag("vCam").GetComponent<CinemachineVirtualCamera>();
                        _vCam.Follow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                        return;
                    }
                    if (SceneManager.GetActiveScene().buildIndex == i)
                    {
                        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1) return;
                        _health.SetActive(true);
                        FindObjectOfType<PlayerController>().transform.position = _playerPositions.Positions[i];
                        CinemachineVirtualCamera _vCam = GameObject.FindGameObjectWithTag("vCam").GetComponent<CinemachineVirtualCamera>();
                        _vCam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = GameObject.Find("Camera Boundary").GetComponent<PolygonCollider2D>();
                        _vCam.Follow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                    }
                }
            }
        }
        /// <summary>
        /// Caso o jogador esteja a fazer load game vem parar nesta função que ativa o sistema de vida e coloca o jogador na posição referida
        /// no ficheiro bem como o sistema da câmera e os colliders.
        /// </summary>
        private void PlacePlayerFromFile()
        {
            _health.SetActive(true);
            FindObjectOfType<PlayerController>().transform.position = _positionFromFile;
            PolygonCollider2D polygonCollider = GameObject.Find(_cameraName).GetComponent<PolygonCollider2D>();
            CinemachineVirtualCamera _vCam = GameObject.FindGameObjectWithTag("vCam").GetComponent<CinemachineVirtualCamera>();
            _vCam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = polygonCollider;
            _vCam.Follow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            
            _healthUI._healthSystem.saveFileFragments = saveFileCurrentHealth;
            _healthUI._healthSystem.LoadingHealthFromSaveFile();
            GC.Collect();
        }
        /// <summary>
        /// Função corrida quando o jogador faz load game
        /// Função asyncrona que faz load em paralelo ao estado atual do jogo o que nos permite utilizar várias ferramentas
        /// para apresentar certas animações ao jogador como por exemplo o loading screen
        /// depois da scene estar carregada é feito a subscrição ao evento de quando a scene estiver loaded que funciona antes
        /// de mostrar realmente a scene ao jogador que permite a mudança de posições entre outras coisas
        /// </summary>
        /// <param name="buildIndex">Respetiva Scene em que o player se encontra</param>
        /// <param name="dir">Coordenadas em que o jogador estava quando fez save</param>
        /// <param name="name">Nome da câmera collider</param>
        /// <param name="musicName">Nome da música que estav a tocar</param>
        /// <param name="currentHealth">Vida atual quando o jogador fez save file</param>
        public async void LoadSceneFromFile(int buildIndex, Vector3 dir, string name, string musicName, int currentHealth)
        {
            _health.SetActive(false);
            _positionFromFile = dir;
            _musicName = musicName;
            _target = 0;
            _progressBar.fillAmount = 0;
            _cameraName = name;
            saveFileCurrentHealth = currentHealth;

            _background.color = Color.black;

            var scene = SceneManager.LoadSceneAsync(buildIndex);
            scene.allowSceneActivation = false;

            _loaderCanvas.SetActive(true);

            do
            {
                await Task.Delay(1000);
                _target = scene.progress;
            } while (scene.progress < 0.9f);

            await Task.Delay(2000);
            scene.allowSceneActivation = true;

            _loadFromFile = true;

            StartCoroutine(TransitioningOver());

            _loaderCanvas.SetActive(false);

        }
        /// <summary>
        /// Função corrida quando o jogador faz load game
        /// Função asyncrona que faz load em paralelo ao estado atual do jogo o que nos permite utilizar várias ferramentas
        /// para apresentar certas animações ao jogador como por exemplo o loading screen
        /// depois da scene estar carregada é feito a subscrição ao evento de quando a scene estiver loaded que funciona antes
        /// de mostrar realmente a scene ao jogador que permite a mudança de posições entre outras coisas 
        /// Depois é feita uma testagem para saber se o jogador está a ir para o menu principal ou entrou no prelude do jogo
        /// para que o background image color do loading screen seja preto e não alguma aldeia.
        /// E se o jogador esteve em gameover então torna o booleano a falso para o jogador ter o controlo total da personagem
        /// </summary>
        /// <param name="sceneName">Nome da scene atual do jogador</param>
        /// <param name="background">Imagem da scene para onde o jogador vai</param>
        public async void LoadScene(string sceneName, Sprite background)
        {

            if (_saveDataLevel != null)
                SaveCorrectDataLevelValues();

            _previousScene = SceneManager.GetActiveScene().buildIndex;

            if (PlayerController.playerDead || sceneName == "Prelude" || _previousScene == 1)
                _background.color = Color.black;
            else
                _background.sprite = background;
            _target = 0;

            _progressBar.fillAmount = 0;

            var scene = SceneManager.LoadSceneAsync(sceneName);
            scene.allowSceneActivation = false;

            _loaderCanvas.SetActive(true);

            do
            {
                await Task.Delay(1000);
                _target = scene.progress;
            } while (scene.progress < 0.9f);

            await Task.Delay(2000);
            scene.allowSceneActivation = true;

            _loaderCanvas.SetActive(false);

            StartCoroutine(TransitioningOver());

        }

        private void AfterSceneIsUnloaded(Scene arg0)
        {
            var objects = GameObject.FindObjectsOfType<GameObject>();
            foreach (var gameobject in objects)
            {
                Destroy(gameobject);
            }
        }
        /// <summary>
        /// Depois da scene estar carregada é possível mover as personagens ou correr outras funções sobre qualquer coisa que queiramos
        /// neste caso pretendo verificar se está a ser feito load a partir dum ficheiro ou de maneira normal
        /// e mover o jogador para a posição pretendida e tocar a música
        /// </summary>
        private void ExtraStuffBeforeShowingScene(Scene arg0, LoadSceneMode arg1)
        {
            if (!_loadFromFile)
            {
                currentScene = SceneManager.GetActiveScene().name;
                PlayBackgroundMusic();
                PlacePlayer();
                if (PlayerController.playerDead)
                {
                    _healthUI._healthSystem.Heal(80);
                    PlayerController.playerDead = false;
                }
            }
            else
            {

                PlayBackgroundMusicFromFile();
                PlacePlayerFromFile();
            }
        }

        /// <summary>
        /// Quando o jopgador derrota o boss é de imediato corrida esta função para o jogador ver os créditos finais do jogo
        /// </summary>
        /// <param name="sceneName">Nome da scene para load</param>
        public async void LoadCreditScene(string sceneName)
        {
            _health.SetActive(false);
            var scene = SceneManager.LoadSceneAsync(sceneName);
            scene.allowSceneActivation = false;
            do
            {
                await Task.Delay(1000);
            } while (scene.progress < 0.9f);

            await Task.Delay(500);
            scene.allowSceneActivation = true;
        }
        /// <summary>
        /// Chama a função acima
        /// </summary>
        public void LoadCredits()
        {
            LoadCreditScene("CreditScene");
        }
        /// <summary>
        /// Chama a função de gravar os dados da scene em que o jogador estava
        /// </summary>
        private void SaveCorrectDataLevelValues()
        {
            _saveDataLevel.DataSave();
        }
        /// <summary>
        /// Fillamount representado pela loading bar
        /// </summary>
        private void Update()
        {
            _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _target, 3 * Time.deltaTime);
        }

        public void NullingSaveDataAndAddingTheCorrectOne(SaveDataLevel saveDataLevel)
        {
            _saveDataLevel = null;
            _saveDataLevel = saveDataLevel;
        }
        /// <summary>
        /// Usando o material do shader de transição usado quando o jogador entra nas casas das outras personagens
        /// </summary>
        /// <param name="name">Nome da scene para onde o jogador vai</param>
        /// <param name="background">background com fundo preto</param>
        public IEnumerator TransitionCoroutine(string name, Sprite background)
        {
            isLoading = true;
            float currentTime = 0f;

            while (currentTime < _transitionTime)
            {
                currentTime += Time.deltaTime * 2;
                float totalTime = currentTime / _transitionTime;
                float value = Mathf.Lerp(_startValue, _endValue, totalTime);
                _screenTransitionMaterial.SetFloat("_Progress", value);
                yield return null;

            }
            _health.SetActive(false);
            if (endCredits)
            {
                endCredits = false;  
                LoadCredits();
            }
            else
                LoadScene(name, background);
        }
        /// <summary>
        /// O mesmo que a coroutina de cima adicionando que como é feito o load pelo ficheiro contém as informações contidas no ficheiro
        /// de save
        /// </summary>
        /// <param name="name">Nome da scene para onde o jogador vai</param>
        /// <param name="background">background da aldeia onde vai o jogador</param>
        public IEnumerator TransitionFromFile(int buildIndex, Vector3 dir, string name, string musicName, int currentHealth)
        {
            isLoading = true;
            _background.color = Color.black;
            float currentTime = 0f;

            while (currentTime < _transitionTime)
            {
                currentTime += Time.deltaTime * 2;
                float totalTime = currentTime / _transitionTime;
                float value = Mathf.Lerp(_startValue, _endValue, totalTime);
                _screenTransitionMaterial.SetFloat("_Progress", value);
                yield return null;

            }
            _health.SetActive(false);

            LoadSceneFromFile(buildIndex, dir, name, musicName, currentHealth);
        }
        /// <summary>
        /// Transição so material do shader em sentido inverso
        /// </summary>
        /// <returns></returns>
        private IEnumerator TransitioningOver()
        {
            float currentTime = 0f;

            while (currentTime < _transitionTime)
            {
                currentTime += Time.deltaTime * 2;
                float totalTime = currentTime / _transitionTime;
                float value = Mathf.Lerp(_endValue, _startValue, totalTime);
                _screenTransitionMaterial.SetFloat("_Progress", value);
                yield return null;
            }
            _background.color = Color.white;
            _sceneName.gameObject.SetActive(true);
            _sceneName.StartCoroutine(_sceneName.Board());
            _loadFromFile = false;
            isLoading = false;
            yield return null;
        }
    }
}