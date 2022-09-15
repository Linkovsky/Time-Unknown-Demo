using DG.Tweening;
using RPG.ScriptableObjects.Duplicate;
using UnityEngine;

namespace RPG.Enemy
{
    public class EnemySemiBoss : MonoBehaviour
    {
        /// <summary>
        /// Declaração de varáveis
        /// </summary>
        [SerializeField] private GameObject _prefabAttack;
        [SerializeField] private GameObject _chest;
        [SerializeField] private PreventDuplicates _preventDoubleChest;
        [SerializeField] private float _attackRadious;
        [SerializeField] private float _pos;
        [field: SerializeField] public float startBtwShots { get; private set; }

        private GameObject thisObject;
        private Transform _player;

        private float timeBtwShots;

        /// <summary>
        /// Atribui os componentes necessários 
        /// Inicia o movimento do gameobject em questão em Y com uma velocidade de 3.5f unidades
        /// Com loop infinito com efeito yoyo com InOuSine que dá um efeito mais smooth entre o valor máximo e minimo
        /// </summary>
        private void Start()
        {
            if (_preventDoubleChest.numberOfTimes == 1)
                _chest.SetActive(true);
            thisObject = gameObject;
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            thisObject.transform.DOMove(new Vector3(transform.position.x, _pos, transform.position.z), 3.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .OnKill(() => thisObject = null)
                .OnKill(() => _chest.SetActive(true));
        }
        /// <summary>
        /// Em cada frame verifica a distância entre o jogador e a área de ataque
        /// </summary>
        private void Update()
        {
            if (Vector2.Distance(transform.position, _player.transform.position) < _attackRadious)
            {
                FireObject();

            }
        }
        /// <summary>
        /// Se o valor for 0 então poderá instanciar um prefab numa linha reta com o intuito de acertar no jogador
        /// com uma rotação em -90 graus para a direita
        /// </summary>
        private void FireObject()
        {
            if (timeBtwShots <= 0)
            {

                GameObject attack = Instantiate(_prefabAttack, transform.position, Quaternion.Euler(0, 0, -90f));
                attack.transform.DOMove(new Vector3(77.42f, transform.position.y, transform.position.z), 0.75f)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() => Destroy(attack))
                    .OnKill(() => attack = null);
                timeBtwShots = startBtwShots;
            }
            else
                timeBtwShots -= Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _attackRadious);
        }
    }
}