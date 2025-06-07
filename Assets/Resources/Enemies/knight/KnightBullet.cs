using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBullet : MonoBehaviour
{
    public float bulletSpeed = 2.5f;
    public float damage = 1;
    private GameObject player;
    private float destroyTime = 10f;
    private float time = 0f;

    private bool hasHitPlayer = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //GetComponent<Rigidbody2D>().linearVelocity = transform.right * bulletSpeed;
    }

    private void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time > destroyTime) Destroy(gameObject);
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
    }
}
