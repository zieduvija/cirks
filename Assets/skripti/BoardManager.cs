using UnityEngine;
using System.Collections;


public class BoardManager : MonoBehaviour
{
    // Instead of separate arrays, we only use one array of Tile components.
    public Tile[] tiles;

    /// <summary>
    /// Smoothly moves a player GameObject to the tile at targetTileIndex.
    /// </summary>
    public IEnumerator MovePlayerToTile(GameObject player, int targetTileIndex, float moveSpeed = 5f)
    {
        if (tiles == null || tiles.Length == 0)
        {
            Debug.LogError("Tiles not set in BoardManager.");
            yield break;
        }
        
        // Clamp targetTileIndex to a valid range.
        targetTileIndex = Mathf.Clamp(targetTileIndex, 0, tiles.Length - 1);
        Vector3 targetPos = tiles[targetTileIndex].transform.position;
        while (Vector3.Distance(player.transform.position, targetPos) > 0.01f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        player.transform.position = targetPos;
    }

    /// <summary>
    /// Returns the override tile number if the tile at tileIndex has one.
    /// Returns -1 if there is no override.
    /// </summary>
    public int GetTileOverride(int tileIndex)
    {
        if (tiles != null && tileIndex >= 0 && tileIndex < tiles.Length)
        {
            if (tiles[tileIndex].moveToTile != -1)
                return tiles[tileIndex].moveToTile;
        }
        return -1;
    }

    /// <summary>
    /// Returns the effect delta for the tile at tileIndex.
    /// </summary>
    public int GetTileDelta(int tileIndex)
    {
        if (tiles != null && tileIndex >= 0 && tileIndex < tiles.Length)
        {
            return tiles[tileIndex].effectValue;
        }
        return 0;
    }
}
