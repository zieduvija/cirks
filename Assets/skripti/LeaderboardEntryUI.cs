using UnityEngine;
using TMPro;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text throwCountText;
    public TMP_Text timeText;

    public void Setup(LeaderboardManager.LeaderboardEntry entry)
    {
        nameText.text = entry.name;
        throwCountText.text = entry.throwCount.ToString();
        timeText.text = entry.time.ToString("F2");
    }
}
