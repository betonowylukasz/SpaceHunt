using UnityEngine;
using static System.Math;
using System.Collections.Generic;

public class Pirate : Enemy
{
    public Transform blasterFirePoint;
    public Transform shotgunFirePoint;
    public GameObject blasterBulletPrefab;
    public GameObject shotgunBulletPrefab;
    public GameObject blaster;
    public GameObject shotgun;
    public AudioClip shootClip;

    public float interval1 = 0.3f;
    public float interval2 = 0.8f;
    public float interval3 = 1.1f;
    public float modeSwitchInterval = 5f;
    public float rotationSpeed = 20f;
    public float specialRotationSpeed = 200f;
    public float weaponRadius = 1.0f;
    public float xWeaponOffset1 = 0.040f;
    public float yWeaponOffset1 = -0.06f;
    public float xWeaponOffset2 = -0.002f;
    public float yWeaponOffset2 = -0.056f;
    public float fireRange = 30f;
    public float blasterSpreadAngle = 30f;
    public float shotgunSpreadAngle = 45f;
    public int shotgunPellets = 5;
    public int radialBullets = 24;
    public float minDistanceFromPlayer = 1.5f;

    private bool isMoving = true;
    private float attackTimer = 0f;
    private float modeTimer = 0f;
    private int currentMode = 0;
    private float attackInterval;
    private float currentRotationAngle1 = 0f;
    private float currentRotationAngle2 = 0f;
    private AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        attackInterval = interval1;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;

        if (currentMode==3)
        {
            RotateWeaponsAroundSelf(blaster, xWeaponOffset1, yWeaponOffset1, 1);
            RotateWeaponsAroundSelf(shotgun, xWeaponOffset2, yWeaponOffset2, 2);
        }
        else
        {
            RotateWeapon(blaster, blasterFirePoint, xWeaponOffset1, yWeaponOffset1);
            RotateWeapon(shotgun, shotgunFirePoint, xWeaponOffset2, yWeaponOffset2);
        }
        Movement();

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        modeTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (modeTimer >= modeSwitchInterval)
        {
            modeTimer = 0f;
            currentMode = Random.RandomRange(0, 4);
            switch (currentMode)
            {
                case 0:
                    attackInterval = interval1;
                    isMoving = true;
                    break;
                case 1:
                    attackInterval = interval2;
                    isMoving = true;
                    break;
                case 2:
                    attackInterval = interval2;
                    isMoving = true;
                    break;
                case 3:
                    attackInterval = interval3;
                    isMoving = false;
                    break;
            }
        }

        if (attackTimer >= attackInterval && distance <= fireRange)
        {
            attackTimer = 0f;
            Attack();
        }

    }

    private void RotateWeapon(GameObject weapon, Transform firePoint, float xWeaponOffset, float yWeaponOffset)
    {
        if (player == null) return;

        Vector3 enemyPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;

        Vector3 direction = playerPosition - enemyPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float radius = weaponRadius;
        float xOffset = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float yOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        Vector3 weaponOffset = new Vector3(xOffset + xWeaponOffset, yOffset + yWeaponOffset, 0f);
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

    private void RotateWeaponsAroundSelf(GameObject weapon, float xWeaponOffset, float yWeaponOffset, int mode)
    {
        float angle;
        if (mode==1)
        {
            currentRotationAngle1 += specialRotationSpeed * Time.deltaTime;
            angle = currentRotationAngle1;
        }
        else
        {
            currentRotationAngle2 -= specialRotationSpeed * Time.deltaTime;
            angle = currentRotationAngle2;
        }

        float radius = weaponRadius;

        float xOffset = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float yOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        Vector3 weaponOffset = new Vector3(xOffset + xWeaponOffset, yOffset + yWeaponOffset, 0f);
        weapon.transform.localPosition = weaponOffset;

        Quaternion rot = Quaternion.Euler(0, 0, angle);
        weapon.transform.rotation = rot;
    }

    protected override void Attack()
    {
        switch (currentMode)
        {
            case 0:
                FireBlaster();
                break;
            case 1:
                FireShotgun();
                break;
            case 2:
                FireBlaster();
                FireShotgun();
                break;
            case 3:
                FireRadial();
                break;
        }
    }

    void FireBlaster()
    {
        Vector3 toPlayer = playerTransform.position - blasterFirePoint.position;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        float randomAngle = baseAngle + Random.Range(-blasterSpreadAngle, blasterSpreadAngle);
        Vector3 direction = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad), 0f);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);

        GameObject bullet = Instantiate(blasterBulletPrefab, blasterFirePoint.position, rotation);
        SoundController.Instance.PlaySound(shootClip, 0.35f, 1f);
    }


    void FireShotgun()
    {
        float angleStep = shotgunSpreadAngle / (shotgunPellets - 1);
        float startAngle = -shotgunSpreadAngle / 2f;

        for (int i = 0; i < shotgunPellets; i++)
        {
            float angle = startAngle + i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, 0, blaster.transform.eulerAngles.z + angle);
            GameObject pellet = Instantiate(shotgunBulletPrefab, shotgunFirePoint.position, rotation);
        }

        SoundController.Instance.PlaySound(shootClip, 0.4f, 0.8f);
    }

    void FireRadial()
    {
        float angleStep = 360f / radialBullets;

        for (int i = 0; i < radialBullets; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            GameObject bullet = Instantiate(blasterBulletPrefab, transform.position, rotation);
        }

        SoundController.Instance.PlaySound(shootClip, 0.4f, 0.9f);
    }

    protected override void Movement()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position);
        if (Sqrt(directionToPlayer.x * directionToPlayer.x + directionToPlayer.y * directionToPlayer.y) >= minDistanceFromPlayer && isMoving)
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
}
