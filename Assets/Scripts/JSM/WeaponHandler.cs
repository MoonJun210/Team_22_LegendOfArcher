using UnityEngine;
using System.Collections;

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

    private Animator animator;
    private SpriteRenderer weaponRenderer;
    private GameObject extraWeaponInstance;

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
                if (extraWeaponInstance == null)
                    StartCoroutine(TemporaryExtraWeapon(5f));
                break;
            case 2://평타의 3배데미지 지뢰 설치 쿨타임 20초
                Debug.Log(WeaponId + "번 특수공격!");
                break;
            case 3://5초 무적 쿨타임 30초
                Debug.Log(WeaponId + "번 특수공격!");
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
        var original = weaponRenderer.gameObject;

        var clone = Instantiate(
            original,
            transform.parent,
            false
        );

        clone.name = original.name + "_Clone";
        clone.transform.localPosition = new Vector3(-0.3f, 0f, 0f);
        clone.transform.localRotation = Quaternion.identity;
        clone.transform.localScale = Vector3.one;
        WeaponUpgrade.Instance.ActiveDoubleWeapon();

        yield return new WaitForSeconds(duration);
        Destroy(clone);
        WeaponUpgrade.Instance.InactiveDoubleWeapon();
    }
}
