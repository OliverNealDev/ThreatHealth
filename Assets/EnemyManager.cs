using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyManager : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private int initialEnemyCount = 0;
    [SerializeField] private float spawnInterval = 5f;
    private float timeSinceLastSpawn = 0f;
    
    [SerializeField] private GameObject enemyPrefab;

    [Header("Turf Settings")]
    [SerializeField] private Tilemap turfTilemap; 
    [SerializeField] private Color enemyTurfColor = Color.white; 

    void Start()
    {
        for(int i = 0; i < initialEnemyCount; i++)
        {
            SpawnEnemy(true);
        }
    }
    
    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnEnemy(false);
            timeSinceLastSpawn = 0f;
        }
    }
    
    void SpawnEnemy(bool spawnAnywhere)
    {
        if (enemyPrefab == null || turfTilemap == null) return;

        List<Vector3> validSpawnPoints = GetValidSpawnPoints(spawnAnywhere);

        if (validSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPoints.Count);
            Vector3 spawnPosition = validSpawnPoints[randomIndex];
            
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    List<Vector3> GetValidSpawnPoints(bool spawnAnywhere)
    {
        List<Vector3> points = new List<Vector3>();
        
        turfTilemap.CompressBounds();

        foreach (var pos in turfTilemap.cellBounds.allPositionsWithin)
        {
            if (!turfTilemap.HasTile(pos)) continue;

            if (spawnAnywhere)
            {
                points.Add(turfTilemap.GetCellCenterWorld(pos));
            }
            else
            {
                Color tileColor = turfTilemap.GetColor(pos);

                if (tileColor == enemyTurfColor)
                {
                    Vector3 worldPos = turfTilemap.GetCellCenterWorld(pos);
                    points.Add(worldPos);
                }
            }
        }

        return points;
    }
}