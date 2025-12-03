using UnityEngine;
using UnityEngine.Tilemaps;

public class TurfManager : MonoBehaviour
{
    public Tilemap turfTilemap;
    
    [Header("Settings")]
    public bool startOwnedByEnemies;
    public Color enemyColor = Color.red;

    [Header("Stats")]
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

                if (startOwnedByEnemies)
                {
                    // Unlock the tile so we can change its color
                    turfTilemap.SetTileFlags(pos, TileFlags.None);
                    
                    // Set visual color
                    turfTilemap.SetColor(pos, enemyColor);
                    
                    // Update count
                    enemyTiles++;
                }
            }
        }
    }

    public void RegisterTile(bool isPlayer)
    {
        if (isPlayer)
        {
            ownedTiles++;
            // Optional: If you want a "Zero Sum" game where taking a tile 
            // removes it from the enemy count, uncomment the line below:
            // if (enemyTiles > 0) enemyTiles--; 
        }
        else
        {
            enemyTiles++;
            // if (ownedTiles > 0) ownedTiles--;
        }
    }

    public float GetTurfPercentage()
    {
        if (totalTiles == 0) return 0f;
        return (float)ownedTiles / (float)totalTiles;
    }
}