using UnityEngine;
using TMPro;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text throwCountText;
    public TMP_Text timeText;

    public void Setup(string playerName, int diceRollCount, float timeSpent, bool isBot)
    {
        nameText.text = isBot ? playerName + " (BOT)" : playerName;
        throwCountText.text = diceRollCount.ToString();
        timeText.text = timeSpent.ToString("F2");
    }
}
