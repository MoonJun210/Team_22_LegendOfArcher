using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // ==== 외부 연결 프리팹 및 컴포넌트 ====
    [SerializeField] private GameObject warningZonePrefab;           // 경고존(폭파 등) 프리팹
    [SerializeField] private GameObject projectilePrefab;            // 투사체 프리팹
    [SerializeField] private ProjectileWarningShooter shooter;       // 경고선 및 발사 처리 클래스
    [SerializeField] private GameObject directionLinePrefab;         // 경고선 시각화용 프리팹

    private StatHandler statHandler; // 체력 관리용 핸들러
    private int phase = 1;           // 현재 페이즈 (1~4)

    GameObject _player;
    PlayerController _playerController;
    DieExplosion _die;

    private void Awake()
    {
        statHandler = GetComponent<StatHandler>();
        _die = GetComponent<DieExplosion>();
        EventManager.Instance.RegisterEvent<GameObject>("GetPlayerPosition", GetPlayerPosition);
    }

    private void Start()
    {
        //statHandler.CurrentHP = statHandler.MaxHP * 0.1f; // 디버그용 체력 설정
        // 디버그용 Player 태그를 가진 오브젝트 자동 연결
        //_player = GameObject.FindGameObjectWithTag("Player");
        //if (_player != null)
        //{
        //    _playerController = _player.GetComponent<PlayerController>();
        //    StartCoroutine(BossRoutine());
        //}
    }

    private void Update() 
    {
        if(_player != null)
        {
            CheckPhase();

        }
    }
    // 페이즈 변경 감지
    private void CheckPhase()
    {
        float hpRatio = statHandler.CurrentHP / statHandler.MaxHP;

        if (hpRatio <= 0.75f && phase < 2)
        {
            phase = 2;
            ActivatePhase2Patterns();
        }
        if (hpRatio <= 0.5f && phase < 3)
        {
            phase = 3;
            ActivatePhase3Patterns();
        }
        if (hpRatio <= 0.25f && phase < 4)
        {
            phase = 4;
            ActivatePhase4Patterns();
        }
    }
    // 통상 패턴 루프
    private IEnumerator BossRoutine()
    {
        while (statHandler.CurrentHP > 0)
        {
            yield return StartCoroutine(DoPattern());
        }

        OnDeath();
    }
    // 한 번의 통상 패턴 실행
    private IEnumerator DoPattern()
    {
        PerformRandomAttack();
        yield return new WaitForSeconds(2.5f);
    }
    // 통상 패턴 중 무작위 하나 선택
    private void PerformRandomAttack()
    {
        int pattern = Random.Range(0, 2);
        Debug.Log("패턴 실행: " + pattern);

        switch (pattern)
        {
            case 0: StartCoroutine(ExplosionPattern()); break;
            case 1: StartCoroutine(RangedPattern()); break;
        }
    }
    //지점 폭파 패턴
    private IEnumerator ExplosionPattern()
    {
        Vector3 targetPos = _player.transform.position;
        GameObject zone = Instantiate(warningZonePrefab, targetPos, Quaternion.identity);
        zone.transform.localScale = new Vector3(3f, 3f, 1f);

        yield return new WaitForSeconds(1f);
        Destroy(zone);

        // 지점 폭파 데미지
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 1.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("데미지");
                // TODO: 데미지 처리
                _playerController.TakeDamaged();

            }
        }
    }
    // 조준 투사체 발사
    private IEnumerator RangedPattern()
    {
        Vector3 origin = transform.position;
        Vector2 dir = (_player.transform.position - origin).normalized;

        // 별도 클래스가 경고 라인 + 투사체 발사 처리
        yield return StartCoroutine(shooter.Fire(origin, dir));
    }
    // 방사형 5방 투사체 발사
    private IEnumerator ShockwavePattern()
    {
        Vector3 spawnPos = transform.position;

        // 기준 방향
        Vector2 baseDir = (_player.transform.position - spawnPos).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        // 2. ±90도 범위 내에서 랜덤 5방 발사
        for (int i = 0; i < 5; i++)
        {
            float offset = Random.Range(-90f, 90f);
            float angle = baseAngle + offset;

            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            StartCoroutine(shooter.Fire(spawnPos, dir));
        }

        yield return new WaitForSeconds(1.2f); // 연사 후 대기
    }
    // 레이저 패턴
    private IEnumerator RepeatShockwavePattern()
    {
        while (phase >= 2)
        {
            yield return StartCoroutine(ShockwavePattern());

            float baseDelay = 6f;
            float delay = (phase == 4) ? baseDelay * 0.5f : baseDelay;

            yield return new WaitForSeconds(delay);
        }
    }


    private IEnumerator LaserPattern()
    {
        Vector3 origin = transform.position;
        Vector2 direction = (_player.transform.position - origin).normalized;

        // 1. 경고선 (기본값 사용)
        yield return StartCoroutine(shooter.Fire(
            origin,
            direction,
            lineWidth: 1f,
            fireProjectile: false
        ));

        // 2. 본 레이저 (시각 효과 먼저)
        StartCoroutine(shooter.Fire(
            origin,
            direction,
            delay: 0.5f,
            lineLength: 20f,
            lineWidth: 1f,
            colorOverride: new Color(0f, 0f, 1f, 1f),
            fireProjectile: false,
            growDuration: 0.07f
        ));

        // 3. 데미지 판정 - 0.5초 동안 반복해서 레이캐스트
        float damageDuration = 0.5f;
        float elapsed = 0f;
        bool hitPlayer = false;

        while (elapsed < damageDuration && !hitPlayer)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, 20f);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject != gameObject)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("Player hit during laser!");
                        _playerController.TakeDamaged();
                        hitPlayer = true;
                        break;
                    }
                }
            }

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }


    }

    private IEnumerator RepeatLaserPattern()
    {
        while (phase >= 3)
        {
            yield return StartCoroutine(LaserPattern());

            float baseDelay = 8f;
            float delay = (phase == 4) ? baseDelay * 0.5f : baseDelay;

            yield return new WaitForSeconds(delay);
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

    private void GetPlayerPosition(GameObject player)
    {
        _player = player;
        _playerController = _player.GetComponent<PlayerController>();
        StartCoroutine(BossRoutine());

    }

    private void OnDeath()
    {
        Debug.Log("보스 사망 처리");
        _die.ExecuteDeathSequence();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 15)
        {
            statHandler.TakeDamage(_playerController.GetPower());
            Destroy(collision.gameObject);
        }
    }
}
