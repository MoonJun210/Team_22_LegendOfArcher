using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private GameObject warningZonePrefab;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private ProjectileWarningShooter shooter;
    [SerializeField] private GameObject directionLinePrefab;

    private StatHandler statHandler;
    private int phase = 1;

    private void Awake()
    {
        statHandler = GetComponent<StatHandler>();
    }

    private void Start()
    {
        statHandler.CurrentHP = statHandler.MaxHP * 0.1f; // 디버그용 체력 설정
        StartCoroutine(BossRoutine());
    }

    private void Update()
    {
        CheckPhase();
    }

    private void CheckPhase()
    {
        float hpRatio = statHandler.CurrentHP / statHandler.MaxHP;

        if (hpRatio <= 0.75f && phase == 1)
        {
            phase = 2;
            ActivatePhase2Patterns();
        }
        else if (hpRatio <= 0.5f && phase == 2)
        {
            phase = 3;
            ActivatePhase3Patterns();
        }
        else if (hpRatio <= 0.25f && phase == 3)
        {
            phase = 4;
            ActivatePhase4Patterns();
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
        PerformRandomAttack();
        yield return new WaitForSeconds(2.5f);
    }

    private void PerformRandomAttack()
    {
        int pattern = Random.Range(0, 3);
        Debug.Log("패턴 실행: " + pattern); // 패턴 번호 출력

        switch (pattern)
        {
            case 0: StartCoroutine(MeleePattern()); break;
            case 1: StartCoroutine(RangedPattern()); break;
        }
    }

    private IEnumerator MeleePattern()
    {
        Vector3 targetPos = GetPlayerPosition();
        GameObject zone = Instantiate(warningZonePrefab, targetPos, Quaternion.identity);
        zone.transform.localScale = new Vector3(3f, 3f, 1f);

        yield return new WaitForSeconds(1f);
        Destroy(zone);

        //지점 폭파 데미지
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 1.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("데미지");
                // TODO: 데미지 처리
            }
        }
    }

    private IEnumerator RangedPattern()
    {
        Vector3 origin = transform.position;
        Vector2 dir = (GetPlayerPosition() - origin).normalized;

        // 별도 클래스가 경고 라인 + 투사체 발사 처리
        yield return StartCoroutine(shooter.Fire(origin, dir));
    }

    private IEnumerator ShockwavePattern()
    {
        Vector3 spawnPos = transform.position;

        for (int i = 0; i < 5; i++)
        {
            float angle = Random.Range(0f, 360f);
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            StartCoroutine(shooter.Fire(spawnPos, dir));
        }

        yield return new WaitForSeconds(1.2f);
    }

    private IEnumerator RepeatShockwavePattern()
    {
        while (phase == 2)
        {
            StartCoroutine(ShockwavePattern());
            yield return new WaitForSeconds(6f); // 반복 주기
        }
    }


    private IEnumerator LaserPattern()
    {
        Vector3 origin = transform.position;
        Vector2 direction = (GetPlayerPosition() - origin).normalized;

        // 1. 경고선 (기본값 사용)
        yield return StartCoroutine(shooter.Fire(origin, direction));

        // 2. 본 레이저 (짧은 시간, 진한 파랑, 투사체 없음)
        yield return StartCoroutine(shooter.Fire(
            origin,
            direction,
            delay: 0.5f,
            lineLength: 20f,
            lineWidth: 1f,
            colorOverride: new Color(0f, 0f, 1f, 1f),
            fireProjectile: false
        ));

        // 3. 데미지 판정
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 12f, LayerMask.GetMask("Player"));
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.Log("Player hit by laser!");
            // TODO: 데미지 처리
        }
    }

    private IEnumerator RepeatLaserPattern()
    {
        while (phase == 3)
        {
            StartCoroutine(LaserPattern());
            yield return new WaitForSeconds(8f); // 쿨타임
        }
    }

    private void ActivatePhase2Patterns()
    {
        Debug.Log("Phase 2 패턴 활성화");
        StartCoroutine(RepeatShockwavePattern());
    }

    private void ActivatePhase3Patterns()
    {
        Debug.Log("Phase 3 패턴 활성화");
        StartCoroutine(RepeatLaserPattern());
    }

    private void ActivatePhase4Patterns()
    {
        Debug.Log("Phase 4 패턴 활성화");
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
