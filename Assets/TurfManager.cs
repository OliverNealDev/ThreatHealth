using UnityEngine;
using UnityEngine.Tilemaps;

public class TurfManager : MonoBehaviour
{
    public Tilemap turfTilemap;
    public int totalTiles = 0;
    public int ownedTiles = 0;
    public int enemyTiles = 0;

    void Start()
    {
        turfTilemap.CompressBounds();
        foreach (var pos in turfTilemap.cellBounds.allPositionsWithin)
        {
            if (turfTilemap.HasTile(pos))
            {
                totalTiles++;
            }
        }
    }

    public void RegisterTile(bool isPlayer)
    {
        if (isPlayer)
        {
            ownedTiles++;
        }
        else
        {
            enemyTiles++;
        }
    }

    public float GetTurfPercentage()
    {
        if (totalTiles == 0) return 0f;
        return (float)ownedTiles / (float)totalTiles;
    }
}