using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private GameObject warningZonePrefab;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject laserPrefab;

    private StatHandler statHandler;
    private int phase = 1;
    private bool laserActivated = false;

    private void Awake()
    {
        statHandler = GetComponent<StatHandler>();
    }

    private void Start()
    {
        StartCoroutine(BossRoutine());
    }

    private void Update()
    {
        CheckPhase();
    }
    private void CheckPhase()
    {
        float hpRatio = statHandler.CurrentHP / statHandler.MaxHP;

        if (!laserActivated && hpRatio <= 0.77f)
        {
            laserActivated = true;
            StartCoroutine(LaserPattern());
        }

        if (hpRatio <= 0.66f && phase == 1)
        {
            phase = 2;
            ActivatePhase2Patterns();
        }
        else if (hpRatio <= 0.33f && phase == 2)
        {
            phase = 3;
            ActivatePhase3Patterns();
        }
    }

    private IEnumerator BossRoutine()
    {
        while (statHandler.CurrentHP > 0)
        {
            yield return StartCoroutine(DoPattern());
        }

        OnDeath();
    }

    private IEnumerator DoPattern()
    {
        Vector3 targetArea = GetPlayerPosition();
        GameObject warning = Instantiate(warningZonePrefab, targetArea, Quaternion.identity);

        yield return new WaitForSeconds(1f);
        Destroy(warning);

        PerformRandomAttack();
        yield return new WaitForSeconds(1.5f);
    }

    private void PerformRandomAttack()
    {
        int pattern = Random.Range(0, 3);
        Debug.Log("패턴 실행: " + pattern); // 패턴 번호 출력

        switch (pattern)
        {
            case 0: StartCoroutine(MeleePattern()); break;
            case 1: StartCoroutine(RangedPattern()); break;
            case 2: StartCoroutine(ShockwavePattern()); break;
        }
    }

    private IEnumerator MeleePattern()
    {
        Vector3 targetPos = GetPlayerPosition();
        GameObject zone = Instantiate(warningZonePrefab, targetPos, Quaternion.identity);
        zone.transform.localScale = new Vector3(3f, 3f, 1f);

        yield return new WaitForSeconds(1f);
        Destroy(zone);

        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 1.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player hit by melee attack!");
            }
        }
    }

 private IEnumerator RangedPattern()
    {
        Vector3 targetPos = GetPlayerPosition();
        GameObject zone = Instantiate(warningZonePrefab, targetPos, Quaternion.identity);
        zone.transform.localScale = new Vector3(2f, 2f, 1f);

        yield return new WaitForSeconds(1f);
        Destroy(zone);

        Vector3 spawnPos = transform.position;
        Vector2 direction = (targetPos - spawnPos).normalized;

        Debug.Log("방향 계산 완료: " + direction);

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Debug.Log("Projectile 생성 완료");

        projectile.GetComponent<Projectile>().SetDirection(direction);
    }

    private IEnumerator ShockwavePattern()
    {
        Vector3 bossPos = transform.position;
        GameObject zone = Instantiate(warningZonePrefab, bossPos, Quaternion.identity);
        zone.transform.localScale = new Vector3(4f, 4f, 1f);

        yield return new WaitForSeconds(1f);
        Destroy(zone);

        Collider2D[] hits = Physics2D.OverlapCircleAll(bossPos, 2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player hit by shockwave!");
            }
        }
    }

    private IEnumerator LaserPattern()
    {
        Vector3 bossPos = transform.position;
        Vector3 playerPos = GetPlayerPosition();
        Vector2 direction = (playerPos - bossPos).normalized;

        GameObject warning = Instantiate(warningZonePrefab, bossPos, Quaternion.identity);
        warning.transform.right = direction;
        warning.transform.localScale = new Vector3(8f, 0.2f, 1f);

        yield return new WaitForSeconds(1f);
        Destroy(warning);

        GameObject laser = Instantiate(laserPrefab, bossPos, Quaternion.identity);
        laser.transform.right = direction;

        RaycastHit2D hit = Physics2D.Raycast(bossPos, direction, 8f, LayerMask.GetMask("Player"));
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.Log("Player hit by laser!");
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(laser);
    }

    private void ActivatePhase2Patterns()
    {
        Debug.Log("Phase 2 패턴 활성화");
    }

    private void ActivatePhase3Patterns()
    {
        Debug.Log("Phase 3 패턴 활성화");
    }

    private Vector3 GetPlayerPosition()
    {
        return GameObject.FindWithTag("Player").transform.position;
    }

    private void OnDeath()
    {
        Debug.Log("보스 사망 처리");
    }
}
