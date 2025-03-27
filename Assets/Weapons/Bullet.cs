using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public float damage = 1;

    void Start()
    {
        // Ustaw pr�dko�� pocisku
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * bulletSpeed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                
            }
            Destroy(gameObject);
        }
        if (other.tag == "TilemapCollider")
        {
            Destroy(gameObject);
        }
        
    }
}
