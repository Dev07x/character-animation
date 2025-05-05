using UnityEngine;
using UnityEngine.InputSystem;
public class newScript : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;

    [SerializeField] float speed = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput =GetComponent<PlayerInput>();
        moveAction= playerInput.actions.FindAction("move");


    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }
    void MovePlayer()
    {
        Vector2 direction =moveAction.ReadValue<Vector2>();
        transform.position += new Vector3(direction.x,0, direction.y) * speed * Time.deltaTime;

    }
}
