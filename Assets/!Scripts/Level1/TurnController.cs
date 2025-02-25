using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    [Header("UI Reference")]
    // Drag the GameObject with the CurrentMoveUI script into this field.
    public CurrentMoveUI currentMoveUI;

    // A list to hold references to all players (via their NameScript component).
    private List<NameScript> players = new List<NameScript>();
    private int currentPlayerIndex = 0;

    void Start()
    {
        // Find all players that have a NameScript in the scene.
        NameScript[] foundPlayers = FindObjectsOfType<NameScript>();
        players.AddRange(foundPlayers);

        if (players.Count == 0)
        {
            Debug.LogWarning("No players found for turn management!");
        }
        else
        {
            // At game start, update the UI with the first player's information.
            UpdateCurrentMoveUI();
        }
    }

    /// <summary>
    /// Call this method when a turn ends to advance to the next player's turn.
    /// </summary>
    public void NextTurn()
    {
        if (players.Count == 0) return;

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        UpdateCurrentMoveUI();
    }

    /// <summary>
    /// Updates the Current Move UI using the current player's data.
    /// </summary>
    void UpdateCurrentMoveUI()
    {
        if (currentMoveUI != null && players.Count > 0)
        {
            NameScript currentPlayer = players[currentPlayerIndex];
            string playerName = currentPlayer.GetPlayerName();

            // For the avatar, we assume each player's prefab has a SpriteRenderer.
            Sprite avatar = null;
            SpriteRenderer sr = currentPlayer.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                avatar = sr.sprite;
            }
            // Update the UI.
            currentMoveUI.UpdateCurrentMove(playerName, avatar);
        }
        else
        {
            Debug.LogWarning("CurrentMoveUI reference missing or no players available!");
        }
    }
}
