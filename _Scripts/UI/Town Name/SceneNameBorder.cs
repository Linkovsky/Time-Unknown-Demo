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
        /// Depois de chamada esta coroutina recebe o nome do n�vel em que o jogador est� e passado 1 segundo
        /// � invocado a fun��o que d� acesso � anima��o depois espera pelo tempo total da anima��o e
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