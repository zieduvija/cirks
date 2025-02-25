using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardManager : MonoBehaviour
{
    public bool useManualTiles = true;
    public int totalTiles { get; private set; }
    public Vector3[] tilePositions { get; private set; }
    public Tile[] tiles { get; private set; }

    public Tile tilePrefab;
    public int rowCount = 5;
    public int colCount = 6;
    public Vector3 startPosition = new Vector3(0, 0, 0);
    public Vector3 tileSpacing = new Vector3(3f, 0, 3f);

    public float dropHeight = 10f;
    public float dropDuration = 1f;

    public float runFraction = 0.8f;
    public float runSpeed = 3f;
    public float jumpDuration = 0.5f;
    public float jumpHeight = 2f;

    public float jumpPauseMin = 0.2f;
    public float jumpPauseMax = 0.6f;

    public float playerYOffset = 0.5f;

    void Awake()
    {
        if (useManualTiles)
            CollectTiles();
    }

    void CollectTiles()
    {
        tiles = GetComponentsInChildren<Tile>();
        System.Array.Sort(tiles, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        totalTiles = tiles.Length;
        tilePositions = new Vector3[totalTiles];
        for (int i = 0; i < totalTiles; i++)
            tilePositions[i] = tiles[i].transform.position;
        Debug.Log("Collected " + totalTiles + " manually placed tiles.");
    }

    public IEnumerator RunAndJumpPlayerToTile(GameObject player, int targetTileIndex)
    {
        Animator animator = player.GetComponent<Animator>();
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        if (tilePositions == null || tilePositions.Length == 0)
            yield break;
        targetTileIndex = Mathf.Clamp(targetTileIndex, 0, tilePositions.Length - 1);
        Vector3 startPos = player.transform.position;
        Vector3 targetPos = tilePositions[targetTileIndex] + Vector3.up * playerYOffset;
        Vector3 delta = targetPos - startPos;
        float distance = delta.magnitude;
        if (distance < 0.01f)
            yield break;
        if (spriteRenderer != null)
            spriteRenderer.flipX = (targetPos.x < startPos.x);
        if (animator != null)
            animator.SetBool("moving", true);
        Vector3 runTarget = startPos + delta.normalized * (distance * runFraction);
        while (Vector3.Distance(player.transform.position, runTarget) > 0.05f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, runTarget, runSpeed * Time.deltaTime);
            yield return null;
        }
        player.transform.position = runTarget;
        if (animator != null)
        {
            animator.SetBool("moving", false);
            animator.Play("Jump", 0, 0f);
            animator.SetBool("jumping", true);
        }
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / jumpDuration;
            t = Mathf.Clamp01(t);
            Vector3 horizontal = Vector3.Lerp(runTarget, targetPos, t);
            float vertical = 4 * jumpHeight * t * (1 - t);
            player.transform.position = horizontal + Vector3.up * vertical;
            yield return null;
        }
        player.transform.position = targetPos;
        if (animator != null)
            animator.SetBool("jumping", false);
        Debug.Log(player.name + " reached tile " + targetTileIndex + " at position " + player.transform.position);
        float pauseDuration = Random.Range(jumpPauseMin, jumpPauseMax);
        yield return new WaitForSeconds(pauseDuration);
    }

    public IEnumerator RunAndJumpPlayerThroughTiles(GameObject player, int startTile, int endTile)
    {
        if (startTile == endTile)
            yield break;
        int step = (endTile > startTile) ? 1 : -1;
        for (int i = startTile + step; (step > 0 ? i <= endTile : i >= endTile); i += step)
            yield return StartCoroutine(RunAndJumpPlayerToTile(player, i));
    }

    public int GetTileDelta(int tileIndex)
    {
        if (tiles != null && tileIndex >= 0 && tileIndex < tiles.Length)
            return tiles[tileIndex].effectValue;
        return 0;
    }
}
