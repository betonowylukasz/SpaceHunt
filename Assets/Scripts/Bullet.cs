using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public float damage = 1;

    private bool hasHitEnemy = false;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * bulletSpeed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitEnemy) return;

        if (other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                print("1 damage");
            }
            hasHitEnemy = true;
            Destroy(gameObject);
        }
        if (other.tag == "TilemapCollider")
        {
            Destroy(gameObject);
        }
        
    }
}
