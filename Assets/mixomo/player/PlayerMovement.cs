using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;

    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on the GameObject.");
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
        if (moveAction != null)
        {
            Vector2 movementInput = moveAction.ReadValue<Vector2>();
            Vector3 movementDirection = new Vector3(movementInput.x, 0f, movementInput.y);

            transform.Translate(movementDirection * movementSpeed * Time.deltaTime);
        }
    }
}
