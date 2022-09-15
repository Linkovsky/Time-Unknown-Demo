using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SceneNameBorder : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _textMeshPro;
        private Animator _animator;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        /// <summary>
        /// Depois de chamada esta coroutina recebe o nome do nível em que o jogador está e passado 1 segundo
        /// é invocado a função que dá acesso à animação depois espera pelo tempo total da animação e
        /// torna a falso
        /// </summary>
        /// <returns></returns>
        public IEnumerator Board()
        {
            _textMeshPro.text = SceneManager.GetActiveScene().name;
            Invoke("StartAnimation", 1f);
            yield return new WaitForSeconds(4f);
            _animator.Play("Oit");
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
            _textMeshPro.text = string.Empty;
            gameObject.SetActive(false);
        }

        private void StartAnimation() => _animator.Play("In");
    }
}