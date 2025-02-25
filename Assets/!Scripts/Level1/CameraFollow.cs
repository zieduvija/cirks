using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Orbit Settings")]
    [Tooltip("The target that the camera follows.")]
    public Transform target;
    [Tooltip("Distance from the target.")]
    public float orbitRadius = 10f;
    [Tooltip("Vertical offset from the target.")]
    public float heightOffset = 5f;
    [Tooltip("Initial horizontal angle (in degrees).")]
    public float orbitAngle = 0f;

    [Header("Follow & Rotation")]
    [Tooltip("Speed at which the camera follows the target.")]
    public float followSpeed = 5f;
    [Tooltip("How fast the orbit angle changes with mouse drag.")]
    public float rotationSpeed = 0.2f;

    [Header("Zoom (FOV) Settings")]
    [Tooltip("How much the field of view changes per scroll input.")]
    public float zoomSensitivity = 2f;
    [Tooltip("Minimum field of view.")]
    public float minFOV = 15f;
    [Tooltip("Maximum field of view.")]
    public float maxFOV = 40f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (target == null)
        {
            Debug.LogError("CameraFollow: No target assigned!");
        }
    }

    void Update()
    {
        // Adjust orbit angle based on horizontal mouse drag (right mouse button)
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            orbitAngle += mouseX * rotationSpeed;
        }

        // Zoom: adjust FOV with scroll wheel (does not affect orbit radius)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.fieldOfView -= scroll * zoomSensitivity;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the camera's desired position along the orbit.
            float rad = orbitAngle * Mathf.Deg2Rad;
            Vector3 desiredPosition = target.position + new Vector3(Mathf.Cos(rad) * orbitRadius, heightOffset, Mathf.Sin(rad) * orbitRadius);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            transform.LookAt(target.position);
        }
    }
}
