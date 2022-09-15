using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.SceneManagement.Timeline
{
    public class TimeLineScript : MonoBehaviour
    {
        /// <summary>
        /// Script utilizada na primeira cutscene depois do new game, faço subscribe ao evento para quando
        /// a cutscene acabar de tocar faz load ao primeiro nível do jogo
        /// </summary>
        [SerializeField] private Sprite something;
        private PlayableDirector playableDirector;

        private void Awake()
        {
            playableDirector = GetComponent<PlayableDirector>();
        }
        private void Start()
        {
            playableDirector.stopped += PlayableDirector_stopped;
        }

        private void PlayableDirector_stopped(PlayableDirector obj)
        {
            LevelManager.Instance.LoadScene("Camp Village", something);
        }
    }
}