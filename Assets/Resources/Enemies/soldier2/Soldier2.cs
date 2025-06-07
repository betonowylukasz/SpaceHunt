using UnityEngine;
using static System.Math;

public class Soldier2 : Enemy
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject weapon;
    public int pelletCount = 5;
    public float spreadAngle = 15f;
    public float minFireTime = 1f;
    public float maxFireTime = 0.5f;
    public float fireRange = 25f;
    public float weaponRadius = 0.08f;
    public float rotationSpeed = 20f;
    public float minDistanceFromPlayer = 1.5f;
    public AudioClip shootClip;

    private float fireTimer;
    private float nextFireTime;
    private bool isPaused = false;
    private float pauseTimer = 0f;
    private float timeUntilNextPause = 0f;
    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        nextFireTime = Random.Range(minFireTime * 2, maxFireTime * 2);
        SetNextPause();
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        RotateWeaponTowardsPlayer();
        Movement();

        fireTimer += Time.deltaTime;
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (fireTimer >= nextFireTime && distance <= fireRange)
        {
            fireTimer = 0f;
            Attack();
            nextFireTime = Random.Range(minFireTime, maxFireTime);
        }
    }

    private void RotateWeaponTowardsPlayer()
    {
        if (player == null) return;

        Vector3 enemyPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;

        Vector3 direction = playerPosition - enemyPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float radius = weaponRadius;
        float xOffset = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float yOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        Vector3 weaponOffset = new Vector3(xOffset, yOffset, 0f);
        weapon.transform.localPosition = weaponOffset;

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        weapon.transform.rotation = Quaternion.Slerp(weapon.transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        SpriteRenderer weaponSpriteRenderer = weapon.GetComponent<SpriteRenderer>();
        if (direction.x < 0)
        {
            if (!weaponSpriteRenderer.flipY)
            {
                Vector3 firePointLocalPos = firePoint.localPosition;
                firePointLocalPos.y = -firePointLocalPos.y;
                firePoint.localPosition = firePointLocalPos;
            }
            weaponSpriteRenderer.flipY = true;
        }
        else
        {
            if (weaponSpriteRenderer.flipY)
            {
                Vector3 firePointLocalPos = firePoint.localPosition;
                firePointLocalPos.y = -firePointLocalPos.y;
                firePoint.localPosition = firePointLocalPos;
            }
            weaponSpriteRenderer.flipY = false;
        }
    }

    void SetNextPause()
    {
        timeUntilNextPause = Random.Range(4f, 12f);      // czas miêdzy pauzami
        pauseTimer = Random.Range(1.5f, 4f);          // d³ugoœæ pauzy
    }

    protected override void Movement()
    {
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                SetNextPause();
            }
            return;
        }

        timeUntilNextPause -= Time.deltaTime;
        if (timeUntilNextPause <= 0f)
        {
            isPaused = true;
            animator.SetBool("isMoving", false);
            return;
        }

        Vector2 directionToPlayer = (playerTransform.position - transform.position);
        if (Sqrt(directionToPlayer.x * directionToPlayer.x + directionToPlayer.y * directionToPlayer.y) >= minDistanceFromPlayer)
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
            animator.SetBool("isMoving", true);
            if (!success) animator.SetInteger("direction", 0);

            if (Abs(directionToPlayer.x) >= Abs(directionToPlayer.y))
            {
                if (directionToPlayer.x < 0)
                {
                    animator.SetInteger("direction", 4);
                }
                else animator.SetInteger("direction", 6);
            }
            else
            {
                if (directionToPlayer.y < 0)
                {
                    animator.SetInteger("direction", 8);
                }
                else animator.SetInteger("direction", 2);
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
            animator.SetInteger("direction", 0);
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

    protected override void Attack()
    {
        float angleStep = spreadAngle / (pelletCount - 1);
        float startAngle = -spreadAngle / 2;

        for (int i = 0; i < pelletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * rotation);
        }
        SoundController.Instance.PlaySound(shootClip, 0.4f, 0.8f);
    }
}
