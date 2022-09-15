using RPG.Entities;
using RPG.SceneManagement;
using RPG.ScriptableObjects.Duplicate;
using UnityEngine;

namespace RPG.Entities.Chest
{
    public class Switch : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private PushBlocks _blocks;
        [SerializeField] private GameObject _object;
        [SerializeField] private AudioClip _audio;
        [SerializeField] private PreventDuplicates _preventThings;
        [SerializeField] private bool _chest = false;
        [SerializeField] private bool _ResetPosition = false;

        private AudioSource _audioSource;
        private Animator _animator;
        private BoxCollider2D _collider;

        /// <summary>
        /// Variáveis recebem o seu respetivo componente
        /// </summary>
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audioSource = GameObject.FindGameObjectWithTag("UI").GetComponent<AudioSource>();
            _collider = GetComponent<BoxCollider2D>();
        }

        /// <summary>
        /// Se já foi utilizado, então muda de animação
        /// </summary>
        private void Start()
        {

            if (_preventThings != null)
                if (_preventThings.numberOfTimes != 0)
                {
                    _animator.Play("Switch_Pressed");
                    _object.SetActive(true);
                    _collider.enabled = false;
                }
        }

        /// <summary>
        /// Testa se o booleano acionado no Editor se refere a um chest ou apenas para resetar posições
        /// </summary>
        /// <param name="collision">Representa o gameobject que entra em colisão</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_ResetPosition)
            {
                _animator.Play("Switch_Pressed");
                _blocks.ResetPositions();
            }
            else if (_chest)
            {
                _animator.Play("Switch_Pressed");
                LevelManager.Instance.pauseMusic = true;
                _object.SetActive(true);
                _audioSource.PlayOneShot(_audio);
                LevelManager.Instance.pauseMusic = false;
            }
        }
    }
}