using System.Collections;
using UnityEngine;

public class playerStutus : MonoBehaviour
{
    [SerializeField] private float maxHealth;

    private float currentHealth;

    public HealthBar healthBar;

    public Animator animator;
    public bool isDead = false;

    public static System.Action OnPlayerDeath;

    audioManger audioManger;
    void Start()
    {
        GameObject audioObject = GameObject.FindGameObjectWithTag("audio");
        if (audioObject != null)
        {
            audioManger = audioObject.GetComponent<audioManger>();
        }
        currentHealth = maxHealth;
        healthBar.SetSliderMax(maxHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (other.CompareTag("enemySwoard"))
        {
            TakeDamage();
        }

    }
    public void TakeDamage()
    {
        if (isDead) return;
        currentHealth -= 10;
        audioManger.playSfx(audioManger.Playerhurt);

        healthBar.SetSlider(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        isDead = true;
        if (animator != null)
        {
            animator.SetTrigger("death");
            audioManger.playSfx(audioManger.PlayerDeath);

        }
            GetComponent<Collider>().enabled = false;

            // Optional: Add delay before destroying object or showing game over
            // StartCoroutine(GameOverAfterDelay(3f));
            OnPlayerDeath?.Invoke();
        StartCoroutine(ShowGameOverAfterDelay(5f));
    }

    // Optional coroutine for game over sequence
    private IEnumerator ShowGameOverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Implement your game over logic here
        // Example: SceneManager.LoadScene("GameOverScene");
        GameManager.instance.ShowGameOver();

    }
}
