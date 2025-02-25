using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentMoveUI : MonoBehaviour
{
    public TMP_Text currentMoveText;  // Displays current player's name.
    public Image playerAvatarImage;   // Displays current player's avatar.

    public void UpdateCurrentMove(string playerName, Sprite avatarSprite)
    {
        if (currentMoveText != null)
            currentMoveText.text = playerName;
        if (playerAvatarImage != null)
            playerAvatarImage.sprite = avatarSprite;
    }
}
