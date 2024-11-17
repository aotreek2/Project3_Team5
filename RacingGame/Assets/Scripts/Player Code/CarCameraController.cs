using UnityEngine;

public class CarCameraController : MonoBehaviour
{
    public Transform car; // Reference to the car (parent) transform
    public float rotationSpeed = 100f; // Speed of camera rotation
    public float returnRotationSpeed = 2f; // Speed for returning to the original rotation
    public float returnPositionSpeed = 2f; // Speed for returning to the original position
    public float returnDelay = 1f; // Delay before returning to original position/rotation

    private Quaternion targetRotation; // Target rotation for the camera (9, 0, 0)
    private Vector3 targetPosition; // Target position for the camera (0, 3, -8)
    private float inactivityTimer = 0f; // Timer for inactivity

    private void Start()
    {
        // Define the target rotation and position
        targetRotation = Quaternion.Euler(9, 0, 0);
        targetPosition = new Vector3(0, 3, -8);

        // Set the initial position and rotation to match the target values exactly
        transform.localPosition = targetPosition;
        transform.localRotation = targetRotation;
    }

    private void LateUpdate()
    {
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            // Reset inactivity timer when user input is detected
            inactivityTimer = 0f;

            // Rotate the camera around the car’s position based on mouse input
            transform.RotateAround(car.position, Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
            transform.RotateAround(car.position, transform.right, -verticalInput * rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Increment the inactivity timer if there’s no input
            inactivityTimer += Time.deltaTime;

            // If inactivity exceeds the delay, start returning to the target position and rotation
            if (inactivityTimer >= returnDelay)
            {
                // Smoothly return the camera to its initial rotation
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * returnRotationSpeed);

                // Smoothly move the camera back to its initial position
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * returnPositionSpeed);
            }
        }
    }

    public void ConfineCursor()
    {
        // Confine the cursor to the game window and make it invisible
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // Confine the cursor when the game window is focused
        if (hasFocus)
        {
            ConfineCursor();
        }
    }
}
