using System.Collections;
using UnityEngine;
using TMPro;

public class DiceRoller : MonoBehaviour
{
    // Declare an event to signal that the dice roll has finished.
    public delegate void DiceRollFinishedEventHandler(int result);
    public event DiceRollFinishedEventHandler OnDiceRollFinished;

    [Header("Dice Physics Settings")]
    public Rigidbody rb;
    public float jumpForce = 5f;
    public float torqueForce = 10f;

    [Header("Suspense Settings")]
    [Tooltip("Time the dice will roll extra on the ground for suspense after landing.")]
    public float suspenseDuration = 2f;
    [Tooltip("Interval between extra torque impulses during suspense.")]
    public float extraTorqueInterval = 0.2f;
    [Tooltip("Magnitude of the extra torque impulses.")]
    public float extraTorqueMagnitude = 5f;

    [Header("UI Settings")]
    public TMP_Text resultText; // Displays the current dice number.
    public DiceCameraFollow diceCameraFollow;

    private bool isRolling = false;

    void OnMouseDown()
    {
        // Ensure the dice has a collider and that clicks are enabled.
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
        // Change text color to yellow while rolling.
        resultText.color = Color.yellow;

        // Start continuously updating the text with the current top face.
        Coroutine updateCoroutine = StartCoroutine(UpdateResultText());

        // Reset previous motion.
        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Launch the dice upward and add a random torque.
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Vector3 randomTorque = Random.insideUnitSphere * torqueForce;
        rb.AddTorque(randomTorque, ForceMode.Impulse);

        // Let the dice fly.
        yield return new WaitForSeconds(1f);

        // Wait until the dice slows down (or until a max time is reached).
        float elapsed = 0f;
        float maxWaitTime = 3f;
        while ((rb.velocity.magnitude > 0.1f || rb.angularVelocity.magnitude > 0.1f) && elapsed < maxWaitTime)
        {
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        // Suspense phase: add extra torque impulses.
        float suspenseElapsed = 0f;
        while (suspenseElapsed < suspenseDuration)
        {
            Vector3 extraTorque = Random.insideUnitSphere * extraTorqueMagnitude;
            rb.AddTorque(extraTorque, ForceMode.Impulse);
            yield return new WaitForSeconds(extraTorqueInterval);
            suspenseElapsed += extraTorqueInterval;
        }

        // Allow the dice to settle.
        yield return new WaitForSeconds(1f);

        // Stop updating the text.
        isRolling = false;
        StopCoroutine(updateCoroutine);

        // Final update: determine the top face.
        int finalResult = GetDiceResult();
        resultText.text = finalResult.ToString();
        resultText.color = Color.white;

        // Notify any listener that the roll is finished.
        if (OnDiceRollFinished != null)
        {
            OnDiceRollFinished(finalResult);
        }
    }

    /// <summary>
    /// Continuously updates the displayed number as the dice rolls.
    /// </summary>
    IEnumerator UpdateResultText()
    {
        while (isRolling)
        {
            resultText.text = GetDiceResult().ToString();
            yield return null;
        }
    }

    /// <summary>
    /// Uses the dice's orientation to determine which face is on top.
    /// Mapping (assuming your mesh’s local axes):
    ///   - transform.forward     -> face 1
    ///   - transform.up          -> face 2
    ///   - -transform.right      -> face 3
    ///   - transform.right       -> face 4
    ///   - -transform.up         -> face 5
    ///   - -transform.forward    -> face 6
    /// </summary>
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

        if (dotDown > maxDot)    { maxDot = dotDown;    result = 5; }
        if (dotForward > maxDot) { maxDot = dotForward; result = 1; }
        if (dotBack > maxDot)    { maxDot = dotBack;    result = 6; }
        if (dotRight > maxDot)   { maxDot = dotRight;   result = 4; }
        if (dotLeft > maxDot)    { maxDot = dotLeft;    result = 3; }

        return result;
    }
}
