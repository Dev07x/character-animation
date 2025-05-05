using UnityEngine;

public class audioManger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("--Audio Clips--")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;


    [Header("--Audio Clips--")]
    public AudioClip background;
    public AudioClip PlayerDeath;
    public AudioClip enemyDeath;
    public AudioClip Playerhurt;
    public AudioClip enemyHurt;
    public AudioClip SwordSlash;
    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }
    public void playSfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}

