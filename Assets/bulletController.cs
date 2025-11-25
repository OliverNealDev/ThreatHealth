using UnityEngine;

public class bulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private bool isPlayerBullet;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float size = 1f;
    [SerializeField] private float stunDuration = 0f;
    
        
    
    void Start()
    {
        Destroy(gameObject, 1);
    }

    public void Initialise(
        bool SetIsPlayerBullet, 
        float SetSpeed, 
        float SetKnockbackForce, 
        float SetSize, 
        float SetStunDuration)
    {
        isPlayerBullet = SetIsPlayerBullet;
        speed = SetSpeed;
        knockbackForce = SetKnockbackForce;
        size = SetSize;
        stunDuration = SetStunDuration;
        
        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D projRb))
        {
            projRb.linearVelocity = transform.right * speed;
        }
        
        if (isPlayerBullet)
        {
            gameObject.tag = "PlayerBullet";
            GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        else
        {
            gameObject.tag = "EnemyBullet";
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
