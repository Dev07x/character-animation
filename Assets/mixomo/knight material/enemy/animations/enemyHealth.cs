using UnityEngine;
using UnityEngine.AI;

public class enemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float currentHealth;
    public HealthBar healthBar;
    public Animator animator;
    private AiScript aiScript; // Reference to the AI script
    private NavMeshAgent agent; // Reference to the NavMeshAgent

    audioManger audioManger;
    void Start()
    {
        GameObject audioObject = GameObject.FindGameObjectWithTag("audio");
        if (audioObject != null)
        {
            audioManger = audioObject.GetComponent<audioManger>();
        }
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        aiScript = GetComponent<AiScript>();
        currentHealth = maxHealth;
        healthBar.SetSliderMax(maxHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("sss"))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        currentHealth -= 10;
        audioManger.playSfx(audioManger.enemyHurt);

        healthBar.SetSlider(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Disable AI and navigation
        if (aiScript != null)
        {
            aiScript.enabled = false;
        }

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // Trigger death animation
        if (animator != null)
        {
            animator.SetTrigger("death");
            audioManger.playSfx(audioManger.enemyDeath);

        }

        // Disable colliders (optional)
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Destroy the object after animation finishes
        //Destroy(gameObject, 5f); // Adjust time based on your animation length

        //Debug.Log("Enemy died");
    }
}