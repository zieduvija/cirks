using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DiceCameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform dice;          // Assign the dice transform.
    public Vector3 offset = new Vector3(0, 10, 0);  // Camera offset (above the dice).
    public float followSpeed = 5f;  
    public float rotationSpeed = 5f;

    [Header("UI Display Settings")]
    public RawImage cameraDisplay;  // UI RawImage element that shows the camera feed.
    [Tooltip("Time (in seconds) the camera view remains fully visible.")]
    public float displayDuration = 2f; 
    [Tooltip("Time (in seconds) for the fade in/out transitions.")]
    public float fadeDuration = 0.5f; 

    private Camera diceCamera;

    void Start()
    {
        diceCamera = GetComponent<Camera>();

        if (cameraDisplay != null)
        {
            // Start fully transparent.
            Color col = cameraDisplay.color;
            col.a = 0f;
            cameraDisplay.color = col;
        }
    }

    void Update()
    {
        if (dice != null)
        {
            // Smoothly follow the dice's position (with the specified offset).
            Vector3 targetPos = dice.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

            // Rotate to face the dice.
            Vector3 direction = dice.position - transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    /// <summary>
    /// Starts the coroutine that fades the camera view in, holds it, then fades it out.
    /// </summary>
    public void ShowCameraView()
    {
        if (cameraDisplay != null)
        {
            StartCoroutine(FadeCameraViewCoroutine());
        }
    }

    IEnumerator FadeCameraViewCoroutine()
    {
        // Fade in.
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            SetCameraDisplayAlpha(alpha);
            yield return null;
        }
        SetCameraDisplayAlpha(1f);

        // Hold fully visible.
        yield return new WaitForSeconds(displayDuration);

        // Fade out.
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            SetCameraDisplayAlpha(alpha);
            yield return null;
        }
        SetCameraDisplayAlpha(0f);
    }

    void SetCameraDisplayAlpha(float alpha)
    {
        if (cameraDisplay != null)
        {
            Color col = cameraDisplay.color;
            col.a = alpha;
            cameraDisplay.color = col;
        }
    }
}
