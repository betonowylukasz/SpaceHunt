using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static System.Math;
using static UnityEngine.RectTransform;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float collisionOffset = 0;
    public ContactFilter2D movementFilter;

    public GameObject bulletPrefab;
    public GameObject barrel;
    public GameObject crosshair;
    public GameObject weapon;
    public Camera mainCamera;
    public float rotationSpeed = 20f;
    public int health = 3;
    public int kills = 0;

    Vector2 movementInput;
    Vector2 lookInput;
    Rigidbody2D rb;
    SpriteRenderer spriteRender;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called before the first frame update
    void Start()
    {
        kills = 0;
        health = 3;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRender = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if(movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);

            if(!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));

                if(!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
            }
            animator.SetBool("isMoving", success);
            if (!success) animator.SetInteger("direction", 0);

            if (Abs(movementInput.x) >= Abs(movementInput.y))
            {
                if (movementInput.x < 0)
                {
                    animator.SetInteger("direction", 4);
                }
                else animator.SetInteger("direction", 6);
            }
            else
            {
                if (movementInput.y < 0)
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

        MoveCrosshair(lookInput);

    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset);

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

    private void MoveCrosshair(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            if (direction.x > 1 || direction.y > 1)
            {
                Vector2 screenSize = new Vector2(Screen.width, Screen.height);

                // Oblicz �rodek ekranu
                Vector2 screenCenter = screenSize / 2f;

                // Przekszta�� pozycj� myszy wzgl�dem �rodka ekranu
                Vector2 transformedDirection = direction - screenCenter;

                // Ustaw przekszta�con� pozycj� jako pozycj� celownika
                crosshair.transform.localPosition = transformedDirection.normalized * 2.5f;
            }
            else
            {
                direction.Normalize();
                crosshair.transform.localPosition = direction * 2.5f;
            }
            RotateWeaponTowardsCrosshair();
        }
    }

    private void RotateWeaponTowardsCrosshair()
    {
        Vector3 characterPosition = transform.position;
        Vector3 crosshairPosition = crosshair.transform.position;

        Vector3 direction = crosshairPosition - characterPosition;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float radius = 0.8f;
        float xOffset = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float yOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        Vector3 weaponOffset = new Vector3(xOffset, yOffset, 0f);
        weapon.transform.localPosition = weaponOffset;

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        weapon.transform.rotation = Quaternion.Slerp(weapon.transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        SpriteRenderer weaponSpriteRenderer = weapon.GetComponent<SpriteRenderer>();
        if (direction.x < 0)
        {
            weaponSpriteRenderer.flipY = true; // Obr�� broni� do g�ry nogami
        }
        else
        {
            weaponSpriteRenderer.flipY = false; // Zresetuj obr�t broni
        }
    }

    public void AddKill()
    {
        kills += 1;
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnLook(InputValue lookValue)
    {
        lookInput = lookValue.Get<Vector2>();
    }

    void OnFire()
    {
        GameObject bullet = Instantiate(bulletPrefab, barrel.transform.position, weapon.transform.rotation);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = (crosshair.transform.position - bullet.transform.GetChild(0).position) * 6f;
    }

    void PlayerWalk()
    {
        GetComponent<AudioSource>().UnPause();
    }

    void PlayerStop()
    {
        GetComponent<AudioSource>().Pause();
    }
}
