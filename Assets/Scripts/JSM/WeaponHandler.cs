using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.XR;
using System.Collections.Generic;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private int weaponId;
    public int WeaponId => weaponId;
    [Header("Attack Info")]
    [SerializeField] private float delay = 1f;
    public float Delay { get => delay; set => delay = value; }

    [SerializeField] private float specialDelay = 1f;
    public float SpecialDelay { get => specialDelay; set => specialDelay = value; }

    [SerializeField] private float weaponSize = 1f;
    public float WeaponSize { get => weaponSize; set => weaponSize = value; }

    [SerializeField] private float power = 1f;
    public float Power { get => power; set => power = value; }

    [SerializeField] private float speed = 1f;
    public float Speed { get => speed; set => speed = value; }

    [SerializeField] private float attackRange = 10f;
    public float AttackRange { get => attackRange; set => attackRange = value; }

    public LayerMask target;

    [Header("Knock Back Info")]
    [SerializeField] private bool isOnKnockback = false;
    public bool IsOnKnockback { get => isOnKnockback; set => isOnKnockback = value; }

    [SerializeField] private float knockbackPower = 0.1f;
    public float KnockbackPower { get => knockbackPower; set => knockbackPower = value; }

    [SerializeField] private float knockbackTime = 0.5f;
    public float KnockbackTime { get => knockbackTime; set => knockbackTime = value; }

    private static readonly int IsAttack = Animator.StringToHash("IsAttack");

    public PlayerController Controller { get; private set; }
    private Queue<GameObject> mineQueue = new Queue<GameObject>();

    private Animator animator;
    private SpriteRenderer weaponRenderer;
    private GameObject extraWeaponInstance;

    [Header("Skill Settings")]
    private float doubleDuration = 5f;     //복제스킬 지속시간
    [SerializeField] private GameObject doubleSkill;
    private float invincibleDuration = 4f; //무적스킬 지속시간
    [SerializeField] private GameObject minePrefab;         // 인스펙터에서 지뢰 프리팹을 할당
    [SerializeField] private int maxMines = 3;
    public Material gray;


    private PlayerController controller;

    protected virtual void Awake()
    {
        Controller = GetComponentInParent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
        weaponRenderer = GetComponentInChildren<SpriteRenderer>();

        animator.speed = 1.0f / delay;
        transform.localScale = Vector3.one * weaponSize;
    }

    protected virtual void Start()
    {
        controller = GetComponentInParent<PlayerController>();
    }

    public virtual void Attack()
    {
        AttackAnimation();
    }
    public virtual void SpecialAttack()
    {
        switch (WeaponId)
        {
            case 1://5초간 무기 발사 2배 쿨타임 15초
                StartCoroutine(TemporaryExtraWeapon(doubleDuration));
                SoundManager.PlayClip("SpecialBasicSound");
                break;
            case 2://평타의 3배데미지 지뢰 설치 쿨타임 20초
                PlaceMine();
                SoundManager.PlayClip("SpecialElfSound");

                break;
            case 3://5초 무적 쿨타임 30초
                StartCoroutine(TemporaryInvincibility(invincibleDuration));
                SoundManager.PlayClip("SpecialDwarfSound");

                break;
        }
    }

    public void AttackAnimation()
    {
        animator.SetTrigger(IsAttack);
    }

    public virtual void Rotate(bool isLeft)
    {
        weaponRenderer.flipY = isLeft;
    }
    private IEnumerator TemporaryExtraWeapon(float duration)
    {
        doubleSkill.SetActive(true);
        WeaponUpgrade.Instance.ActiveDoubleWeapon();
        yield return new WaitForSeconds(duration);
        doubleSkill.SetActive(false);
        WeaponUpgrade.Instance.InactiveDoubleWeapon();
    }
    private IEnumerator TemporaryInvincibility(float duration)
    {
        if (controller == null)
        {
            Debug.LogWarning("PlayerController를 찾을 수 없어 무적 불가");
            yield break;
        }

        controller.SetInvincible(true);

        float blinkDuration = 1f;
        float stableTime = Mathf.Max(0, duration - blinkDuration);
        yield return new WaitForSeconds(stableTime);

        float blinkInterval = 0.1f;
        float elapsed = 0f;
        bool visible = true;

        var rends = controller.GetComponentsInChildren<SpriteRenderer>();
        while (elapsed < blinkDuration)
        {
            foreach (var r in rends)
                r.enabled = visible;

            visible = !visible;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        foreach (var r in rends)
            r.enabled = true;

        controller.SetInvincible(false);
    }
    private void PlaceMine()
    {
        if (minePrefab == null)
        {
            Debug.LogWarning("Mine Prefab이 할당되지 않았습니다!");
            return;
        }

        // 최대 개수 초과 시 가장 오래된 지뢰 제거
        if (mineQueue.Count >= maxMines)
        {
            var oldMine = mineQueue.Dequeue();
            Destroy(oldMine);
        }

        // 플레이어 위치에 지뢰 설치 (원하는 위치 조절 가능)
        Vector3 spawnPos = controller.transform.position;
        var mine = Instantiate(minePrefab, spawnPos, Quaternion.identity);
        mineQueue.Enqueue(mine);

        Debug.Log($"지뢰 설치! 현재 개수: {mineQueue.Count}/{maxMines}");
    }
}
