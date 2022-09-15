
using DG.Tweening;
using RPG.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private GameObject _chest;
        [SerializeField] private AudioSource _source;
        [SerializeField] private Animator _bossAnimator;
        [SerializeField] private Animator _flashFX;
        [SerializeField] private SpriteRenderer _sr;
        [SerializeField] private EnemyAI _enemyAI;
        [SerializeField] private GameObject _heartForRegainingHealth;
        [SerializeField] private AudioClip hit;
        [SerializeField] private Sprite _backgroundSprite;
        [SerializeField] private float _fillAmountSpeed;

        [SerializeField] private float _maxHealth;
        public float _currentHealth { get; private set; }

        private Image _healthBar;
        private WaitForSeconds _sleep01;
        private WaitForSeconds _sleep1;
        private WaitForSeconds _sleep05;
        /// <summary>
        /// Atribuição dos componentes às variáveis
        /// </summary>
        private void Awake()
        {
            _sleep01 = new WaitForSeconds(0.1f);
            _sleep1 = new WaitForSeconds(1f);
            _sleep05 = new WaitForSeconds(0.5f);
            _healthBar = GetComponent<Image>();

            if (transform.root.name == "Boss")
            {
                _bossAnimator = GetComponentInParent<Animator>();
                _flashFX = GameObject.Find("DeadAnimation").GetComponent<Animator>();

            }
        }
        private void Start()
        {
            _currentHealth = _maxHealth;
        }

        /// <summary>
        /// A cada frame
        /// </summary>
        private void Update()
        {
            HealthBarFillAmount();
        }
        /// <summary>
        /// Toca um audioclip, faz a subtração da vida atual com o value e verifica se chegou a 0
        /// </summary>
        /// <param name="value">Valor vindo de outras classes onde os inimigos efetuam o dano (value) para o jogador perder vida</param>
        public void Damage(float value)
        {
            _source.PlayOneShot(hit);
            _currentHealth -= value;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                StartCoroutine(DeathAnimation());
                return;
            }
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// Verifica se o value + a vida atual se é maior que a vida máxima
        /// </summary>
        /// <param name="value">Valor para o ganho de vida</param>
        public void Heal(float value)
        {
            if (value + _currentHealth > _maxHealth) _currentHealth = _maxHealth;
            else _currentHealth += value;
        }
        /// <summary>
        /// Muda a variável fillAmount da barra de vida para um valor recebido entre a interpolação do valor do fillAmount
        /// + a divisão entre a vida atual com a vida máxima para dar a percentagem de vida atual que ainda resta.
        /// </summary>
        private void HealthBarFillAmount()
        {
            _healthBar.fillAmount = Mathf.Lerp(_healthBar.fillAmount, _currentHealth / _maxHealth, _fillAmountSpeed * Time.deltaTime);
        }
        /// <summary>
        /// Coroutina que verifica se o inimigo que utiliza o script se chama LastBoss ou se usa o animador de Boss para dar inicio
        /// às animações de morte.
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator DeathAnimation()
        {

            if (transform.root.name == "LastBoss")
            {
                LevelManager.Instance.endCredits = true;
                LevelManager.Instance.StartCoroutine(LevelManager.Instance.TransitionCoroutine("CreditScene", _backgroundSprite));
                Destroy(transform.root.gameObject);

            }
            else if (_bossAnimator != null)
            {
                _bossAnimator.gameObject.GetComponent<CircleCollider2D>().enabled = false;
                _bossAnimator.SetBool("hurt", true);
                yield return _sleep05;
                _bossAnimator.SetBool("hurt", false);
                _flashFX.Play("DeadFX");
                yield return _bossAnimator.gameObject.GetComponent<EnemyBoss>().StartCoroutine(_bossAnimator.gameObject.GetComponent<EnemyBoss>().BossOver());
                if (_chest != null)
                {
                    _chest.SetActive(true);
                    _chest.GetComponent<BoxCollider2D>().enabled = true;
                    _chest.GetComponent<CapsuleCollider2D>().enabled = true;
                }
                Destroy(_bossAnimator.gameObject);
            }
            if (_enemyAI != null)
            {
                _enemyAI.gameObject.GetComponent<CircleCollider2D>().enabled = false;
                if (_heartForRegainingHealth != null)
                {
                    int num = UnityEngine.Random.Range(1, 5);

                    if (num < 3)
                        Instantiate(_heartForRegainingHealth, transform.root.position, Quaternion.identity);
                }
                Destroy(_enemyAI.gameObject, 0.5f);
            }
        }
        /// <summary>
        /// Verifica se as variáveis são nulas para não darem nenhum erro.
        /// Primeiro inicia a animação de hurt
        /// Segundo caso a primeira dê falso, modifica os valores em alpha entre 0 e 1 por um determinado período de tempo
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator FadeIn()
        {
            if (_bossAnimator != null)
            {
                _bossAnimator.SetBool("hurt", true);
                yield return _sleep05;
                _bossAnimator.SetBool("hurt", false);
            }
            else
            {
                if (_enemyAI != null)
                    _enemyAI._playerCollide = true;
                Color tmp = _sr.color;
                tmp.a = 0f;
                _sr.color = tmp;
                yield return _sleep01;
                tmp.a = 1f;
                _sr.color = tmp;
                yield return _sleep01;
                tmp.a = 0f;
                _sr.color = tmp;
                yield return _sleep01;
                tmp.a = 1f;
                _sr.color = tmp;
                yield return _sleep01;
                tmp.a = 0f;
                _sr.color = tmp;
                yield return _sleep01;
                tmp.a = 1f;
                _sr.color = tmp;
                if (_enemyAI != null)
                    _enemyAI._playerCollide = false;
            }
        }
    }
}
