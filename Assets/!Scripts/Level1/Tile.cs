using UnityEngine;

public class Tile : MonoBehaviour
{
    // Default effect: move the player +1 tile forward.
    public int effectValue = 1;
    // If not -1, landing on this tile sends the player directly to this tile index.
    public int moveToTile = -1;
}
