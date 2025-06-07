using System.Collections;
using UnityEngine;

public class Knight : Enemy
{
    public GameObject standardBulletPrefab;
    public float attackInterval0 = 4f;
    public float attackInterval1 = 2.5f;
    public float cooldown = 2f;
    public float modeSwitchInterval = 8f;
    public float projectileSpeed = 3f;
    public int ringCount = 12;
    public int rainCount = 15;
    public int convergeCount = 20;
    public float rainRadius = 4f;
    public float ringDistance = 4f;
    public float convergeRadius = 7f;
    public float directionChangeInterval = 2f;

    public LayerMask obstacleLayerMask;

    private float directionChangeTimer = 0f;
    private float modeTimer = 10f;
    private float attackTimer = 0f;
    private int currentMode = 0;
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

        if (modeTimer >= modeSwitchInterval + cooldown)
        {
            modeTimer = 0f;
            currentMode = Random.Range(0, 3);
            attackTimer = 0f;
            Attack();
        }
        if (((currentMode == 0 && attackTimer >= attackInterval0) || (currentMode == 1 && attackTimer >= attackInterval1)) && modeTimer < modeSwitchInterval)
        {
            attackTimer = 0f;
            Attack();
        }
        if (modeTimer >= modeSwitchInterval)
        {
            animator.SetBool("isAttacking", false);
        }
        Movement();
    }

    protected override void Attack()
    {
        switch (currentMode)
        {
            case 0:
                StartCoroutine(AttackRing());
                break;
            case 1:
                StartCoroutine(AttackRain());
                break;
            case 2:
                StartCoroutine(AttackWall());
                break;
        }
    }

    IEnumerator AttackRing()
    {
        animator.SetInteger("attack", 0);
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.5f);
        Vector3 playerPos = playerTransform.position;

        for (int i = 0; i < ringCount; i++)
        {
            float angle = i * (360f / ringCount);
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * ringDistance;
            Vector3 spawnPos = playerPos + offset;

            GameObject bullet = Instantiate(standardBulletPrefab, spawnPos, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            // Kierunek od pozycji pocisku do gracza (do środka pierścienia)
            Vector2 dir = (playerPos - spawnPos).normalized;

            yield return new WaitForSeconds(0.05f);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * projectileSpeed;
            if (i == 6) animator.SetBool("isAttacking", false);
        }
    }

    IEnumerator AttackRain()
    {
        animator.SetInteger("attack", 1);
        animator.SetBool("isAttacking", true);
        int rows = 5;                    // mniej fal
        int bulletsPerRow = 15;          // rzadsze
        float width = 15f;
        float heightOffset = 7f;
        float jitter = 0.7f;             // większy rozrzut
        float delay = 0.2f;              // więcej czasu między falami

        Vector3 center = playerTransform.position;

        for (int row = 0; row < rows; row++)
        {
            for (int i = 0; i < bulletsPerRow; i++)
            {
                float t = i / (float)(bulletsPerRow - 1);
                float x = Mathf.Lerp(-width / 2, width / 2, t) + Random.Range(-jitter, jitter);
                Vector3 spawnPos = new Vector3(center.x + x, center.y + heightOffset, 0f);

                GameObject bullet = Instantiate(standardBulletPrefab, spawnPos, Quaternion.identity);

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.linearVelocity = Vector2.down * projectileSpeed;

                bullet.transform.rotation = Quaternion.identity;
            }

            yield return new WaitForSeconds(delay);
        }
    }


    IEnumerator AttackWall()
    {
        animator.SetInteger("attack", 2);
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.5f);
        int wallCount = 4;               // ile ścian
        int bulletsPerWall = 24;         // ile pocisków w pionie
        float verticalSpacing = 0.5f;    // odległość między pociskami w pionie
        float wallSpacing = 3.0f;        // odległość między ścianami
        float startX = playerTransform.position.x - (wallCount - 1) * wallSpacing - 5f; // najbardziej na lewo
        float startY = playerTransform.position.y - ((bulletsPerWall - 1) / 2f) * verticalSpacing;
        for (int w = wallCount - 1; w >= 0; w--)
        {
            float xPos = startX + w * wallSpacing;

            for (int b = 0; b < bulletsPerWall; b++)
            {
                Vector3 spawnPos = new Vector3(xPos, startY + b * verticalSpacing, 0f);

                GameObject bullet = Instantiate(standardBulletPrefab, spawnPos, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * projectileSpeed;
            }

            yield return new WaitForSeconds(0.3f); // opóźnienie między kolejnymi ścianami
            animator.SetBool("isAttacking", false);
        }
    }

    protected override void Movement()
    {
        directionChangeTimer += Time.fixedDeltaTime;

        // Sprawdzenie kolizji
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

        // Jeśli uderzył w ścianę lub minął interwał zmiany kierunku
        if (collidedWithWall || directionChangeTimer >= directionChangeInterval)
        {
            currentDirection = GetRandomHorizontalDirection();
            directionChangeTimer = 0f;
        }

        rb.MovePosition(rb.position + currentDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private Vector2 GetRandomHorizontalDirection()
    {
        float x = Random.value < 0.5f ? -1f : 1f;
        float y = Random.Range(-0.5f, 0.5f); // lekko góra/dół
        return new Vector2(x, y).normalized;
    }
}
