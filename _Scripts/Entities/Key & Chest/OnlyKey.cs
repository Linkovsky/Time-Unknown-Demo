using RPG.ScriptableObjects.Duplicate;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Entities.Chest
{
    public class OnlyKey : MonoBehaviour
    {
        /// <summary>
        /// Declara��o de vari�veis
        /// </summary>
        [SerializeField] private Vector2 _posToGo;
        [SerializeField] private List<GameObject> _enemies = new();
        [SerializeField] private PreventDuplicates _preventKeyFromSpawning;
        [SerializeField] private bool _willMove;

        private SpriteRenderer _sr;
        private CircleCollider2D _cirlceCollider;

        private bool _enemyIsDead = false;
        private bool _stopUpdating = false;

        /// <summary>
        /// Atribui o componente �s respetivas vari�veis
        /// </summary>
        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _cirlceCollider = GetComponent<CircleCollider2D>();
        }
        /// <summary>
        /// Verifica se o SO � de valor 1 se assim for destr�i o objeto
        /// </summary>
        private void Start()
        {
            _stopUpdating = false;
            if (_preventKeyFromSpawning.numberOfTimes == 1)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Verifica a cada frame se na lista dos inimigos eles ainda permanecem na scene
        /// Se n�o, inicia o processo de movimenta��o do objeto para a localiza��o predifinida
        /// </summary>
        private void Update()
        {
            if (!_stopUpdating)
            {
                if (_enemies.Count != 0)
                    for (int i = 0; i < _enemies.Count; i++)
                    {
                        if (_enemies[i] == null)
                            _enemies.Remove(_enemies[i]);
                    }
                else
                    _enemyIsDead = true;
                if (_enemyIsDead)
                    if (_willMove)
                        transform.position = Vector2.MoveTowards(transform.position, _posToGo, 3f * Time.deltaTime);
                    else if (_sr.enabled == false)
                    {
                        _sr.enabled = true;
                        _cirlceCollider.enabled = true;
                    }
            }
        }
        /// <summary>
        /// Collider em trigger que quando o collision entra em contacto, incrementa o SO que impede de haver multiplos objetos
        /// </summary>
        /// <param name="collision">Representa a colis�o do objeto</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            _stopUpdating = true;
            _preventKeyFromSpawning.numberOfTimes++;
        }
    }
}