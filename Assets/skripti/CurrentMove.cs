using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentMove : MonoBehaviour
{
    [Header("UI Components")]
    // Assign your TextMeshPro text component in the Inspector.
    public TMP_Text currentMoveText; 
    // Assign your Image component (to display the player's avatar) in the Inspector.
    public Image playerAvatarImage;

    /// <summary>
    /// Updates the UI with the given player's name and avatar.
    /// </summary>
    /// <param name="playerName">The current player's name.</param>
    /// <param name="avatarSprite">The current player's avatar sprite.</param>
    public void UpdateCurrentMove(string playerName, Sprite avatarSprite)
    {
        if (currentMoveText != null)
            currentMoveText.text = playerName;
        
        if (playerAvatarImage != null)
            playerAvatarImage.sprite = avatarSprite;
    }
}
