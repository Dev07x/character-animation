
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player; // Reference to the player's transform
    [SerializeField] private Vector3 cameraPosition = new Vector3(0, 2, -5); // Camera position relative to player
    [SerializeField] private float smoothSpeed = 0.125f; // Smooth follow speed
    [SerializeField] private float rotationSpeed = 0.2f; // Adjust this for sensitivity
    private Vector3 velocity = Vector3.zero;
    private void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is not set in CameraFollow script.");
            return;
        }

        // Handle touch input for camera rotation
        if (Input.touchCount > 0) // Check if there are touches on the screen
        {
            Touch touch = Input.GetTouch(0); // Get the first touch (single touch)

            if (touch.phase == TouchPhase.Moved) // Check if the finger is moving
            {
                // Rotate the camera position based on touch movement
                float horizontalInput = touch.deltaPosition.x * rotationSpeed * Time.deltaTime;
                Quaternion rotation = Quaternion.Euler(0, horizontalInput, 0);
                cameraPosition = rotation * cameraPosition;
            }
        }

        // Calculate the desired position (player position + camera position)
        Vector3 desiredPosition = player.position + cameraPosition;

        transform.position = Vector3.SmoothDamp(
        transform.position,
        desiredPosition,
        ref velocity,
        smoothSpeed
    );
        transform.LookAt(player);
    }

    // Public method to manually set the camera's position relative to the player
    public void SetCameraPosition(Vector3 newPosition)
    {
        cameraPosition = newPosition;
    }
}