using UnityEngine;
using Fusion;

public class CameraController : NetworkBehaviour
{
    public float sensitivityX = 10f;
    public float sensitivityY = 10f;
    public Transform playerBody; // Reference to the player object or body

    private float verticalRotation;
    private float horizontalRotation;

    void LateUpdate()
    {
        if (playerBody == null)
        {
            return;
        }
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            transform.position = playerBody.position;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            verticalRotation -= mouseY * sensitivityX;
            verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);
            horizontalRotation += mouseX * sensitivityX;
            transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
        }
    }
}
