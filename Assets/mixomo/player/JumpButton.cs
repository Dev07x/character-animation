using UnityEngine;
using UnityEngine.UI;

public class JumpButton : MonoBehaviour
{
    [SerializeField] private PlayerController playerController; // Reference to the PlayerController
    private Button jumpButton; // Reference to the UI Button

    private void Awake()
    {
        // Get the Button component
        jumpButton = GetComponent<Button>();

        // Ensure the PlayerController is assigned
        if (playerController == null)
        {
            Debug.LogError("PlayerController reference is not set in JumpButton.");
            return;
        }

        // Add a listener to the button to call the OnJumpButtonClicked method when clicked
        jumpButton.onClick.AddListener(OnJumpButtonClicked);
    }

    private void OnJumpButtonClicked()
    {
        // Trigger the jump action in the PlayerController
        playerController.JumpFromButton();
    }

    private void OnDestroy()
    {
        // Remove the listener when the object is destroyed to avoid memory leaks
        if (jumpButton != null)
        {
            jumpButton.onClick.RemoveListener(OnJumpButtonClicked);
        }
    }
}