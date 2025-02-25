using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CirksGameManager : MonoBehaviour
{
    public CurrentMoveUI currentMoveUI;
    public TMP_Text diceResultText;
    public TMP_Text messageText;
    public TMP_Text winningMessageText;
    public GameObject winningScreenUI;
    public DiceRoller diceRoller;
    public BoardManager boardManager;
    public DiceSpawner diceSpawner;
    public CameraFollow cameraFollow;
    public int humanPlayerIndex = 0;
    private List<NameScript> players = new List<NameScript>();
    private int[] playerTileIndices;
    private int[] diceRollCounts;
    private int currentPlayerIndex = 0;
    private bool gameStarted = false;
    private bool turnInProgress = false;
    private bool winningScreenShown = false;
    private float gameStartTime;

    IEnumerator Start()
    {
        PlayerPrefs.SetString("previousScene", "Level 1");
        gameStartTime = Time.time;
        while (FindObjectsOfType<NameScript>().Length == 0)
            yield return null;
        NameScript[] foundPlayers = FindObjectsOfType<NameScript>();
        players.AddRange(foundPlayers);
        string humanName = PlayerPrefs.GetString("PlayerName", "");
        if (!string.IsNullOrEmpty(humanName))
        {
            NameScript humanPlayer = players.Find(p => p.GetPlayerName() == humanName);
            if (humanPlayer != null)
            {
                players.Remove(humanPlayer);
                players.Insert(0, humanPlayer);
                humanPlayerIndex = 0;
            }
        }
        playerTileIndices = new int[players.Count];
        diceRollCounts = new int[players.Count];
        for (int i = 0; i < players.Count; i++)
        {
            playerTileIndices[i] = 0;
            diceRollCounts[i] = 0;
        }
        yield return StartCoroutine(ShowBoardOverview());
        UpdateCurrentMoveUI();
        messageText.text = "Current player: " + players[currentPlayerIndex].GetPlayerName() + ". Roll the dice to start (roll 1 or 6).";
        if (cameraFollow != null)
            cameraFollow.target = players[currentPlayerIndex].transform;
        if (diceRoller == null)
        {
            GameObject diceObj = GameObject.Find("Dice");
            if (diceObj != null)
                diceRoller = diceObj.GetComponent<DiceRoller>();
        }
        if (diceRoller != null && diceRoller.resultText == null)
        {
            TMP_Text sceneTMP = GameObject.Find("DiceResultText")?.GetComponent<TMP_Text>();
            if (sceneTMP != null)
                diceRoller.resultText = sceneTMP;
        }
        if (diceRoller != null)
            diceRoller.OnDiceRollFinished += HandleDiceRollFinished;
    }

    IEnumerator ShowBoardOverview()
    {
        if (cameraFollow != null)
        {
            float originalHeight = cameraFollow.heightOffset;
            cameraFollow.heightOffset = 5f;
            cameraFollow.target = boardManager.transform;
            messageText.text = "Board Overview";
            yield return new WaitForSeconds(3f);
            cameraFollow.heightOffset = originalHeight;
            cameraFollow.target = players[currentPlayerIndex].transform;
        }
        yield break;
    }

    IEnumerator ProcessPlayerMove(int result)
    {
        turnInProgress = true;
        diceRollCounts[currentPlayerIndex]++;
        if (!gameStarted)
        {
            if (result == 1 || result == 6)
            {
                int startingTile = 6;
                yield return StartCoroutine(boardManager.RunAndJumpPlayerThroughTiles(players[currentPlayerIndex].gameObject, playerTileIndices[currentPlayerIndex], startingTile));
                playerTileIndices[currentPlayerIndex] = startingTile;
                messageText.text = players[currentPlayerIndex].GetPlayerName() + " has started and advanced to tile " + startingTile + "!";
                gameStarted = true;
            }
            else
            {
                messageText.text = players[currentPlayerIndex].GetPlayerName() + " did not roll a valid start (" + result + ").";
            }
        }
        else
        {
            int currentTile = playerTileIndices[currentPlayerIndex];
            int targetTile = currentTile + result;
            if (targetTile >= boardManager.totalTiles)
                targetTile = boardManager.totalTiles - 1;
            yield return StartCoroutine(boardManager.RunAndJumpPlayerThroughTiles(players[currentPlayerIndex].gameObject, currentTile, targetTile));
            playerTileIndices[currentPlayerIndex] = targetTile;
            if (targetTile == boardManager.totalTiles - 1)
            {
                float finishTime = Time.time - gameStartTime;
                bool isBot = currentPlayerIndex != humanPlayerIndex;
                GameStatsManager.Instance.AddOrUpdatePlayerStats(players[currentPlayerIndex].GetPlayerName(), diceRollCounts[currentPlayerIndex], finishTime, isBot);
                if (!winningScreenShown)
                {
                    winningScreenShown = true;
                    ShowWinningScreen(finishTime, diceRollCounts[currentPlayerIndex], players[currentPlayerIndex].GetPlayerName(), isBot);
                }
            }
            int delta = boardManager.GetTileDelta(targetTile);
            if (delta != 0)
            {
                int newTarget = targetTile + delta;
                newTarget = Mathf.Clamp(newTarget, 0, boardManager.totalTiles - 1);
                yield return StartCoroutine(boardManager.RunAndJumpPlayerThroughTiles(players[currentPlayerIndex].gameObject, targetTile, newTarget));
                playerTileIndices[currentPlayerIndex] = newTarget;
            }
        }
        turnInProgress = false;
        NextTurn();
    }

    void HandleDiceRollFinished(int result)
    {
        diceResultText.text = result.ToString();
        if (!turnInProgress)
            StartCoroutine(ProcessPlayerMove(result));
    }

    void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        UpdateCurrentMoveUI();
        messageText.text = "Current player: " + players[currentPlayerIndex].GetPlayerName() + ". Roll the dice.";
        if (cameraFollow != null)
            cameraFollow.target = players[currentPlayerIndex].transform;
        if (currentPlayerIndex != humanPlayerIndex && !turnInProgress)
            StartCoroutine(SimulateComputerTurn());
    }

    IEnumerator SimulateComputerTurn()
    {
        yield return new WaitForSeconds(2f);
        int simulatedResult = Random.Range(1, 7);
        if (diceRoller != null)
            diceRoller.SimulateRoll();
        yield return new WaitForSeconds(4f);
        HandleDiceRollFinished(simulatedResult);
    }

    void UpdateCurrentMoveUI()
    {
        if (currentMoveUI != null && players.Count > 0)
        {
            NameScript currentPlayer = players[currentPlayerIndex];
            string playerName = currentPlayer.GetPlayerName();
            Sprite avatar = currentPlayer.GetPlayerSprite();
            currentMoveUI.UpdateCurrentMove(playerName, avatar);
            if (diceSpawner != null)
            {
                diceSpawner.SpawnDiceAbovePlayer(currentPlayer.transform);
                GameObject spawnedDice = diceSpawner.CurrentDice;
                if (spawnedDice != null)
                {
                    DiceRoller newDiceRoller = spawnedDice.GetComponent<DiceRoller>();
                    if (newDiceRoller != null)
                    {
                        TMP_Text sceneTMP = GameObject.Find("DiceResultText")?.GetComponent<TMP_Text>();
                        if (sceneTMP != null)
                            newDiceRoller.resultText = sceneTMP;
                        if (currentPlayerIndex == humanPlayerIndex)
                            newDiceRoller.SetInteractable(true);
                        else
                            newDiceRoller.SetInteractable(false);
                        if (diceRoller != null)
                            diceRoller.OnDiceRollFinished -= HandleDiceRollFinished;
                        newDiceRoller.OnDiceRollFinished += HandleDiceRollFinished;
                        diceRoller = newDiceRoller;
                    }
                }
            }
        }
    }

    void ShowWinningScreen(float finishTime, int rolls, string winnerName, bool isBot)
    {
        Time.timeScale = 0f;
        if (winningScreenUI != null && winningMessageText != null)
        {
            winningScreenUI.SetActive(true);
            string botText = isBot ? " (BOT)" : "";
            winningMessageText.text = "Winner: " + winnerName + botText + "\nRolls: " + rolls + "\nTime: " + finishTime.ToString("F2") + " seconds";
        }
    }

    public void ContinueWatching()
    {
        Time.timeScale = 1f;
        winningScreenUI.SetActive(false);
    }

    public void GoToLeaderboard()
    {
        GameStatsManager.Instance.SaveStatsToJson();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Leaderboard");
    }

    public void ExitToMainMenu()
    {
        GameStatsManager.Instance.SaveStatsToJson();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
