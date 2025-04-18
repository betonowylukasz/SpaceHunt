using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float health;
    public float moveSpeed;

    protected GameObject player;
    protected Transform playerTransform;
    public event System.Action OnEnemyDeath;


    protected Rigidbody2D rb;
    public ContactFilter2D movementFilter;
    public List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Start() { }

    protected virtual void Update()
    {

    }

    protected abstract void Movement();
    protected abstract void Attack();

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
        OnEnemyDeath?.Invoke();
    }
}
