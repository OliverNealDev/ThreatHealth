using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 6f;

    private Rigidbody2D rb;
    private Vector2 move;
    
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileRange;
    [SerializeField] private float fireRate;
    private float timeSinceShot;

    [SerializeField] private Tilemap turfTilemap;
    [SerializeField] private Color turfColor = Color.green;
    private Vector3Int lastCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

    void Awake()
    { 
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        CheckMovementInputs();
        CheckAttackInputs();
        PaintTurfUnderPlayer();
    }

    void Update()
    {
        TurnToCursor();
        if (timeSinceShot < fireRate)
        {
            timeSinceShot += Time.deltaTime;
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
        if (Mouse.current.leftButton.isPressed && timeSinceShot >= fireRate)
        {
            GameObject projectile = Instantiate(
                projectilePrefab,
                transform.position + transform.right * 0.5f,
                Quaternion.identity
            );

            projectile.GetComponent<Rigidbody2D>().linearVelocity = transform.right * projectileSpeed;
            
            Destroy(projectile, projectileRange / projectileSpeed);
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

        if (turfTilemap.GetTile(cellPos) == null)
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
        lastCell = cellPos;
    }
}
