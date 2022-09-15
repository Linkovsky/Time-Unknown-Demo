using UnityEngine;

namespace RPG.Enemy
{
    public class EnemyFire : MonoBehaviour
    {
        /// <summary>
        /// Declara��o de vari�veis
        /// </summary>
        [SerializeField] private float _fireRadius;
        [SerializeField] private GameObject _firePrefab;
        [SerializeField] private float _startBtwShots;

        private Transform _playerTransform;
        private EnemyAI _enemyAI;
        private float _timeBtwShots;
        /// <summary>
        /// Atrbu� o componente em uso para a respetiva vari�vel
        /// </summary>
        private void Start()
        {
            _enemyAI = GetComponent<EnemyAI>();
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        /// <summary>
        /// A cada frame verificar se o jogador est� em game over state para n�o dar qualquer erro
        /// E verificar a dist�ncia entre o objeto e o jogador
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
        /// Verifica se o tempo restante � igual a 0
        /// Se sim ent�o � declarada uma variavel Vector3 onde ir� receber a posi��o naquele momento que o m�todo � chamado
        /// o valor da coordenada Z � colocada a 0 e � feita outra declara��o onde recebe a posi��o do inimigo. Usando Mathf.Atan2
        /// para receber o valor do �ngulo em radianos sob a tangente x e y para calcular a rota��o que a fireball precisa de ter
        /// para ir com a anima��o no angulo certo para a posi��o do jogador
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

