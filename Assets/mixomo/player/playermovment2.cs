using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement2 : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f; // Degrees per second
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    public Joystick joystick;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on the GameObject.");
        }

        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }
    }

    private void OnEnable()
    {
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
            moveAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
        }
    }

    private void Update()
    {
        Vector2 moveInput = joystick.GetInput();

        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * movementSpeed;

        // Apply movement to the Rigidbody

        // Optional: Rotate the player to face the movement direction
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    
    // Check if grounded
    isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keep player grounded
        }

        if (moveAction != null)
        {
            Vector2 movementInput = moveAction.ReadValue<Vector2>();
            Vector3 movementDirection = new Vector3(movementInput.x, 0f, movementInput.y);

            // Move the player
            characterController.Move(movementDirection * movementSpeed * Time.deltaTime);

            // Rotate the player
            if (movementDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}
