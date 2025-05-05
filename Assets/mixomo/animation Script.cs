using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public Animator myAnim; // Reference to the Animator component
    public PlayerController playerController; // Reference to the PlayerController

    void Start()
    {

        // Get the Animator component
        myAnim = GetComponent<Animator>();

        // Ensure the PlayerController is assigned
        if (playerController == null)
        {
            Debug.LogError("PlayerController reference is not set in AnimationScript.");
        }
    }

    void Update()
    {
        // Update animation parameters based on PlayerController's state
        if (playerController != null)
        {
            // Check if the player is running
            bool isRunning = playerController.IsRunning();
            myAnim.SetBool("run", isRunning);

            // Check if the player is jumping
            bool isJumping = playerController.IsJumping();
            myAnim.SetBool("jump", isJumping);

            
        }
    }
}
