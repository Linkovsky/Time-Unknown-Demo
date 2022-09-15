using Cinemachine;
using RPG.SceneManagement;
using System.Collections;
using UnityEngine;

namespace RPG.Entities
{
    public class Teleportation : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private Transform _posToGo;
        [SerializeField] private CinemachineConfiner2D _cameraCollider;
        [SerializeField] private PolygonCollider2D _cameraBoundary;
        [SerializeField] private Material _screenTransitionMaterial;
        [SerializeField] private float _transitionTime = 1f;
        [SerializeField] private bool _isDungeon = false;
        [SerializeField] private int _dungeonClipNumber;

        private float _startValue = 1f;
        private float _endValue = 0f;
        private const string _propertyName = "_Progress";

        /// <summary>
        /// Define o valor float do material a 1 para que quando iniciamos o jogo não esteja com o material à frente do player
        /// </summary>
        private void Start()
        {
            _screenTransitionMaterial.SetFloat(_propertyName, 1);
        }

        /// <summary>
        /// Coroutina, para o jogador poder transacionar de uma posição para outra. Dentro do while enquanto não estiver o tempo 
        /// de transição o set float do material para depois ser mudado de posição + trocar de collider para a cinamachine
        /// onde é preciso utilizar o InvalidateCache primeiro para limpar a cache das boundaries anteriores.
        /// </summary>
        /// <param name="_player">Colllider do player</param>
        /// <param name="_playerController">Componente do jogador PlayerController</param>
        /// <returns></returns>
        private IEnumerator TransitionCoroutine(Collider2D _player, PlayerController _playerController)
        {
            _playerController.stopPlayer = true;
            float currentTime = 0f;

            while (currentTime < _transitionTime)
            {
                currentTime += Time.deltaTime * 2;
                float totalTime = currentTime / _transitionTime;
                float value = Mathf.Lerp(_startValue, _endValue, totalTime);
                _screenTransitionMaterial.SetFloat(_propertyName, value);
                yield return null;
            }
            _player.transform.position = _posToGo.position;

            _cameraCollider.InvalidateCache();
            _cameraCollider.m_BoundingShape2D = _cameraBoundary;

            yield return new WaitForSecondsRealtime(1f);

            StartCoroutine(TransitioningOver(_playerController));
        }

        /// <summary>
        /// Faz o mesmo que a coroutina acima mas no sentido inverso e não move o jogador, mas verifica se 
        /// o booleano que é acionado no Editor for verdadeiro ou não para poder mudar a música do jogo.
        /// </summary>
        /// <param name="_playerController">Componente do jogador</param>
        /// <returns>Nothing</returns>
        private IEnumerator TransitioningOver(PlayerController _playerController)
        {
            float currentTime = 0f;
            if (_isDungeon)
            {
                yield return new WaitForSeconds(0.05f);
                LevelManager.Instance.PlayDungeonMusic(_dungeonClipNumber);
            }
            else if (LevelManager.Instance.GetAudioName() != LevelManager.Instance.GetAudioNameFromSO())
            {
                LevelManager.Instance.PlayBackgroundMusic();
            }

            while (currentTime < _transitionTime)
            {
                currentTime += Time.deltaTime * 2;
                float totalTime = currentTime / _transitionTime;
                float value = Mathf.Lerp(_endValue, _startValue, totalTime);
                _screenTransitionMaterial.SetFloat(_propertyName, value);
                yield return null;
            }

            _playerController.stopPlayer = false;
            yield return null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                StartCoroutine(TransitionCoroutine(collision, collision.gameObject.GetComponent<PlayerController>()));
            }
        }
    }
}