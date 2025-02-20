using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CirksGameManager : MonoBehaviour
{
    [Header("UI References")]
    public CurrentMove currentMoveUI;    // Panel that shows current player's name and avatar.
    public TMP_Text diceResultText;        // Displays the dice roll result.
    public TMP_Text messageText;           // Displays game messages/instructions.

    [Header("References")]
    public DiceRoller diceRoller;          // Reference to your DiceRoller component.
    public BoardManager boardManager;     // Reference to your BoardManager (holds tile order/effects).

    // List of players (found via their NameScript component).
    private List<NameScript> players = new List<NameScript>();
    // Track each player's current tile index (starting at 0).
    private int[] playerTileIndices;
    // Index of the current player whose turn it is.
    private int currentPlayerIndex = 0;
    // Flag to indicate whether the game has started (i.e. at least one player has rolled a valid start).
    private bool gameStarted = false;

    IEnumerator Start()
    {
        Debug.Log("CirksGameManager Start() called.");

        // Wait until at least one player (with a NameScript) is instantiated.
        while (FindObjectsOfType<NameScript>().Length == 0)
        {
            Debug.Log("Waiting for players to be instantiated...");
            yield return null;
        }
        
        // Gather all players.
        NameScript[] foundPlayers = FindObjectsOfType<NameScript>();
        players.AddRange(foundPlayers);
        Debug.Log("Players found: " + players.Count);
        if (players.Count == 0)
        {
            Debug.LogError("No players found! Ensure your players have a NameScript attached.");
            yield break;
        }

        // Initialize tracking array: all players start at tile index 0.
        playerTileIndices = new int[players.Count];
        for (int i = 0; i < players.Count; i++)
        {
            playerTileIndices[i] = 0;
        }

        UpdateCurrentMoveUI();
        messageText.text = "Current player: " + players[currentPlayerIndex].GetPlayerName() +
                           ". Roll the dice to start (roll 1 or 6).";

        // Subscribe to the dice roll finished event.
        if (diceRoller != null)
        {
            diceRoller.OnDiceRollFinished += HandleDiceRollFinished;
        }
        else
        {
            Debug.LogError("DiceRoller reference not assigned in CirksGameManager!");
        }
    }

    /// <summary>
    /// Called when the DiceRoller finishes its roll.
    /// </summary>
    /// <param name="result">The final dice roll result.</param>
    void HandleDiceRollFinished(int result)
    {
        Debug.Log("Dice roll finished with result: " + result);
        diceResultText.text = result.ToString();

        if (!gameStarted)
        {
            // Starting phase: Only one valid start is needed.
            if (result == 1 || result == 6)
            {
                // Advance this player to tile 6 regardless of whether they rolled a 1 or a 6.
                int startingTile = 6;
                playerTileIndices[currentPlayerIndex] = startingTile;
                StartCoroutine(boardManager.MovePlayerToTile(players[currentPlayerIndex].gameObject, startingTile));
                messageText.text = players[currentPlayerIndex].GetPlayerName() + " has started and advanced to tile " + startingTile + "!";
                Debug.Log(players[currentPlayerIndex].GetPlayerName() + " has started and advanced to tile " + startingTile + "!");
                
                // Set the game as started.
                gameStarted = true;
            }
            else
            {
                // Invalid start: do nothing for movement.
                messageText.text = players[currentPlayerIndex].GetPlayerName() + " did not roll a valid start (" + result + "). Next player's turn.";
                Debug.Log(players[currentPlayerIndex].GetPlayerName() + " did not roll a valid start (" + result + ").");
            }
            NextTurn();
        }
        else
        {
            // Normal gameplay: move the current player's piece by the dice roll value.
            int currentTile = playerTileIndices[currentPlayerIndex];
            int targetTile = currentTile + result;
            // Clamp the target tile to board limits using boardManager.tiles.
            if (targetTile >= boardManager.tiles.Length)
                targetTile = boardManager.tiles.Length - 1;

            StartCoroutine(boardManager.MovePlayerToTile(players[currentPlayerIndex].gameObject, targetTile));
            playerTileIndices[currentPlayerIndex] = targetTile;
            Debug.Log(players[currentPlayerIndex].GetPlayerName() + " moved to tile " + targetTile);

            // Check for a tile override first.
            int overrideTile = boardManager.GetTileOverride(targetTile);
            if (overrideTile != -1)
            {
                Debug.Log("Tile override detected on tile " + targetTile + ". Moving directly to tile " + overrideTile);
                StartCoroutine(boardManager.MovePlayerToTile(players[currentPlayerIndex].gameObject, overrideTile));
                playerTileIndices[currentPlayerIndex] = overrideTile;
            }
            else
            {
                // Otherwise, apply the tile's default effect (delta), which is +1 by default.
                int delta = boardManager.GetTileDelta(targetTile);
                if (delta != 0)
                {
                    int newTarget = targetTile + delta;
                    newTarget = Mathf.Clamp(newTarget, 0, boardManager.tiles.Length - 1);
                    Debug.Log("Tile effect: " + delta + " on tile " + targetTile + ". Moving to tile " + newTarget);
                    StartCoroutine(boardManager.MovePlayerToTile(players[currentPlayerIndex].gameObject, newTarget));
                    playerTileIndices[currentPlayerIndex] = newTarget;
                }
            }

            NextTurn();
        }
    }

    /// <summary>
    /// Advances the turn to the next player.
    /// </summary>
    void NextTurn()
    {
        if (players.Count == 0)
            return;

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        Debug.Log("Next turn: " + players[currentPlayerIndex].GetPlayerName());
        UpdateCurrentMoveUI();

        if (!gameStarted)
            messageText.text = "Current player: " + players[currentPlayerIndex].GetPlayerName() +
                                ". Roll the dice to start (roll 1 or 6).";
        else
            messageText.text = "Current player: " + players[currentPlayerIndex].GetPlayerName() + ". Roll the dice.";
    }

    /// <summary>
    /// Updates the Current Move UI with the current player's name and avatar.
    /// </summary>
    void UpdateCurrentMoveUI()
    {
        if (currentMoveUI != null && players.Count > 0)
        {
            NameScript currentPlayer = players[currentPlayerIndex];
            string playerName = currentPlayer.GetPlayerName();

            // For the avatar, we assume the player prefab has a SpriteRenderer component.
            Sprite avatar = null;
            SpriteRenderer sr = currentPlayer.GetComponent<SpriteRenderer>();
            if (sr != null)
                avatar = sr.sprite;

            currentMoveUI.UpdateCurrentMove(playerName, avatar);
            Debug.Log("Updated UI for: " + playerName);
        }
        else
        {
            Debug.LogWarning("CurrentMoveUI reference missing or no players available!");
        }
    }
}
