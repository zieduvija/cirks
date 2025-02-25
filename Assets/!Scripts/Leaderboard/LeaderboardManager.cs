using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject headerRowPrefab;
    public GameObject leaderboardEntryPrefab;
    public Button sortByThrowsButton;
    public Button sortByTimeButton;
    public TMP_Text specialtyStatsText;
    public TMP_Text winner1Text;
    public TMP_Text winner2Text;
    public TMP_Text winner3Text;

    private List<PlayerStats> entries;

    void Start()
    {
        sortByThrowsButton.onClick.AddListener(SortByThrows);
        sortByTimeButton.onClick.AddListener(SortByTime);
        LoadStats();
        PopulateLeaderboard();
        UpdateSpecialtyStats();
        UpdateTopWinners();
    }

    void LoadStats()
    {
        string path = Path.Combine(Application.persistentDataPath, "gamestats.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameStats stats = JsonUtility.FromJson<GameStats>(json);
            entries = stats.playersStats;
        }
        else
        {
            entries = new List<PlayerStats>();
        }
    }

    void PopulateLeaderboard()
    {
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);
        if (headerRowPrefab != null)
            Instantiate(headerRowPrefab, contentPanel);
        foreach (PlayerStats entry in entries)
        {
            GameObject entryGO = Instantiate(leaderboardEntryPrefab, contentPanel);
            LeaderboardEntryUI entryUI = entryGO.GetComponent<LeaderboardEntryUI>();
            if (entryUI != null)
                entryUI.Setup(entry.playerName, entry.diceRollCount, entry.timeSpent, entry.isBot);
        }
    }

    void SortByThrows()
    {
        entries = entries.OrderBy(e => e.diceRollCount).ThenBy(e => e.timeSpent).ToList();
        PopulateLeaderboard();
        UpdateSpecialtyStats();
        UpdateTopWinners();
    }

    void SortByTime()
    {
        entries = entries.OrderBy(e => e.timeSpent).ThenBy(e => e.diceRollCount).ToList();
        PopulateLeaderboard();
        UpdateSpecialtyStats();
        UpdateTopWinners();
    }

    void UpdateSpecialtyStats()
    {
        if (entries == null || entries.Count == 0)
        {
            specialtyStatsText.text = "No data available.";
            return;
        }
        PlayerStats leastRolls = entries.OrderBy(e => e.diceRollCount).First();
        PlayerStats bestTime = entries.OrderBy(e => e.timeSpent).First();
        PlayerStats slowestTime = entries.OrderByDescending(e => e.timeSpent).First();
        PlayerStats mostRolls = entries.OrderByDescending(e => e.diceRollCount).First();
        specialtyStatsText.text = "Least Rolls: " + leastRolls.playerName + " (" + leastRolls.diceRollCount + " rolls)\n" +
                                    "Best Time: " + bestTime.playerName + " (" + bestTime.timeSpent.ToString("F2") + " sec)\n\n" +
                                    "Slowest Time: " + slowestTime.playerName + " (" + slowestTime.timeSpent.ToString("F2") + " sec)\n" +
                                    "Most Rolls: " + mostRolls.playerName + " (" + mostRolls.diceRollCount + " rolls)";
    }

    void UpdateTopWinners()
    {
        if (entries == null || entries.Count == 0)
        {
            winner1Text.text = "N/A";
            winner2Text.text = "N/A";
            winner3Text.text = "N/A";
            return;
        }
        List<PlayerStats> sorted = entries.OrderBy(e => e.timeSpent).ToList();
        winner1Text.text = sorted.Count > 0 ? sorted[0].playerName : "N/A";
        winner2Text.text = sorted.Count > 1 ? sorted[1].playerName : "N/A";
        winner3Text.text = sorted.Count > 2 ? sorted[2].playerName : "N/A";
    }
}
