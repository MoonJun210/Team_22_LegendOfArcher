using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class RangeWeaponHandler : WeaponHandler
{
    [Header("Ranged Attack Data")]
    [SerializeField] private Transform projectileSpawnPosition;

    [SerializeField] private int bulletIndex;
    public int BulletIndex { get { return bulletIndex; } }

    [SerializeField] private float bulletSize = 1;
    public float BulletSize { get { return bulletSize; } }

    [SerializeField] private float duration;
    public float Duration { get { return duration; } }

    [SerializeField] private float spread;
    public float Spread { get { return spread; } }

    [SerializeField] private int numberofProjectilesPerShot;
    public int NumberofProjectilesPerShot { get { return numberofProjectilesPerShot; } }

    [SerializeField] private float multipleProjectilesAngel;
    public float MultipleProjectilesAngel { get { return multipleProjectilesAngel; } }

    [SerializeField] private Color projectileColor;
    public Color ProjectileColor { get { return projectileColor; } }

    [Header("Special Modes")]
    public bool tripleShotEnabled = false;   // 3점사 모드
    private float burstInterval = 0.1f;

    private ProjectileManager projectileManager;
    protected override void Start()
    {
        base.Start();
        projectileManager = ProjectileManager.Instance;
    }
    public override void Attack()
    {
        base.Attack();

        if (tripleShotEnabled)
            StartCoroutine(TripleBurst());
        else
            SingleBurst();
    }

    private IEnumerator TripleBurst()
    {
        for (int i = 0; i < 3; i++)
        {
            SingleBurst();
            yield return new WaitForSeconds(burstInterval);
        }
    }

    private void SingleBurst()
    {
        float angleStep = MultipleProjectilesAngel;
        int count = NumberofProjectilesPerShot;
        float startAng = -(count - 1) * angleStep * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAng + angleStep * i
                        + Random.Range(-Spread, Spread);
            CreateProjectile(Controller.LookDirection, angle);
        }
    }

    private void CreateProjectile(Vector2 _lookDirection, float angle)
    {
        projectileManager.ShootBullet(
            this,
            projectileSpawnPosition.position,
            RotateVector2(_lookDirection, angle)
            );
    }
    private static Vector2 RotateVector2(Vector2 v, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * v;
    }
}
