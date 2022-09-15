using RPG.SceneManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.HealthSystem
{
    public class HealthUI : MonoBehaviour
    {
        /// <summary>
        /// Declara��o de vari�veis
        /// </summary>
        public static HealthSystem healthSystem;

        [SerializeField] private Animator _animatorGameOver;
        [SerializeField] private AudioClip _gameOverClip;
        [SerializeField] private Transform _parentPrefab;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Sprite[] _heartSprite;
        [SerializeField] private HeartContainerSO _numberOfHearts;

        public HealthSystem _healthSystem;

        private PlayerController _playerController;
        private List<GameObject> _prefabImage;

        private int _fragImg;
        /// <summary>
        /// As listas s�o iniciadas
        /// Chamada da fun��o que coloca os cora��es vis�veis no ecr� e � feita a subscri��o aos eventos para quando as fun��es
        /// s�o chamadas os eventos possam ser acionados
        /// </summary>
        private void Awake()
        {
            _prefabImage = new();
            _healthSystem = new HealthSystem(_numberOfHearts.heartNumber);
            SetAllHearts();

            healthSystem = _healthSystem;
            _healthSystem.OnDamaged += TookDamage;
            _healthSystem.OnSaveFileReloaded += RefreshCurrentHealth;
            _healthSystem.OnHealed += Healed;
            _healthSystem.OnDead += IsDead;
        }

        private void RefreshCurrentHealth(object sender, EventArgs e)
        {
            for (int i = 0; i < _healthSystem.heartList.Count; i++)
            {
                GetFragments(_healthSystem.heartList[i].Fragments);
                _prefabImage[i].GetComponent<Image>().sprite = _heartSprite[_fragImg];
                print(_fragImg);
            }
        }

        /// <summary>
        /// Usando um for para percorrer os items da lista depois s�o adicionados a uma outra lista que � 
        /// depois instanciada o gameobject para ser vis�vel
        /// </summary>
        private void SetAllHearts()
        {
            for (int i = 0; i < _healthSystem.heartList.Count; i++)
            {
                GameObject heart = Instantiate(_prefab, _parentPrefab);
                _prefabImage.Add(heart);
                GetFragments(_healthSystem.heartList[i].Fragments);
                heart.GetComponent<Image>().sprite = _heartSprite[_fragImg];
            }
        }
        /// <summary>
        /// Cada vez que o jogador leva dano tem que ser feito um refresh dos cora��es para mostrar quanto de vida restante
        /// resta ao jogador. Para isso utilizamos um novo for que l� a lista e que vai buscar o n�mero de fragmentos restantes em cada imagem
        /// Caso seja adicionar um cora��o entra dentro do if para poder verificar se o tamanho da lista � diferente e se for ent�o
        /// tem que se adicionar mais um gameobject � lista bem como colocado a sua imagem
        /// </summary>
        private void RefreshHearts()
        {
            for (int i = 0; i < _healthSystem.heartList.Count; i++)
            {
                if (_healthSystem.heartList.Count != _prefabImage.Count)
                {
                    GameObject heart = Instantiate(_prefab, _parentPrefab);
                    _prefabImage.Add(heart);
                    _healthSystem.HealAfterNewHeartContainer();
                }
                GetFragments(_healthSystem.heartList[i].Fragments);
                if (_prefabImage != null)
                    _prefabImage[i].GetComponent<Image>().sprite = _heartSprite[_fragImg];
            }
        }
        /// <summary>
        /// Utilizada para poder adicionar mais 1 cora��o � lista
        /// </summary>
        public void IncreaseTotalHearts()
        {
            _numberOfHearts.IncreaseHeartContainer();
            _healthSystem.AddHeartToList();
            RefreshHearts();
        }
        /// <summary>
        /// Quando o evento for acionado
        /// Quando o jogador fica com 0 fragmentos entra no modo game over ent�o para isso temos de verificar se esse modo j� est� ativo
        /// e depois � feito uma nova verifica��o de quantos cora��es tem para mostrar 0 cora��es que falta
        /// � usado uma anima��o ao jogador de morrer e um som, bem como a anima��o de game over.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsDead(object sender, EventArgs e)
        {
            if (PlayerController.playerDead) return;
            RefreshHearts();
            PlayerController.playerDead = true;
            _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            _playerController.GetComponent<Animator>().Play("Death");
            _playerController.GetComponent<Rigidbody2D>().mass = 100000;
            _animatorGameOver.SetTrigger("In");
            LevelManager.Instance._audioSource.clip = _gameOverClip;
            LevelManager.Instance._audioSource.PlayOneShot(_gameOverClip);
        }
        /// <summary>
        /// Quando o evento for acionado de ganhar vida est� fun��o � corrida que faz um refresh para ver quantos cora��es ainda resta
        /// </summary>
        private void Healed(object sender, EventArgs e)
        {
            RefreshHearts();
        }
        /// <summary>
        /// Quando o evento for acionado de perder vida est� fun��o � corrida que faz um refresh para ver quantos cora��es ainda resta
        /// </summary>

        private void TookDamage(object sender, EventArgs e)
        {
            RefreshHearts();
        }

        public void GetFragments(int fragAmount)
        {
            switch (fragAmount)
            {
                case 0: _fragImg = 0; break;
                case 1: _fragImg = 1; break;
                case 2: _fragImg = 2; break;
                case 3: _fragImg = 3; break;
                case 4: _fragImg = 4; break;
            }
        }
    }
}
