using UnityEngine;

public class enemyAiScript : MonoBehaviour
{
    private Animator animator;
    public PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController reference is not set in AnimationScript.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController != null)
        {

            bool isRunning = playerController.IsRunning();
            animator.SetBool("run", isRunning);
           
        }
    }
}
