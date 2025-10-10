using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 6f;

    private Rigidbody2D rb;
    private Vector2 move;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        CheckMovementInputs();
    }

    void Update()
    {
        TurnToCursor();
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

    void TurnToCursor()
    {
        transform.right = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
    }
}