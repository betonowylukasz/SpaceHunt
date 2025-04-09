using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class EnemyOld : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public float health = 3f;
    public float moveSpeed = 2.5f;
    private float lastShootTime;
    public float collisionOffset = 0.01f;

    private GameObject player;

    private Rigidbody2D rb;
    public ContactFilter2D movementFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player");
        health = 3;
        rb = GetComponent<Rigidbody2D>();
        lastShootTime = Time.time;
    }

    void Update()
    {
        if (health <= 0) Defeated();
        Movement();
        
        if (Time.time - lastShootTime >= Random.Range(0.75f, 1.25f))
        {
            ShootAtPlayer();
            lastShootTime = Time.time;
        }
    }

    void Movement()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position);
        if (Sqrt(directionToPlayer.x * directionToPlayer.x + directionToPlayer.y * directionToPlayer.y) >= 3)
        {
            directionToPlayer = directionToPlayer.normalized;
            bool success = TryMove(directionToPlayer);

            if (!success)
            {
                success = TryMove(new Vector2(directionToPlayer.x, 0));

                if (!success)
                {
                    success = TryMove(new Vector2(0, directionToPlayer.y));
                }
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime);

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        }
        else return false;
    }

    void ShootAtPlayer()
    {
        Vector3 firePoint = transform.position;
        // Obliczanie kierunku do gracza
        Vector2 directionToPlayer = (playerTransform.position - firePoint).normalized;

        // Obliczanie k�ta obrotu pocisku
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // Tworzenie pocisku z odpowiednim obrotem
        GameObject bullet = Instantiate(bulletPrefab, firePoint, Quaternion.AngleAxis(angle, Vector3.forward));

        // Ustawianie pr�dko�ci pocisku w kierunku gracza
        bullet.GetComponent<Rigidbody2D>().linearVelocity = directionToPlayer * bulletSpeed;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public void Defeated()
    {
        Destroy(gameObject);
    }
}