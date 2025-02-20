using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    // Data class for a leaderboard entry.
    [System.Serializable]
    public class LeaderboardEntry {
        public string name;
        public int throwCount;
        public float time; // In seconds.
    }

    [Header("UI References")]
    [Tooltip("The Content object of the Scroll View where leaderboard entries will be instantiated.")]
    public Transform contentPanel;
    
    [Tooltip("The prefab for the header row that displays column titles.")]
    public GameObject headerRowPrefab;

    [Tooltip("The prefab for a single leaderboard entry row. It should have a LeaderboardEntryUI component attached.")]
    public GameObject leaderboardEntryPrefab;
    
    [Tooltip("Button to sort entries by throw count (with time as a tiebreaker).")]
    public Button sortByThrowsButton;
    
    [Tooltip("Button to sort entries by time (with throw count as a tiebreaker).")]
    public Button sortByTimeButton;
    
    [Tooltip("TextMeshPro text object to display specialty stats.")]
    public TMP_Text specialtyStatsText;

    [Header("Top Winners Text Fields")]
    [Tooltip("Text field for the top winner.")]
    public TMP_Text winner1Text;
    
    [Tooltip("Text field for the second winner.")]
    public TMP_Text winner2Text;
    
    [Tooltip("Text field for the third winner.")]
    public TMP_Text winner3Text;

    // The list of leaderboard entries.
    private List<LeaderboardEntry> entries;

    void Start()
    {
        // For demonstration, create some sample data.
        entries = new List<LeaderboardEntry>()
        {
            new LeaderboardEntry() { name = "Alice", throwCount = 10, time = 120f },
            new LeaderboardEntry() { name = "Bob", throwCount = 12, time = 110f },
            new LeaderboardEntry() { name = "Charlie", throwCount = 8, time = 130f },
            new LeaderboardEntry() { name = "Daisy", throwCount = 12, time = 115f }
        };

        // Set up sort button listeners.
        sortByThrowsButton.onClick.AddListener(SortByThrows);
        sortByTimeButton.onClick.AddListener(SortByTime);

        // Populate the leaderboard UI, update specialty stats, and update top winners.
        PopulateLeaderboard();
        UpdateSpecialtyStats();
        UpdateTopWinners();
    }

    /// <summary>
    /// Clears the current UI entries and populates the content panel with a header row followed by rows for each leaderboard entry.
    /// </summary>
    void PopulateLeaderboard()
    {
        // Clear any existing children.
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Instantiate the header row as the first entry.
        if (headerRowPrefab != null)
        {
            Instantiate(headerRowPrefab, contentPanel);
        }
        else
        {
            Debug.LogWarning("Header Row Prefab is not assigned!");
        }

        // Instantiate a row for each leaderboard entry.
        foreach (LeaderboardEntry entry in entries)
        {
            GameObject entryGO = Instantiate(leaderboardEntryPrefab, contentPanel);
            LeaderboardEntryUI entryUI = entryGO.GetComponent<LeaderboardEntryUI>();
            if (entryUI != null)
            {
                entryUI.Setup(entry);
            }
        }
    }

    /// <summary>
    /// Sorts the leaderboard entries by throw count (ascending) and then by time (ascending),
    /// repopulates the UI, and updates specialty stats and top winners.
    /// </summary>
    void SortByThrows()
    {
        entries = entries
                    .OrderBy(e => e.throwCount)
                    .ThenBy(e => e.time)
                    .ToList();
        PopulateLeaderboard();
        UpdateSpecialtyStats();
        UpdateTopWinners();
    }

    /// <summary>
    /// Sorts the leaderboard entries by time (ascending) and then by throw count (ascending),
    /// repopulates the UI, and updates specialty stats and top winners.
    /// </summary>
    void SortByTime()
    {
        entries = entries
                    .OrderBy(e => e.time)
                    .ThenBy(e => e.throwCount)
                    .ToList();
        PopulateLeaderboard();
        UpdateSpecialtyStats();
        UpdateTopWinners();
    }

    /// <summary>
    /// Calculates specialty stats (least rolls, best time, slowest time, and most rolls) and outputs them to the specialtyStatsText.
    /// </summary>
    void UpdateSpecialtyStats()
    {
        if (entries == null || entries.Count == 0)
        {
            specialtyStatsText.text = "No data available.";
            return;
        }

        // Calculate stats.
        LeaderboardEntry leastRolls = entries.OrderBy(e => e.throwCount).First();
        LeaderboardEntry bestTime = entries.OrderBy(e => e.time).First();
        LeaderboardEntry slowestTime = entries.OrderByDescending(e => e.time).First();
        LeaderboardEntry mostRolls = entries.OrderByDescending(e => e.throwCount).First();

        // Build the output string.
        string output = $"Least Rolls: {leastRolls.name} ({leastRolls.throwCount} rolls)\n" +
                        $"Best Time: {bestTime.name} ({bestTime.time:F2} sec)\n\n" +
                        $"Slowest Time: {slowestTime.name} ({slowestTime.time:F2} sec)\n" +
                        $"Most Rolls: {mostRolls.name} ({mostRolls.throwCount} rolls)";
        specialtyStatsText.text = output;
    }

    /// <summary>
    /// Updates the top winners text fields with the names of the top three entries.
    /// In this example, "top" is determined by best (lowest) time.
    /// </summary>
    void UpdateTopWinners()
    {
        if (entries == null || entries.Count == 0)
        {
            winner1Text.text = "N/A";
            winner2Text.text = "N/A";
            winner3Text.text = "N/A";
            return;
        }
        
        // Sort entries by time ascending.
        List<LeaderboardEntry> sorted = entries.OrderBy(e => e.time).ToList();
        winner1Text.text = sorted.Count > 0 ? sorted[0].name : "N/A";
        winner2Text.text = sorted.Count > 1 ? sorted[1].name : "N/A";
        winner3Text.text = sorted.Count > 2 ? sorted[2].name : "N/A";
    }
}
