using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public string enemyName = "";
    public float maxHealth;
    public float health;
    public float moveSpeed;
    public AudioClip deathSound;

    protected GameObject player;
    protected Transform playerTransform;
    public event System.Action OnEnemyDeath;
    public event System.Action OnEnemyDamage;

    private bool isDead = false;


    protected Rigidbody2D rb;
    public ContactFilter2D movementFilter;
    public List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        maxHealth = health;
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
        if (health <= 0 && !isDead)
            Die();
        else
            OnEnemyDamage?.Invoke();
    }

    protected virtual void Die()
    {
        isDead = true;
        SoundController.Instance.PlaySound(deathSound, 0.2f, 0.8f);
        Destroy(gameObject);
        OnEnemyDeath?.Invoke();
    }
}
