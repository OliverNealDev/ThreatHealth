using UnityEngine;
using UnityEngine.InputSystem;

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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        CheckMovementInputs();
        CheckAttackInputs();
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
        if (Input.GetKey(KeyCode.W))
        {
            rb.linearVelocity += new Vector2(0f, speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.linearVelocity += new Vector2(0f, -speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.linearVelocity += new Vector2(-speed, 0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.linearVelocity += new Vector2(speed, 0f);
        }
    }

    void CheckAttackInputs()
    {
        if (Input.GetMouseButton(0) && timeSinceShot >= fireRate)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.right * 0.5f, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().linearVelocity = transform.right * projectileSpeed;
            
            Destroy(projectile, projectileRange / projectileSpeed);
            timeSinceShot = 0f;
        }
    }

    void TurnToCursor()
    {
        transform.right = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, Camera.main.WorldToScreenPoint(transform.position).z)) - transform.position, Vector3.forward).normalized;
    }
}