using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float bulletSpeed = 6f;
    public float damage = 1;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * bulletSpeed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerHitbox")
        {
            Destroy(gameObject);
        }
        if (other.tag == "TilemapCollider")
        {
            Destroy(gameObject);
        }
    }
}
