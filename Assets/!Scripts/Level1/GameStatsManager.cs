using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class PlayerStats
{
    public string playerName;
    public int diceRollCount;
    public float timeSpent;
    public bool isBot;
}

[System.Serializable]
public class GameStats
{
    public List<PlayerStats> playersStats = new List<PlayerStats>();
}

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance;
    public GameStats gameStats = new GameStats();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadExistingStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadExistingStats()
    {
        string path = Path.Combine(Application.persistentDataPath, "gamestats.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameStats loadedStats = JsonUtility.FromJson<GameStats>(json);
            if (loadedStats != null && loadedStats.playersStats != null)
            {
                gameStats.playersStats = loadedStats.playersStats;
            }
        }
    }

    public void AddOrUpdatePlayerStats(string playerName, int diceRollCount, float timeSpent, bool isBot)
    {
        PlayerStats ps = gameStats.playersStats.Find(x => x.playerName == playerName);
        if (ps == null)
        {
            ps = new PlayerStats();
            ps.playerName = playerName;
            ps.diceRollCount = diceRollCount;
            ps.timeSpent = timeSpent;
            ps.isBot = isBot;
            gameStats.playersStats.Add(ps);
        }
        else
        {
            ps.diceRollCount = diceRollCount;
            ps.timeSpent = timeSpent;
            ps.isBot = isBot;
        }
    }

    public void SaveStatsToJson()
    {
        string path = Path.Combine(Application.persistentDataPath, "gamestats.json");
        GameStats existingStats = new GameStats();
        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            GameStats loadedStats = JsonUtility.FromJson<GameStats>(existingJson);
            if (loadedStats != null && loadedStats.playersStats != null)
            {
                existingStats.playersStats = loadedStats.playersStats;
            }
        }
        foreach (PlayerStats newEntry in gameStats.playersStats)
        {
            PlayerStats existingEntry = existingStats.playersStats.Find(x => x.playerName == newEntry.playerName);
            if (existingEntry == null)
            {
                existingStats.playersStats.Add(newEntry);
            }
            else
            {
                existingEntry.diceRollCount = newEntry.diceRollCount;
                existingEntry.timeSpent = newEntry.timeSpent;
                existingEntry.isBot = newEntry.isBot;
            }
        }
        string json = JsonUtility.ToJson(existingStats, true);
        File.WriteAllText(path, json);
        Debug.Log("Game stats saved to: " + path);
    }
}
