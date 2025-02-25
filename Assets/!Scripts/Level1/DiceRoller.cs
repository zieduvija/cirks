using System.Collections;
using UnityEngine;
using TMPro;

public class DiceRoller : MonoBehaviour
{
    [Header("Dice Physics Settings")]
    public Rigidbody rb;
    public float jumpForce = 5f;
    public float torqueForce = 10f;

    [Header("Suspense Settings")]
    public float suspenseDuration = 2f;
    public float extraTorqueInterval = 0.2f;
    public float extraTorqueMagnitude = 5f;

    [Header("UI Settings")]
    public TMP_Text resultText; // Will be assigned at runtime if not set.
    public DiceCameraFollow diceCameraFollow;

    [Header("Computer Appearance")]
    [Tooltip("Optional texture to use for the dice when it's unclickable (computer's turn).")]
    public Texture computerTexture;

    // Components for interactivity.
    private Collider diceCollider;
    private Renderer diceRenderer;
    private Color originalColor;
    private Texture originalTexture; // Store the original texture.

    // Instead of disabling the collider, we use this flag to ignore clicks.
    private bool isInteractable = true;
    private bool isRolling = false;

    public delegate void DiceRollFinishedEventHandler(int result);
    public event DiceRollFinishedEventHandler OnDiceRollFinished;

    void Awake()
    {
        diceCollider = GetComponent<Collider>();
        diceRenderer = GetComponent<Renderer>();
        if (diceRenderer != null)
        {
            originalColor = diceRenderer.material.color;
            originalTexture = diceRenderer.material.mainTexture;
        }
    }

    void OnMouseDown()
    {
        if (!isInteractable)
            return; // Ignore clicks when not interactable.
        if (!isRolling)
        {
            if (diceCameraFollow != null)
                diceCameraFollow.ShowCameraView();
            StartCoroutine(RollDiceCoroutine());
        }
    }

    // Called to simulate a roll (for computer turns).
    public void SimulateRoll()
    {
        if (!isRolling)
        {
            if (diceCameraFollow != null)
                diceCameraFollow.ShowCameraView();
            StartCoroutine(RollDiceCoroutine());
        }
    }

    IEnumerator RollDiceCoroutine()
    {
        isRolling = true;
        resultText.color = Color.yellow;

        Coroutine updateCoroutine = StartCoroutine(UpdateResultText());

        // Reset physics.
        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Launch dice upward and apply random torque.
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Vector3 randomTorque = Random.insideUnitSphere * torqueForce;
        rb.AddTorque(randomTorque, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);

        // Wait until dice slows down.
        float elapsed = 0f;
        float maxWaitTime = 3f;
        while ((rb.velocity.magnitude > 0.1f || rb.angularVelocity.magnitude > 0.1f) && elapsed < maxWaitTime)
        {
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        // Suspense phase: apply extra torque.
        float suspenseElapsed = 0f;
        while (suspenseElapsed < suspenseDuration)
        {
            Vector3 extraTorque = Random.insideUnitSphere * extraTorqueMagnitude;
            rb.AddTorque(extraTorque, ForceMode.Impulse);
            yield return new WaitForSeconds(extraTorqueInterval);
            suspenseElapsed += extraTorqueInterval;
        }

        yield return new WaitForSeconds(1f);

        isRolling = false;
        StopCoroutine(updateCoroutine);

        int finalResult = GetDiceResult();
        resultText.text = finalResult.ToString();
        resultText.color = Color.white;

        if (OnDiceRollFinished != null)
            OnDiceRollFinished(finalResult);
    }

    IEnumerator UpdateResultText()
    {
        while (isRolling)
        {
            resultText.text = GetDiceResult().ToString();
            yield return null;
        }
    }

    int GetDiceResult()
    {
        float dotUp = Vector3.Dot(transform.up, Vector3.up);             // face 2
        float dotDown = Vector3.Dot(-transform.up, Vector3.up);            // face 5
        float dotForward = Vector3.Dot(transform.forward, Vector3.up);     // face 1
        float dotBack = Vector3.Dot(-transform.forward, Vector3.up);       // face 6
        float dotRight = Vector3.Dot(transform.right, Vector3.up);         // face 4
        float dotLeft = Vector3.Dot(-transform.right, Vector3.up);         // face 3

        float maxDot = dotUp;
        int result = 2;
        if (dotDown > maxDot) { maxDot = dotDown; result = 5; }
        if (dotForward > maxDot) { maxDot = dotForward; result = 1; }
        if (dotBack > maxDot) { maxDot = dotBack; result = 6; }
        if (dotRight > maxDot) { maxDot = dotRight; result = 4; }
        if (dotLeft > maxDot) { maxDot = dotLeft; result = 3; }
        return result;
    }

    /// <summary>
    /// Sets whether the dice is interactable.
    /// If not interactable, the dice material's texture is replaced with computerTexture (if assigned)
    /// and its color is set to gray.
    /// When interactable, the original texture and color are restored.
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        if (diceRenderer != null)
        {
            if (interactable)
            {
                diceRenderer.material.color = originalColor;
                diceRenderer.material.mainTexture = originalTexture;
            }
            else
            {
                diceRenderer.material.color = Color.gray;
                if (computerTexture != null)
                    diceRenderer.material.mainTexture = computerTexture;
            }
        }
    }
}
