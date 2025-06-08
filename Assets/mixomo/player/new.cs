//using UnityEngine;
//using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterController))]
//public class PlayerController : MonoBehaviour
//{
//    [SerializeField] private float movementSpeed = 5f;
//    [SerializeField] private float rotationSpeed = 720f; // Degrees per second
//    [SerializeField] private float gravity = -9.81f;
//    [SerializeField] private float jumpHeight = 2f;
//    [SerializeField] private float groundDistance = 0.4f;
//    [SerializeField] private LayerMask groundMask;

//    private CharacterController characterController;
//    private Vector3 velocity;
//    private bool isGrounded;
//    public Joystick joystick; // Reference to the joystick

//    private PlayerInput playerInput;
//    private InputAction moveAction;
//    private InputAction jumpAction;
//    private bool jumpRequested = false;

//    public Animator animator;

//    public Transform cameraa;
//    private float turningSpeed = 2f;
//    private float rotationSmoothing = 0.1f;
//    private Vector3 smoothedLookDirection;
//    [SerializeField] private Transform cameraTransform; // Reference to the camera

//    private void Awake()
//    {
//        velocity.y = 0f;

//        characterController = GetComponent<CharacterController>();

//        // Set up the New Input System
//        playerInput = GetComponent<PlayerInput>();
//        if (playerInput == null)
//        {
//            Debug.LogError("PlayerInput component not found on the GameObject.");
//        }

//        moveAction = playerInput.actions["Move"];
//        jumpAction = playerInput.actions["Jump"];
//    }

//    private void Start()
//    {
//        isGrounded = true;

//        // If cameraTransform is not set, try to find the main camera
//        if (cameraTransform == null)
//        {
//            cameraTransform = Camera.main.transform;
//        }
//    }

//    private void OnEnable()
//    {
//        // Enable the input actions
//        moveAction.Enable();
//        jumpAction.Enable();
//    }



//    public void JumpFromButton()
//    {
//        // Set the jump flag
//        jumpRequested = true;
//    }

//    private void Update()
//    {
//        // Check if the player is grounded
//        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);

//        if (isGrounded && velocity.y < 0)
//        {
//            velocity.y = -2f; // Keep the player grounded
//        }

//        // Get input from the joystick
//        Vector2 moveInput = joystick.GetInput();

//        // Get input from the New Input System
//        Vector2 movementInput = moveAction.ReadValue<Vector2>();

//        // Combine joystick and input action movement (if needed)
//        Vector2 combinedInput = moveInput + movementInput;

//        float deadzone = 0.1f;
//        if (combinedInput.magnitude < deadzone)
//        {
//            combinedInput = Vector2.zero;
//        }
//        else
//        {
//            combinedInput = combinedInput.normalized * ((combinedInput.magnitude - deadzone) / (1 - deadzone));
//        }

//        // Transform the input direction to the camera's coordinate space
//        Vector3 movement = new Vector3(combinedInput.x, 0, combinedInput.y);
//        movement = cameraTransform.TransformDirection(movement);
//        movement.y = 0; // Ensure the movement is horizontal

//        // Move the player
//        if (movement.magnitude > 0)
//        {
//            // Normalize the movement vector to prevent faster diagonal movement
//            movement.Normalize();

//            // Move the character
//            characterController.Move(movement * movementSpeed * Time.deltaTime);

//            // Rotate the player to face the movement direction
//            Quaternion targetRotation = Quaternion.LookRotation(movement);
//            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
//        }

//        // Handle jumping
//        if ((isGrounded && jumpAction.triggered) || (isGrounded && jumpRequested))
//        {
//            animator.SetTrigger("jump");
//            velocity.y = Mathf.Sqrt(jumpHeight * 2f * -gravity);
//            jumpRequested = false; // Reset the jump flag
//        }

//        // Apply gravity
//        velocity.y += gravity * Time.deltaTime;
//        characterController.Move(velocity * Time.deltaTime);
//    }

//    public bool IsRunning()
//    {
//        Vector2 moveInput = joystick.GetInput();
//        Vector2 movementInput = moveAction.ReadValue<Vector2>();
//        return moveInput.magnitude > 0 || movementInput.magnitude > 0;
//    }

//    // Method to check if the player is jumping
//    public bool IsJumping()
//    {
//        return !isGrounded; // or use a dedicated flag if needed
//    }

//    public void Move(Vector3 direction)
//    {
//        characterController.Move(direction * Time.deltaTime);
//        Turn();
//    }
//    public void Turn()
//    {
//        // Get camera forward (flattened)
//        Vector3 targetLookDirection = cameraa.forward;
//        targetLookDirection.y = 0;
//        targetLookDirection.Normalize();

//        // Smooth direction change (dampening)
//        smoothedLookDirection = Vector3.SmoothDamp(
//            smoothedLookDirection,
//            targetLookDirection,
//            ref velocity,
//            rotationSmoothing
//        );

//        // Calculate target rotation
//        Quaternion targetRotation = Quaternion.LookRotation(smoothedLookDirection);

//        // Apply smoothed rotation
//        transform.rotation = Quaternion.Slerp(
//            transform.rotation,
//            targetRotation,
//            turningSpeed * Time.deltaTime
//        );
//    }
//}



using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Ground Detection")]
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;

    [Header("References")]
    public Joystick joystick;
    public Animator animator;
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private bool jumpRequested = false;
    private Vector2 cachedInput;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
#endif

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Update the Animator update mode to use Fixed instead of AnimatePhysics  
        animator.updateMode = AnimatorUpdateMode.Fixed;
        animator.animatePhysics = true;

#if ENABLE_INPUT_SYSTEM
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
        }
#endif
    }

    private void Start()
    {
        StartCoroutine(CheckGroundedRoutine());
    }

    private void Update()
    {
        // Read input each frame (UI or Input System)  
        Vector2 moveInput = joystick != null ? joystick.GetInput() : Vector2.zero;

#if ENABLE_INPUT_SYSTEM
        if (playerInput != null)
        {
            moveInput += moveAction.ReadValue<Vector2>();
            if (jumpAction.triggered)
                jumpRequested = true;
        }
#else
       if (Input.GetButtonDown("Jump"))  
           jumpRequested = true;  

       moveInput += new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));  
#endif

        // Deadzone filtering  
        float deadzone = 0.1f;
        if (moveInput.magnitude < deadzone)
        {
            moveInput = Vector2.zero;
        }
        else
        {
            moveInput = moveInput.normalized * ((moveInput.magnitude - deadzone) / (1 - deadzone));
        }

        cachedInput = moveInput;
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(cachedInput.x, 0, cachedInput.y);

        // Camera relative movement  
        move = cameraTransform.TransformDirection(move);
        move.y = 0f;
        move.Normalize();

        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Jump  
        if (isGrounded && jumpRequested)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("jump");
            jumpRequested = false;
        }

        // Gravity  
        velocity.y += gravity * Time.fixedDeltaTime;

        // Final move  
        Vector3 finalMove = move * movementSpeed;
        finalMove.y = velocity.y;
        characterController.Move(finalMove * Time.fixedDeltaTime);
    }

    private System.Collections.IEnumerator CheckGroundedRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (true)
        {
            Vector3 checkPos = groundCheck != null ? groundCheck.position : transform.position;
            isGrounded = Physics.CheckSphere(checkPos, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            yield return wait;
        }
    }

    // UI Button jump call  
    public void JumpFromButton()
    {
        jumpRequested = true;
    }

    public bool IsRunning()
    {
        return cachedInput.magnitude > 0.1f;
    }

    public bool IsJumping()
    {
        return !isGrounded;
    }
}
