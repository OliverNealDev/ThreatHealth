using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TurfManager : MonoBehaviour
{
    [Header("References")]
    public Tilemap turfTilemap;
    [SerializeField] private Slider turfSlider;
    
    [Header("Settings")]
    public bool startOwnedByEnemies;
    public Color playerColor = Color.green;
    public Color enemyColor = Color.red;

    [Header("Stats")]
    public int totalTiles = 0;
    public int ownedTiles = 0;
    public int enemyTiles = 0;

    void Start()
    {
        RecalculateTotalTiles();
    }

    void RecalculateTotalTiles()
    {
        turfTilemap.CompressBounds();
        totalTiles = 0;
        ownedTiles = 0;
        enemyTiles = 0;

        foreach (var pos in turfTilemap.cellBounds.allPositionsWithin)
        {
            if (turfTilemap.HasTile(pos))
            {
                totalTiles++;
                
                // Unlock tile to ensure we can color it later
                turfTilemap.SetTileFlags(pos, TileFlags.None);

                if (startOwnedByEnemies)
                {
                    turfTilemap.SetColor(pos, enemyColor);
                    enemyTiles++;
                }
                else
                {
                    // If not starting as enemy, check if it's already painted in editor
                    Color c = turfTilemap.GetColor(pos);
                    if (IsColorSimilar(c, playerColor)) ownedTiles++;
                    else if (IsColorSimilar(c, enemyColor)) enemyTiles++;
                }
            }
        }
        
        UpdateSlider();
    }

    public void RegisterTile(Vector3Int cellPos, bool isPlayer)
    {
        if (!turfTilemap.HasTile(cellPos)) return;

        Color currentColor = turfTilemap.GetColor(cellPos);
        Color targetColor = isPlayer ? playerColor : enemyColor;

        // If tile is already the correct color, do nothing
        if (IsColorSimilar(currentColor, targetColor)) return;

        // Logic for taking tiles
        if (isPlayer)
        {
            // Player is capturing
            ownedTiles++;
            
            // If we stole it from the enemy, reduce their count
            if (IsColorSimilar(currentColor, enemyColor))
            {
                enemyTiles--;
            }
        }
        else
        {
            // Enemy is capturing
            enemyTiles++;

            // If they stole it from player, reduce player count
            if (IsColorSimilar(currentColor, playerColor))
            {
                ownedTiles--;
            }
        }

        // Apply the visual change
        turfTilemap.SetColor(cellPos, targetColor);
        
        UpdateSlider();
    }

    void UpdateSlider()
    {
        if (turfSlider != null)
        {
            turfSlider.value = GetTurfPercentage();
        }
    }

    public float GetTurfPercentage()
    {
        if (totalTiles == 0) return 0f;
        return (float)ownedTiles / (float)totalTiles;
    }

    bool IsColorSimilar(Color a, Color b)
    {
        float tolerance = 0.01f;
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}