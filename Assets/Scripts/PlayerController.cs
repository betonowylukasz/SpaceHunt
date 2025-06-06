using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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
    public float dodgeDuration = 0.6f;
    public float dodgeCost = 20f;
    public float staminaRegenRate = 7.5f;
    public UnityEngine.UI.Slider healthBar;
    public UnityEngine.UI.Slider staminaBar;

    public event Action OnMoveAction;
    public event Action OnDodgeAction;
    public event Action OnLookAction;
    public event Action OnShootAction;
    public event Action OnWeapon1Action;
    public event Action OnWeapon2Action;

    private bool isDodging = false;
    private bool freezePlayer = false;
    private bool isInvincible = false;
    private Vector2 movementInput;
    private Vector2 lookInput;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRender;
    private Animator animator;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private float health = 100;
    private float stamina = 100f;
    private float damageRedution = 0f;

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
    }

    private void FixedUpdate()
    {
        StaminaRegen();

        if (movementInput != Vector2.zero && CanMove())
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

        if (animator.GetBool("isMoving"))
        {
            PlayerWalk();
        }
        else PlayerStop();
    }

    private bool TryMove(Vector2 direction, float speedMultiplayer = 1)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * speedMultiplayer * Time.fixedDeltaTime);

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * speedMultiplayer * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        }
        else return false;
    }

    private IEnumerator Dodge()
    {
        freezePlayer = true;
        isDodging = true;
        isInvincible = true;

        Vector2 dodgeDirection = movementInput != Vector2.zero ? movementInput.normalized : Vector2.right;
        
        if (dodgeDirection.x >= 0) animator.Play("janus_rr", 0);
        else animator.Play("janus_rl", 0);

        float firstPhaseDuration = dodgeDuration / 2f;
        float secondPhaseDuration = dodgeDuration / 2f;
        float thirdPhaseDuration = dodgeDuration / 4f;
        float elapsed = 0f;

        // Faza 1 – szybki ruch
        while (elapsed < firstPhaseDuration)
        {
            TryMove(dodgeDirection, 1.5f);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        elapsed = 0f;

        // Faza 2 – wolniejsze wyhamowanie
        while (elapsed < secondPhaseDuration)
        {
            TryMove(dodgeDirection, 0.75f);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        animator.Play("janus_idle", 0);
        freezePlayer = false;
        isInvincible = false;
        elapsed = 0f;

        // Faza 3 – odstęp między kolejnym przewrotem
        while (elapsed < thirdPhaseDuration)
        {
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDodging = false;
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

    public void UpgradePlayer(Upgrade upgrade)
    {
        switch (upgrade.type)
        {
            case Upgrade.UpgradeType.Health:
                health += upgrade.value;
                healthBar.value = health;
                break;
            case Upgrade.UpgradeType.DamageReduction:
                damageRedution += 0.1f * upgrade.value;
                break;
            case Upgrade.UpgradeType.Stamina:
                dodgeCost -= upgrade.value;
                break;
            case Upgrade.UpgradeType.StaminaRegeneration:
                staminaRegenRate = 0.1f * upgrade.value;
                break;
            case Upgrade.UpgradeType.Ammo:
                Weapon[] weapons = weaponManager.GetWeapons();
                for (int i=0;i<weapons.Length;i++)
                {
                    weapons[i].AddAmmo(upgrade.value);
                }
                break;
            case Upgrade.UpgradeType.Speed:
                moveSpeed += 0.01f * upgrade.value;
                break;
        }
    }

    private void StaminaRegen()
    {
        if (stamina < 100)
        {
            stamina += Time.deltaTime * staminaRegenRate;
            if(stamina > 100) stamina = 100;
            staminaBar.value = stamina;
        }
    }

    public void TakeDamage()
    {
        if (isInvincible) return;
        health -= Max(1, 10f - damageRedution);
        healthBar.value = health;
        SoundController.Instance.PlaySound(takingDamageSound);
    }

    public void DeadSound()
    {
        SoundController.Instance.PlaySound(deadSound);
    }

    public bool CanMove()
    {
        if (GameController.Instance) return !freezePlayer && !GameController.Instance.ScreenFader.isFading;
        else return !freezePlayer;
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
        if (movementInput.sqrMagnitude > 0.1f) OnMoveAction?.Invoke();
    }

    void OnLook(InputValue lookValue)
    {
        lookInput = lookValue.Get<Vector2>();
        if (lookInput.sqrMagnitude > 0.1f) OnLookAction?.Invoke();
    }

    void OnShoot()
    {
        if (CanMove())
        {
            weaponManager.Shoot();
            OnShootAction?.Invoke();
        }
    }

    void OnWeapon1()
    {
        weaponManager.Weapon1();
        OnWeapon1Action?.Invoke();
    }

    void OnWeapon2()
    {
        weaponManager.Weapon2();
        OnWeapon2Action?.Invoke();
    }

    void OnDodge()
    {
        if (!isDodging && stamina >= dodgeCost && CanMove())
        {
            StartCoroutine(Dodge());
            stamina -= Max(1, dodgeCost);
            staminaBar.value = stamina;
            OnDodgeAction?.Invoke();
        }
    }

    void PlayerWalk()
    {
        SoundController.Instance.StartWalking();
    }

    void PlayerStop()
    {
        SoundController.Instance.StopWalking();
    }
}
