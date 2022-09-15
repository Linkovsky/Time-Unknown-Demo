using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.UI.Dialogue
{

    public class TypeWriterEffect : MonoBehaviour
    {
        /// <summary>
        /// Declara��o de vari�veis
        /// </summary>
        [SerializeField] private float _writeSpeed = 50f;
        [SerializeField] private AudioClip _dialogueSound;

        private AudioSource _dialogueAudioSource;
        public bool IsRunning { get; private set; }

        private void Awake()
        {
            _dialogueAudioSource = GetComponent<AudioSource>();
        }
        /// <summary>
        /// Lista para ler apenas as pontua��es com um tempo definido de espera
        /// </summary>
        private readonly List<Punctuation> punctuations = new()
        {
            new Punctuation(new HashSet<char>() { '.', '!', '?' }, 0.6f),
            new Punctuation(new HashSet<char>() { ',', ';', ':' }, 0.3f)
        };
        private Coroutine _typingCoroutine;
        /// <summary>
        /// Inicia a coroutina que ir� dar inicio � "escrita" na caixa de di�logo
        /// </summary>
        /// <param name="textToType">A frase contida na string do SO</param>
        /// <param name="text">O texto do Textmeshpro que � para modificar</param>
        /// <param name="charName">O nome do NPC</param>
        /// <param name="nameText">O texto textmeshpro do nome</param>
        public void Run(string textToType, TextMeshProUGUI text, string charName, TextMeshProUGUI nameText)
        {
            _typingCoroutine = StartCoroutine(TypeText(textToType, text, charName, nameText));
        }
        /// <summary>
        /// Para a coroutina de imediato
        /// </summary>
        public void Stop()
        {

            StopCoroutine(_typingCoroutine);
            IsRunning = false;

        }
        /// <summary>
        /// Todas as strings s�o colocadas predefinidas como empty para n�o haver problemas caso j� exista algo que poderiam conter
        /// Depois � tudo feito num while, enquanto o o index atual n�o for menor que o total length da string
        /// � utilizado um for para ler cada caracter e verifica se dentro da string existe alguma pontua��o que est� definida na lista
        /// caso exista � feita uma esp�cie de parar o tempo para dar o efeito de uma pequena pausa enquanto a pessoa l�
        /// </summary>
        private IEnumerator TypeText(string textToType, TextMeshProUGUI text, string charName, TextMeshProUGUI nameText)
        {
            IsRunning = true;
            text.text = string.Empty;
            nameText.text = string.Empty;
            float t = 0;
            int charIndex = 0;
            nameText.text = charName;

            while (charIndex < textToType.Length)
            {
                int lastCharIndex = charIndex;
                t += Time.deltaTime * _writeSpeed;
                charIndex = Mathf.FloorToInt(t);
                charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);
                for (int i = lastCharIndex; i < charIndex; i++)
                {
                    bool isLast = i >= textToType.Length - 1;
                    text.text = textToType.Substring(0, i + 1);
                    _dialogueAudioSource.PlayOneShot(_dialogueSound);
                    if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i + 1], out _))
                    {
                        yield return new WaitForSeconds(waitTime);
                    }
                }
                yield return null;
            }
            IsRunning = false;

        }

        /// <summary>
        /// Entra num ciclo foreach que verifica se existe um dos caracteres que foram adicionados na lista 
        /// Se for verdadeiro ent�o o waittime � adicionado o tempo predefinido na struct
        /// </summary>
        /// <param name="character"></param>
        /// <param name="waitTime"></param>
        /// <returns>Verdadeiro se conter caracter, Falso se n�o tiver</returns>
        private bool IsPunctuation(char character, out float waitTime)
        {
            foreach (Punctuation punctuationCategory in punctuations)
            {
                if (punctuationCategory.Punctuations.Contains(character))
                {
                    waitTime = punctuationCategory.WaitTime;
                    return true;
                }
            }
            waitTime = default;
            return false;
        }
        /// <summary>
        /// Estrutura que � utilizada pela lista para adicionar a pontua��o e o seu respetivo tempo de espera
        /// </summary>
        private readonly struct Punctuation
        {
            public readonly HashSet<char> Punctuations;
            public readonly float WaitTime;

            public Punctuation(HashSet<char> punctuations, float waitTime)
            {
                Punctuations = punctuations;
                WaitTime = waitTime;
            }
        }
    }
}
