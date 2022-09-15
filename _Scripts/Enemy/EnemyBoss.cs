using DG.Tweening;
using UnityEngine;
using System.Collections;
using RPG.SceneManagement;

namespace RPG.Enemy
{
    public class EnemyBoss : MonoBehaviour
    {
        /// <summary>
        /// Declaração das variáveis
        /// </summary>
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private GameObject _firePrefab;
        [SerializeField] private float _startBtwShots;
        [SerializeField] private float _fireRadius;

        private Transform _playerTransform;
        private EnemyHealth _bossHealth;
        private DOTweenPath _bossPath;

        private float _timeBtwShots;
        private bool _secondStage;

        /// <summary>
        /// Atribuindo o componente respetivo a cada uma das variáveis
        /// </summary>
        private void Awake()
        {
            _bossHealth = GetComponentInChildren<EnemyHealth>();
            _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            _bossPath = GetComponent<DOTweenPath>();
        }
        /// <summary>
        /// A cada frame verifica se a vida atual é igual a 0
        /// Se a vida atual é maior que 50 para se movimentar + instanciar bolas de fogo
        /// Se a vida atual é menor que 49 para iniciar a segunda fase da batalha onde anda mais rápido
        /// e lança projeteis mais rápido também
        /// </summary>
        private void Update()
        {
            if(_bossHealth._currentHealth > 50)
            {
                if (Vector2.Distance(this.transform.position, _playerTransform.position) < _fireRadius)
                {
                    FireObject();
                    return;
                }
            }
            
            if (_bossHealth._currentHealth < 49)
            {
                if(_secondStage == false)
                {
                    _secondStage = true;
                    _bossPath.duration /= 3;
                    _startBtwShots /= 2;
                }
                if (Vector2.Distance(this.transform.position, _playerTransform.position) < _fireRadius)
                {
                    FireObject();
                }
            }

            
        }
        public void StopMovement()
        {
            _bossPath.DOPause();
        }
        /// <summary>
        /// Coroutina onde o level manager recebe um audio clip e toca uma música
        /// o Tween do inimigo é colocado em pausa para o mesmo parar de se movimentar, espera-se pelo tempo do audio
        /// e mata o Tween e a coroutina volta a valor null
        /// </summary>
        /// <returns></returns>
        public IEnumerator BossOver()
        {
            LevelManager.Instance._audioSource.PlayOneShot(_audioClip);
            _bossPath.DOPause();
            yield return new WaitForSeconds(_audioClip.length);
            LevelManager.Instance.PlayDungeonMusic(0);
            _bossPath.DOKill(true);
        }
        /// <summary>
        /// Verifica se o tempo restante é igual a 0
        /// Se sim então é declarada uma variavel Vector3 onde irá receber a posição naquele momento que o método é chamado
        /// o valor da coordenada Z é colocada a 0 e é feita outra declaração onde recebe a posição do inimigo. Usando Mathf.Atan2
        /// para receber o valor do ângulo em radianos sob a tangente x e y para calcular a rotação que a fireball precisa de ter
        /// para ir com a animação no angulo certo para a posição do jogador
        /// </summary>
        private void FireObject()
        {
            if (_timeBtwShots <= 0)
            {
                Vector3 targ = _playerTransform.transform.position;
                targ.z = 0f;

                Vector3 objectPos = transform.position;
                targ.x = targ.x - objectPos.x;
                targ.y = targ.y - objectPos.y;

                float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
                GameObject prefab = Instantiate(_firePrefab, transform.position, Quaternion.identity);
                prefab.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + (-84.756527f)));
                _timeBtwShots = _startBtwShots;
            }
            else
                _timeBtwShots -= Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _fireRadius);
        }
    }
    
}
