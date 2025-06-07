using UnityEngine;
using System.Collections.Generic;
using static System.Math;
using System.Collections;

public class Minirobot : Enemy
{
    public GameObject singleBulletPrefab;
    public Transform firePoint;
    public AudioClip shootClip;

    public LayerMask obstacleLayerMask;

    public float attackInterval = 1f;

    private float attackTimer = 0f;
    private bool facingRight = true;

    private Animator animator;

    private Vector2 currentDirection;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentDirection = GetRandomHorizontalDirection();
    }

    void FixedUpdate()
    {
        if (player == null) return;
        attackTimer += Time.deltaTime;
        Movement();
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            Attack();
        }
    }

    protected override void Attack()
    {
        Vector3 toPlayer = playerTransform.position - firePoint.position;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        float spread = 10f; // ±10 stopni

        float angle = baseAngle + Random.Range(-spread, spread);
        Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir);

        Instantiate(singleBulletPrefab, firePoint.position, rot);
        SoundController.Instance.PlaySound(shootClip, 0.4f, 0.75f);
    }

    protected override void Movement()
    {
        int collisionCount = rb.Cast(
            currentDirection,
            movementFilter,
            castCollisions,
            moveSpeed * Time.fixedDeltaTime);

        bool collidedWithWall = false;

        for (int i = 0; i < collisionCount; i++)
        {
            if (((1 << castCollisions[i].collider.gameObject.layer) & obstacleLayerMask) != 0)
            {
                collidedWithWall = true;
                break;
            }
        }

        if (collidedWithWall)
        {
            currentDirection = GetRandomHorizontalDirection();
        }
        else
        {
            if (currentDirection.x < 0 && facingRight)
            {
                Flip();
            }
            else if (currentDirection.x > 0 && !facingRight)
            {
                Flip();
            }
            rb.MovePosition(rb.position + currentDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private Vector2 GetRandomHorizontalDirection()
    {
        float x = Random.value < 0.5f ? -1f : 1f;
        float y = Random.Range(-0.5f, 0.5f); // lekko góra/dó³
        return new Vector2(x, y).normalized;
    }
}
