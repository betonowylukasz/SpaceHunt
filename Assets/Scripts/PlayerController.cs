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
    public AudioClip deadSound;
    public AudioClip takingDamageSound;

    private Vector2 movementInput;
    private Vector2 lookInput;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRender;
    private Animator animator;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private static AudioSource audioSource;

    private static PlayerController _instance;
    public static PlayerController Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRender = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if(movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);

            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));

                if (!success)
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

    public void TakeDamage()
    {
        audioSource.PlayOneShot(takingDamageSound);
    }

    public void DeadSound()
    {
        audioSource.PlayOneShot(deadSound);
    }


    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
        //print(movementValue);
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
