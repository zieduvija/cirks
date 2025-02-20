using UnityEngine;

public class Tile : MonoBehaviour
{
    // By default, landing on this tile moves you +1 tile.
    public int effectValue = 1;
    // Special override: if set to a value other than -1, the player will be sent
    // directly to that tile number instead of adding effectValue.
    public int moveToTile = -1;
}
