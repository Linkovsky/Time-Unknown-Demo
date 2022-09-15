using RPG.SceneManagement;
using RPG.ScriptableObjects.Duplicate;
using System.Collections;
using UnityEngine;

namespace RPG.Entities.Chest
{
    public class SwitchAndFadeIn : MonoBehaviour
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        [SerializeField] private GameObject _stopPlayerCollider;
        [SerializeField] private Material _material;
        [SerializeField] private float _endValue;
        [SerializeField] private float _startValue;
        [SerializeField] PreventDuplicates _preventDoubleAction;
        [SerializeField] private AudioClip _audioClip;

        /// <summary>
        /// Verifica se o mesmo já foi utilizado pelo jogador neste caso se o SO ultrpassa o valor 0
        /// Se sim modifica o material para ficar escondido ou visível
        /// </summary>
        private void Start()
        {
            if (_preventDoubleAction.numberOfTimes != 0)
            {
                _material.SetFloat("_DissolveAmount", _endValue);
                Destroy(_stopPlayerCollider);
            }
            else
            {
                _material.SetFloat("_DissolveAmount", _startValue);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Shuriken"))
                if (_preventDoubleAction.numberOfTimes == 0)
                {
                    LevelManager.Instance._audioSource.PlayOneShot(_audioClip);
                    _preventDoubleAction.numberOfTimes++;
                    StartCoroutine(Dissolve());
                }
        }

        /// <summary>
        /// Coroutina que inicia o processo de dissolver do material
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator Dissolve()
        {
            float currentTime = 0f;
            float transitionTime = 1f;

            while (currentTime < transitionTime)
            {
                currentTime += Time.deltaTime * 2;
                float totalTime = currentTime / transitionTime;
                float value = Mathf.Lerp(_startValue, _endValue, totalTime);
                _material.SetFloat("_DissolveAmount", value);
                yield return null;

            }

            Destroy(_stopPlayerCollider);
        }
    }
}