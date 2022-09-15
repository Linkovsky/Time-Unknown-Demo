using RPG.PlayerControlls.Health;
using RPG.SceneManagement;
using UnityEngine;

public class RegainHealthScript : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;
    private AudioSource _audioSource;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && collision.TryGetComponent(out PlayerHealth health))
        {
            LevelManager.Instance._audioSource.PlayOneShot(_audioClip);
            health.PlayerHeal(UnityEngine.Random.Range(1, 4));
            Destroy(this.gameObject);
        }
    }
}
