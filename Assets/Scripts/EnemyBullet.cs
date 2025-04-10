using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float bulletSpeed = 6f;
    public float damage = 1;
    private GameObject player;

    private bool hasHitPlayer = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * bulletSpeed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitPlayer) return;

        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.TakeDamage();
            hasHitPlayer = true;
            Destroy(gameObject);
        }
        if (other.tag == "TilemapCollider")
        {
            Destroy(gameObject);
        }
    }
}
