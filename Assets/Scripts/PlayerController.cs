using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static System.Math;
using static UnityEngine.RectTransform;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public ContactFilter2D movementFilter;
    public GameObject crosshair;
    public Camera mainCamera;
    public float crosshairDistance = 0.5f;
    public WeaponManager weaponManager;

    Vector2 movementInput;
    Vector2 lookInput;
    Rigidbody2D rb;
    SpriteRenderer spriteRender;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called before the first frame update
    void Start()
    {
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

    private void MoveCrosshair(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            if (direction.x > 1 || direction.y > 1)
            {
                Vector2 screenSize = new Vector2(Screen.width, Screen.height);

                Vector2 screenCenter = screenSize / 2f;

                Vector2 transformedDirection = direction - screenCenter;

                crosshair.transform.localPosition = transformedDirection.normalized * crosshairDistance;
            }
            else
            {
                direction.Normalize();
                crosshair.transform.localPosition = direction * crosshairDistance;
            }
        }
    }


    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
        print(movementValue);
    }

    void OnLook(InputValue lookValue)
    {
        lookInput = lookValue.Get<Vector2>();
    }

    void OnShoot()
    {
        weaponManager.Shoot();
    }

    void OnWeapon1()
    {
        weaponManager.Weapon1();
    }

    void OnWeapon2()
    {
        weaponManager.Weapon2();
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
