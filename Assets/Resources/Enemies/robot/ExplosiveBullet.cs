using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    public GameObject miniBulletPrefab;
    public float explodeDelay = 1f;
    public float miniSpeed = 5f;
    public float explosiveSpeed = 1.5f;

    private int burstBullets = 12;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * explosiveSpeed;
    }

    public void Init(int count)
    {
        burstBullets = count;
        Invoke(nameof(Explode), explodeDelay);
    }

    void Explode()
    {
        float step = 360f / burstBullets;
        for (int i = 0; i < burstBullets; i++)
        {
            float angle = i * step;
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
            GameObject bullet = Instantiate(miniBulletPrefab, transform.position, Quaternion.LookRotation(Vector3.forward, dir));
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * miniSpeed;
            }
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "TilemapCollider")
        {
            Explode();
        }
    }
}
