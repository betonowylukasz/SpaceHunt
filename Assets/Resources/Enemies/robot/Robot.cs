using UnityEngine;
using System.Collections.Generic;
using static System.Math;
using System.Collections;

public class Robot : Enemy
{
    public GameObject singleBulletPrefab;
    public GameObject explosiveBulletPrefab;
    public Transform firePoint;
    public AudioClip shootClip;
    public AudioClip chargeClip;

    public LayerMask obstacleLayerMask;

    public float modeSwitchInterval = 4f;
    public float attackInterval = 1f;
    public int burstBullets = 12;

    private float modeTimer = 0f;
    private float attackTimer = 0f;
    private int currentMode = 1;
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

        modeTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (modeTimer >= modeSwitchInterval)
        {
            modeTimer = 0f;
            currentMode = Random.Range(0, 2); // 0 - run + shoot, 1 - charge shot
            attackTimer = 0f;
        }

        if (currentMode == 0)
        {
            Movement();
        }
        else animator.SetBool("isMoving", false);
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            Attack();
        }
    }

    protected override void Attack()
    {
        if (currentMode == 0)
        {
            StartCoroutine(FireTripleShot());
        }
        else if (currentMode == 1)
        {
            FireExplodingShot();
        }
    }

    void FireRandomShot()
    {
        float angle = Random.Range(0f, 360f);
        Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir);

        Instantiate(singleBulletPrefab, firePoint.position, rot);
        SoundController.Instance.PlaySound(shootClip, 0.4f, 0.75f);
    }

    IEnumerator FireTripleShot()
    {
        Vector3 toPlayer = playerTransform.position - firePoint.position;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        int shots = 3;
        float spread = 10f; // ±10 stopni

        for (int i = 0; i < shots; i++)
        {
            float angle = baseAngle + Random.Range(-spread, spread);
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir);

            Instantiate(singleBulletPrefab, firePoint.position, rot);
            SoundController.Instance.PlaySound(shootClip, 0.4f, 0.75f);

            yield return new WaitForSeconds(0.15f); // ma³a przerwa miêdzy strza³ami
        }
    }

    void FireExplodingShot()
    {
        float angle = Random.Range(0f, 360f);
        Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir);

        GameObject bullet = Instantiate(explosiveBulletPrefab, firePoint.position, rot);
        ExplosiveBullet explosive = bullet.GetComponent<ExplosiveBullet>();
        if (explosive != null)
        {
            explosive.Init(burstBullets);
        }

        SoundController.Instance.PlaySound(shootClip, 0.5f, 0.5f);
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
            animator.SetBool("isMoving", true);
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
