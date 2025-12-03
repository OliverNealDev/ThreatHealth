using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{ 
    private NavMeshAgent agent;
    [SerializeField] private GameObject playerObj;
    
    [Header("Turf Settings")]
    [SerializeField] private TurfManager turfManager;
    [SerializeField] private Tilemap turfTilemap;
    [SerializeField] private Color enemyColor;
    [SerializeField] private Color disabledColor;
    private bool isDisabled = false;
    private Vector3Int lastCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    
    private Vector2 randomNearbyPoint;
    private bool isChasing;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        var pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0f);
        
        // It is often safer to assign these in Inspector, but Find is okay for simple scenes
        if (playerObj == null) playerObj = GameObject.Find("Player");
        if (turfManager == null) turfManager = FindAnyObjectByType<TurfManager>();
        if (turfTilemap == null) turfTilemap = GameObject.Find("TurfTilemap")?.GetComponent<Tilemap>();
        
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); 
    }

    void Start()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Enemy is NOT on NavMesh at Start. Check Z position and NavMesh baking.");
        }
        
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(2f, 5f);
        randomNearbyPoint = (Vector2)transform.position + randomDirection;
    }

    void FixedUpdate()
    {
        PaintTurfUnderEnemy();
    }

    void Update()
    {
        if (agent == null || playerObj == null) return;
        
        // --- Growing Phase ---
        if (transform.localScale.x < 1f)
        {
            if (!isDisabled)
            {
                isDisabled = true;
                GetComponent<SpriteRenderer>().color = disabledColor;
            }
            
            // Stop moving while growing
            if (agent.hasPath) agent.ResetPath(); 

            transform.localScale += Vector3.one * Time.deltaTime * 0.1f;
            if (transform.localScale.x >= 1f) transform.localScale = Vector3.one;
            
            return; // Exit Update so we don't run movement logic while growing
        }

        // --- Active Phase ---
        if (isDisabled)
        {
            isDisabled = false;
            GetComponent<SpriteRenderer>().color = enemyColor;
        }
            
        float distanceToPlayer = Vector2.Distance(transform.position, playerObj.transform.position);

        // 1. Chase Player
        if (distanceToPlayer <= 5f)
        {
            agent.SetDestination(playerObj.transform.position);
        }
        // 2. Roam Randomly
        else
        {
            // Check if we reached the destination OR if we have no path
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                SetNewRandomDestination();
            }
        }
    }

    // Helper method to find a VALID point on the NavMesh
    void SetNewRandomDestination()
    {
        Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(2f, 5f);
        Vector3 targetPos = transform.position + new Vector3(randomDir.x, randomDir.y, 0);

        NavMeshHit hit;
        // SamplePosition checks if the point is actually walkable. 
        // If the random point is in a wall, this finds the closest floor point.
        if (NavMesh.SamplePosition(targetPos, out hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void PaintTurfUnderEnemy()
    {
        if (turfTilemap == null || turfManager == null) return;

        Vector3Int cellPos = turfTilemap.WorldToCell(transform.position);
        
        if (cellPos == lastCell) return;

        // Delegate all logic to the Manager (Painting + Scoring)
        // Pass 'false' because this is an Enemy
        turfManager.RegisterTile(cellPos, false);

        lastCell = cellPos;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            
            transform.localScale -= Vector3.one * 0.2f;

            if (transform.localScale.x <= 0.25f)
            {
                Destroy(gameObject);
            }
        }
    }
}