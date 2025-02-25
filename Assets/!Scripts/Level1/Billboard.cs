using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Automatically get the camera from the CameraFollow script (or fallback to Camera.main)
        CameraFollow camFollow = FindObjectOfType<CameraFollow>();
        if (camFollow != null)
        {
            mainCamera = camFollow.GetComponent<Camera>();
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (mainCamera == null)
        {
            Debug.LogError("Billboard: No camera found!");
        }
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Calculate a target position at the camera's XZ position and this object's Y.
            Vector3 targetPosition = mainCamera.transform.position;
            targetPosition.y = transform.position.y;
            // Make the object look at the target.
            transform.LookAt(targetPosition);
            // Then rotate 180 degrees on Y to fix mirror issues with text.
            transform.Rotate(0, 180, 0);
        }
    }
}
