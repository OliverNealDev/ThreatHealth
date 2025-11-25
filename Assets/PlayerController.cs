using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 6f;

    private Rigidbody2D rb;
    
    [Header("Combat Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileRange;

    [Header("Dynamic Fire Rate")]
    [Tooltip("Time between shots when you have 0% turf (Slow)")]
    [SerializeField] private float slowFireRate = 0.5f; 
    [Tooltip("Time between shots when you have 100% turf (Fast)")]
    [SerializeField] private float fastFireRate = 0.1f;
    
    private float currentFireDelay; 
    private float timeSinceShot;

    [Header("Turf Settings")]
    [SerializeField] private TurfManager turfManager;
    [SerializeField] private Tilemap turfTilemap;
    [SerializeField] private Color turfColor = Color.green;
    private Vector3Int lastCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

    void Awake()
    { 
        rb = GetComponent<Rigidbody2D>();
        currentFireDelay = slowFireRate;
    }

    void FixedUpdate()
    {
        CheckMovementInputs();
        PaintTurfUnderPlayer();
    }

    void Update()
    {
        TurnToCursor();
        
        CalculateFireRate();

        if (timeSinceShot < currentFireDelay)
        {
            timeSinceShot += Time.deltaTime;
        }

        CheckAttackInputs();
    }

    void CalculateFireRate()
    {
        if (turfManager != null)
        {
            currentFireDelay = Mathf.Lerp(slowFireRate, fastFireRate, turfManager.GetTurfPercentage());
        }
    }

    void CheckMovementInputs()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;

        input = input.normalized * speed;
        rb.linearVelocity = input;
    }

    void CheckAttackInputs()
    {
        if (Mouse.current.leftButton.isPressed && timeSinceShot >= currentFireDelay)
        {
            GameObject projectile = Instantiate(
                projectilePrefab,
                transform.position + transform.right * 0.5f,
                transform.rotation
            );

            /*if (projectilePrefab.TryGetComponent<Rigidbody2D>(out Rigidbody2D projRb))
            {
                projRb.linearVelocity = transform.right * projectileSpeed;
            }*/
            
            projectile.GetComponent<bulletController>().Initialise(
                true,
                projectileSpeed,
                5f,
                1f,
                0f
            );
             
            timeSinceShot = 0f;
        }
    }

    void TurnToCursor()
    {
        var mousePos = Mouse.current.position.ReadValue();
        var screenPos = new Vector3(
            mousePos.x,
            mousePos.y,
            Camera.main.WorldToScreenPoint(transform.position).z
        );

        Vector3 world = Camera.main.ScreenToWorldPoint(screenPos);
        transform.right = Vector3.ProjectOnPlane(world - transform.position, Vector3.forward).normalized;
    }

    void PaintTurfUnderPlayer()
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
            turfManager.RegisterTile(true);
        }

        lastCell = cellPos;
    }
}