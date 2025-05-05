//using UnityEngine;
//using UnityEngine.UI;

//public class PlayerAttack : MonoBehaviour
//{
//    public Animator playerAnimator; // Reference to the player's Animator component
//    public Button attackButton; // Reference to the UI Button
//    public PlayerController playerController; // Reference to the PlayerController
//    private void Start()
//    {
//        // Assign the onClick event to the Attack method
//        if (attackButton != null)
//        {
//            attackButton.onClick.AddListener(Attack);
//        }
//        if (playerAnimator != null)
//        {
//            playerAnimator.applyRootMotion = false;
//        }
//    }

//    private void Attack()
//    {
//        // Trigger the attack animation
//        if (playerAnimator != null)
//        {
//            playerAnimator.SetTrigger("attack1"); // "Attack" is the name of the trigger parameter in the Animator
//        }

//    }

//}
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public Animator playerAnimator; // Reference to the player's Animator component
    public Button attackButton; // Reference to the UI Button

    private void Start()
    {
        // Assign the onClick event to the Attack method
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(Attack);
        }
        //if (playerAnimator != null)
        //{
        //    playerAnimator.applyRootMotion = false;
        //}
    }

    private void Attack()
    {
        // Trigger the attack animation
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("attack1"); // "attack1" is the name of the trigger parameter in the Animator
        }
    }
}

