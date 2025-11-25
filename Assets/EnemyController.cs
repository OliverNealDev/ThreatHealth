using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{ 
    private NavMeshAgent agent;
    [SerializeField] private GameObject playerObj;
    
    [Header("Turf Settings")]
    [SerializeField] private TurfManager turfManager;
    [SerializeField] private Tilemap turfTilemap;
    [SerializeField] private Color turfColor = Color.green;
    private Vector3Int lastCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        var pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0f);
    }

    void Start()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Enemy is NOT on NavMesh at Start. Check Z position and NavMesh baking.");
        }
    }

    void FixedUpdate()
    {
        PaintTurfUnderEnemy();
    }

    void Update()
    {
        if (agent == null || playerObj == null) return;

        agent.SetDestination(playerObj.transform.position);
        // we need to set destination to the nearest available tile
    }

    void PaintTurfUnderEnemy()
    {
        if (turfTilemap == null) return;

        Vector3Int cellPos = turfTilemap.WorldToCell(transform.position);
        
        if (cellPos == lastCell) return;

        if (!turfTilemap.HasTile(cellPos))
        {
            lastCell = cellPos;
            return;
        }

        if (turfTilemap.GetColor(cellPos) == turfColor)
        {
            lastCell = cellPos;
            return;
        }

        var flags = turfTilemap.GetTileFlags(cellPos);
        if ((flags & TileFlags.LockColor) != 0)
        {
            turfTilemap.SetTileFlags(cellPos, TileFlags.None);
        }

        turfTilemap.SetColor(cellPos, turfColor);

        if (turfManager != null)
        {
            turfManager.RegisterTile(false);
        }

        lastCell = cellPos;
    }
}