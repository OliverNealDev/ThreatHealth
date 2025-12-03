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
    }

    void FixedUpdate()
    {
        PaintTurfUnderEnemy();
    }

    void Update()
    {
        if (agent == null || playerObj == null) return;
        
        if (transform.localScale == Vector3.one)
        {
            if (isDisabled)
            {
                isDisabled = false;
                GetComponent<SpriteRenderer>().color = enemyColor;
            }
            
            agent.SetDestination(playerObj.transform.position);
        }
        else
        {
            if (!isDisabled)
            {
                isDisabled = true;
                GetComponent<SpriteRenderer>().color = disabledColor;
            }
            
            transform.localScale += Vector3.one * Time.deltaTime * 0.1f;
            if (transform.localScale.x >= 1f)
            {
                transform.localScale = Vector3.one;
            }
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