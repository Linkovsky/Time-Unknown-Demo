using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Credits
{
    /// <summary>
    /// Inicia o Start como um IEnumerator para poder utilizar o yield return que me dá acesso a poder usar delay de 1 minuto,
    /// que é a duração da animação para depois fazer load ao menu principal
    /// </summary>
    public class Credits : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private WaitForSeconds _delay;

        private void Awake()
        {
            _delay = new WaitForSeconds(60f);
        }
        private IEnumerator Start()
        {
            _animator.Play("Credits");
            yield return _delay;
            SceneManager.LoadScene("StartMenu");
        }
    }
}