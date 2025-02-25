using UnityEngine;

public class DiceSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject dicePrefab;             // Dice prefab (with DiceRoller attached)
    public GameObject invisibleFloorPrefab;   // Invisible floor prefab

    [Header("Offsets")]
    public float diceSpawnOffset = 2.0f; // Vertical offset for the dice
    public float floorOffset = 1.5f;     // Vertical offset for the invisible floor

    private GameObject currentDice;
    private GameObject currentFloor;

    // Public property to allow access to the spawned dice.
    public GameObject CurrentDice
    {
        get { return currentDice; }
    }

    public void SpawnDiceAbovePlayer(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            Debug.LogError("DiceSpawner: playerTransform is null!");
            return;
        }

        Vector3 dicePosition = playerTransform.position + Vector3.up * diceSpawnOffset;
/*        Debug.Log("Spawning dice at position: " + dicePosition);*/

        if (dicePrefab == null)
        {
            Debug.LogError("DiceSpawner: dicePrefab is not assigned!");
            return;
        }

        if (currentDice != null)
        {
            Destroy(currentDice);
        }
        currentDice = Instantiate(dicePrefab, dicePosition, Quaternion.identity);
/*        Debug.Log("Dice spawned: " + currentDice.name);*/

        if (invisibleFloorPrefab != null)
        {
            Vector3 floorPosition = playerTransform.position + Vector3.up * floorOffset;
/*            Debug.Log("Positioning invisible floor at: " + floorPosition);*/
            if (currentFloor == null)
            {
                currentFloor = Instantiate(invisibleFloorPrefab, floorPosition, Quaternion.identity);
                Debug.Log("Invisible floor instantiated.");
            }
            else
            {
                currentFloor.transform.position = floorPosition;
/*                Debug.Log("Invisible floor repositioned.");*/
            }
        }
        else
        {
            Debug.LogWarning("DiceSpawner: invisibleFloorPrefab is not assigned!");
        }
    }
}
