using UnityEngine;

namespace RPG.Enemy
{
    public class EnemyFire : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private float _fireRadius;
        [SerializeField] private GameObject _firePrefab;
        [SerializeField] private float _startBtwShots;

        private Transform _playerTransform;
        private EnemyAI _enemyAI;
        private float _timeBtwShots;
        /// <summary>
        /// Atrbuí o componente em uso para a respetiva variável
        /// </summary>
        private void Start()
        {
            _enemyAI = GetComponent<EnemyAI>();
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        /// <summary>
        /// A cada frame verificar se o jogador está em game over state para não dar qualquer erro
        /// E verificar a distância entre o objeto e o jogador
        /// </summary>
        private void Update()
        {
            if (PlayerController.playerDead) return;
            if (_enemyAI._playerCollide || _enemyAI._isDead) return;
                if (Vector2.Distance(this.transform.position, _playerTransform.position) < _fireRadius)
                {
                    FireObject();
                }
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
            Gizmos.DrawWireSphere(this.transform.position, _fireRadius);
        }
    }
}

