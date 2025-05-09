using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : BaseController
{
    private new Camera camera;

    [SerializeField] private Transform weaponPivot;

    private Vector2 knockback = Vector2.zero;
    private float knockbackDuration = 0.0f;

    protected PlayerAnimationHandler animationHandler;

    protected PlayerStatHandler statHandler;

    protected PlayerUI playerUI;

    [SerializeField] public WeaponHandler WeaponPrefab;
    protected WeaponHandler weaponHandler;

    protected bool isAttacking;
    private float timeSinceLastAttack = float.MaxValue;


    [SerializeField] private Image dodgeCooldownImage;

    [SerializeField] private float dodgeDistance = 5f;
    [SerializeField] private float dodgeCooldown = 1.5f;
    [SerializeField] private float dodgeDuration = 0.2f;

    private bool isDodging = false;
    private float dodgeTimer = 0f;


    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float ghostSpawnInterval = 0.05f;

    protected override void Awake()
    {
        base.Awake();
        statHandler = GetComponent<PlayerStatHandler>();
        animationHandler = GetComponent<PlayerAnimationHandler>();
        playerUI = GetComponent<PlayerUI>();

        camera = Camera.main;

        if (WeaponPrefab != null)
            weaponHandler = Instantiate(WeaponPrefab, weaponPivot);
        else
            weaponHandler = GetComponentInChildren<WeaponHandler>();
    }
    protected override void Update()
    {
        base.Update();
        if (dodgeTimer > 0f)
        {
            dodgeTimer -= Time.deltaTime;
            dodgeCooldownImage.fillAmount = dodgeTimer / dodgeCooldown;
        }
        else
        {
            dodgeCooldownImage.fillAmount = 0f;
        }
        HandleAttackDelay();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (knockbackDuration > 0.0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }


    protected override void Movment(Vector2 direction)
    {
        if (isDodging)
        {
            return;
        }
        direction = direction * statHandler.CurrentSpeed; // 속도 조절은 여기서
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f;
            direction += knockback;
        }
        _rigidbody.velocity = direction;
        animationHandler.Move(direction);
    }

    protected override void Rotate(Vector2 direction)
    {
        base.Rotate(direction);

        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bool isLeft = Mathf.Abs(rotZ) > 90f;
        if (weaponPivot != null)
        {
            weaponPivot.rotation = Quaternion.Euler(0, 0, rotZ);
        }
        weaponHandler?.Rotate(isLeft);
    }
    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        knockback = -(other.position - transform.position).normalized * power;
    }

    private void HandleAttackDelay()
    {
        if (weaponHandler == null)
            return;

        if (timeSinceLastAttack <= weaponHandler.Delay)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        if (isAttacking && timeSinceLastAttack > weaponHandler.Delay)
        {
            timeSinceLastAttack = 0;
            Attack();
        }
    }

    protected void Attack()
    {
        if (lookDirection != Vector2.zero)
            weaponHandler?.Attack();
    }

    public void Death()
    {
        _rigidbody.velocity = Vector3.zero;

        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = renderer.color;
            color.a = 0.3f;
            renderer.color = color;
        }

        foreach (Behaviour component in transform.GetComponentsInChildren<Behaviour>())
        {
            component.enabled = false;
        }

        Destroy(gameObject, 2f);
    }

    public void TakeDamaged()
    {
        if (statHandler.Health > 0)
        {
            statHandler.TakeDamage();
            playerUI.UpdateHealthImg();
            animationHandler.Damage();
        }

        if (statHandler.Health <= 0)
        {
            Death();
        }
    }

    public void PlusHealth()
    {
        if (statHandler.Health >= statHandler.MaxHealth)
        {
            return;
        }

        statHandler.Heal();
        playerUI.UpdateHealthImg();
    }


    void OnMove(InputValue inputValue)
    {
        movementDirection = inputValue.Get<Vector2>();
        movementDirection = movementDirection.normalized;
    }

    void OnLook(InputValue inputValue)
    {

        Vector2 mousePosition = inputValue.Get<Vector2>();
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition);
        lookDirection = (worldPos - (Vector2)transform.position);

        if (lookDirection.magnitude < .9f)
        {
            lookDirection = Vector2.zero;
        }
        else
        {
            lookDirection = lookDirection.normalized;
        }
    }

    void OnFire(InputValue inputValue)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        isAttacking = inputValue.isPressed;
    }
    void OnDodge(InputValue inputValue)
    {
        if (isDodging || dodgeTimer > 0f || movementDirection == Vector2.zero)
        {
            Debug.Log("실패");
            return;
        }


        if (inputValue.isPressed)
        {
            Debug.Log("성공");

            StartCoroutine(DodgeCoroutine());
        }
    }

    private IEnumerator DodgeCoroutine()
    {
        isDodging = true;
        dodgeTimer = dodgeCooldown;

        Vector2 dodgeDir = movementDirection.normalized;
        float elapsed = 0f;

        StartCoroutine(SpawnGhosts());

        while (elapsed < dodgeDuration)
        {
            _rigidbody.velocity = dodgeDir * dodgeDistance / dodgeDuration;
            elapsed += Time.deltaTime;
            yield return null;
        }

        _rigidbody.velocity = Vector2.zero;
        isDodging = false;
    }

    private IEnumerator SpawnGhosts()
    {
        float alpha = 0.2f;
        while (isDodging)
        {
            GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
            SpriteRenderer ghostRenderer = ghost.GetComponentInChildren<SpriteRenderer>();
            SpriteRenderer playerRenderer = characterRenderer;


            ghostRenderer.sprite = playerRenderer.sprite;
            ghostRenderer.flipX = playerRenderer.flipX;
            ghostRenderer.material.color = new Color(1f, 1f, 1f, alpha);
            alpha += 0.2f;
            yield return new WaitForSeconds(ghostSpawnInterval);
        }
    }

}
