using UnityEngine;
using UnityEngine.Tilemaps;

public class TurfManager : MonoBehaviour
{
    public Tilemap turfTilemap;
    public int totalTiles = 0;
    public int ownedTiles = 0;

    void Start()
    {
        // Count all tiles in the map to establish the "100%" baseline
        turfTilemap.CompressBounds(); // Ensures bounds are tight to the drawn tiles
        foreach (var pos in turfTilemap.cellBounds.allPositionsWithin)
        {
            if (turfTilemap.HasTile(pos))
            {
                totalTiles++;
            }
        }
    }

    public void RegisterTile()
    {
        ownedTiles++;
    }

    public float GetTurfPercentage()
    {
        if (totalTiles == 0) return 0f;
        return (float)ownedTiles / (float)totalTiles;
    }
}